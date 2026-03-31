Shader "URP2D/SpriteGlowOutline_8Samples_Final"
{
    Properties
    {
        [MainTexture] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)

        [Header(Glow Settings)]
        [HDR] _GlowColor("Glow Color", Color) = (1, 0, 0, 1) //  默认亮度也给足
        _GlowRange("Glow Range (1-9)", Range(1, 9)) = 2.0
        _GlowIntensity("Glow Intensity", Float) = 3.0 //  默认强度给足
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "RenderPipeline"="UniversalPipeline" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
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
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                half4 _GlowColor;
                float _GlowRange;
                float _GlowIntensity;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_TexelSize; // x=1/width, y=1/height, z=width, w=height

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.color = v.color * _Color;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * i.color;

                // 获取单个像素在 UV 空间的尺寸
                float2 texelSize = _MainTex_TexelSize.xy * _GlowRange;

                float totalAlpha = 0;
                
                // --- 4 个正方向 ---
                totalAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2( texelSize.x, 0)).a;
                totalAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2(-texelSize.x, 0)).a;
                totalAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2(0,  texelSize.y)).a;
                totalAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2(0, -texelSize.y)).a;

                // --- 4 个对角线方向 (权重稍微低一点点点，保证圆润) ---
                totalAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2( texelSize.x,  texelSize.y)).a * 0.707;
                totalAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2(-texelSize.x,  texelSize.y)).a * 0.707;
                totalAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2( texelSize.x, -texelSize.y)).a * 0.707;
                totalAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + float2(-texelSize.x, -texelSize.y)).a * 0.707;
                
                // 归一化并计算最终光晕 Alpha (saturate 保证在 0-1)
                // 用 1-texColor.a 确保只生成外发光
                float glowAlpha = saturate(totalAlpha / 6.828) * (1.0 - saturate(texColor.a * 10.0));

                // 混合颜色：原图 + 经强度加成和色彩加成的外发光
                half3 finalColor = texColor.rgb + (_GlowColor.rgb * glowAlpha * _GlowIntensity);
                
                // 最后的 Alpha 是两者的叠加
                float finalAlpha = saturate(texColor.a + glowAlpha * _GlowColor.a);

                return half4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
    }
}