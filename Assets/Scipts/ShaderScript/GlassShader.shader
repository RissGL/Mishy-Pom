Shader "URP2D/SpriteGlass_SweepWave"
{
    Properties
    {
        [MainTexture] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Sprite Tint", Color) = (1,1,1,1)

        _NoiseTex("Noise Texture", 2D) = "grey" {}
        _NoiseScale("Noise Scale", Float) = 3.0
        
        [Header(Sweep Wave Settings)]
        _WaveSpeed("Wave Speed", Float) = 1.5
        _WaveWidth("Wave Width", Float) = 0.15
        _Interval("Pause Interval", Float) = 3.0
        
        _Distortion("Distortion", Range(0, 0.5)) = 0.08
        _GlassTint("Glass Tint", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "RenderPipeline"="UniversalPipeline" 
            "PreviewType"="Plane" 
        }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;       
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _NoiseTex_ST;
                float4 _Color;
                float _NoiseScale;
                float _WaveSpeed;
                float _WaveWidth;
                float _Interval;
                float _Distortion;
                half4 _GlassTint;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            TEXTURE2D(_CameraSortingLayerTexture);
            SAMPLER(sampler_CameraSortingLayerTexture);

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.color = v.color * _Color;
                o.uv = v.uv; 
                o.screenPos = ComputeScreenPos(o.positionCS);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float2 screenUV = i.screenPos.xy / i.screenPos.w;

                // 1. 让噪声图缓慢流动，增加一点有机感
                float2 noiseUV = i.uv * _NoiseScale;
                noiseUV.y -= _Time.y * (_WaveSpeed * 0.3); 
                noiseUV = frac(noiseUV);

                half4 noiseColor = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV);
                
                // 2. 核心：计算扫描带 (加入停顿间隔 Interval)
                // 让扫描带的 Y 坐标从 -0.5 走到 1.5（完整越过画面），剩下的大把时间就是 Interval 停顿
                float timeOffset = fmod(_Time.y * _WaveSpeed, 2.0 + _Interval) - 0.5;
                
                // 计算当前像素离扫描带中心的距离
                float dist = abs(i.uv.y - timeOffset);
                
                // 生成一个柔和的遮罩，只有在波浪扫过的区域，mask 才是 1，其余地方全是 0
                float sweepMask = 1.0 - smoothstep(_WaveWidth, _WaveWidth + 0.2, dist);

                // 3. 把遮罩乘在扭曲力度上！没有波浪扫过的地方，折射完全静止！
                float2 offset = (noiseColor.rg - 0.5) * 2.0 * _Distortion * sweepMask;

                // 4. 抓取屏幕并输出
                float2 finalGrabUV = screenUV + offset;
                half4 grabColor = SAMPLE_TEXTURE2D(_CameraSortingLayerTexture, sampler_CameraSortingLayerTexture, finalGrabUV);

                half3 finalColor = grabColor.rgb * _GlassTint.rgb * i.color.rgb;

                return half4(finalColor, _GlassTint.a * i.color.a);
            }
            ENDHLSL
        }
    }
}