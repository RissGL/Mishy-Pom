using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BoardAudioSystem : MonoBehaviour
{
    private PlayerBoard board;
    private AudioSource audioSource;

    [Header("音效配置")]
    [SerializeField] private AudioClip badMishyMatchClip;     // 坏咪西消除
    [SerializeField] private AudioClip matchClip;     // 消除
    [SerializeField] private AudioClip pushSkillClip;     // 技能
    [SerializeField] private AudioClip matchKillClip;     // 技能
    [SerializeField] private AudioClip pushUpClip;    // 推上来的音效
    [SerializeField] private AudioClip gameOver;    // 游戏结束音效
    [SerializeField] private AudioClip comboClip;
    [SerializeField] private AudioClip moveClip;      // 左右移动
    [SerializeField] private AudioClip swapClipUp;// 交换
    [SerializeField] private AudioClip swapClipDown;// 交换
    [SerializeField] private AudioClip hardDropClip;  // 快速下落砸地
    [SerializeField] private AudioClip errorClip;     // 撞墙/错误
    [SerializeField] private AudioClip countdownClip; //倒计时音效

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        board = GetComponent<PlayerBoard>();

        audioSource.playOnAwake = false;
        CountdownUI.OnCountdownStart += CountdownUI_OnCountdownStart;
    }

    private void CountdownUI_OnCountdownStart(object sender, System.EventArgs e)
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(countdownClip, SFXConfig.sfxVolume);
    }

    private void Start()
    {
        board.matchSystem.OnSkillMatch += MatchSystem_OnSkillMatch;
        board.matchSystem.OnMatchCleared += MatchSystem_OnMatchCleared;
        board.matchSystem.OnBadMishyClear += MatchSystem_OnBadMishyClear;
        board.matchSystem.OnGameOver += MatchSystem_OnGameOver;

        board.skillSystem.OnSkillCast += SkillSystem_OnSkillUse;
        board.pushUpColumn.OnMultiMishyPushUp += PushUpColumn_OnMultiMishyPushUp;
        
    }

    private void OnDestroy()
    {
        board.matchSystem.OnSkillMatch -= MatchSystem_OnSkillMatch;
        board.matchSystem.OnMatchCleared -= MatchSystem_OnMatchCleared;
        board.matchSystem.OnBadMishyClear -= MatchSystem_OnBadMishyClear;
        board.matchSystem.OnGameOver -= MatchSystem_OnGameOver;

        board.skillSystem.OnSkillExecute -= SkillSystem_OnSkillUse;
        board.pushUpColumn.OnMultiMishyPushUp -= PushUpColumn_OnMultiMishyPushUp;
        CountdownUI.OnCountdownStart -= CountdownUI_OnCountdownStart;

    }

    private void PushUpColumn_OnMultiMishyPushUp(object sender, MishyType[][] e)
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(pushUpClip, SFXConfig.sfxVolume);
    }

    private void SkillSystem_OnSkillUse(object sender, SkillSystem.SkillUseInfo e)
    {
        if (e.isUpToEnemy)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(pushSkillClip, SFXConfig.sfxVolume);
        }
        else 
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(matchKillClip, SFXConfig.sfxVolume);
        }
    }

    private void MatchSystem_OnGameOver(object sender, PlayerBoard e)
    {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(gameOver, SFXConfig.sfxVolume);
    }

    private void MatchSystem_OnBadMishyClear(object sender, Vector3 e)
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(badMishyMatchClip, SFXConfig.sfxVolume);
    }

    private void MatchSystem_OnMatchCleared(object sender, MatchSystem.MatchInfo e)
    {
        float pitch = 1.0f + (e.matchCombo - 1) * 0.15f;

        pitch = Mathf.Clamp(pitch,1.0f, 2.2f);
        audioSource.pitch = pitch;

        audioSource.PlayOneShot(matchClip, SFXConfig.sfxVolume);

        if (e.matchCombo > 1)
        {
            audioSource.PlayOneShot(comboClip, SFXConfig.sfxVolume);
        }
    }

    private void MatchSystem_OnSkillMatch(object sender, int e)
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(matchClip, SFXConfig.sfxVolume);
    }

    public void PlayMoveSound()
    {
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(moveClip, SFXConfig.sfxVolume);
    }

    public void PlayErrorSound()
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(errorClip, SFXConfig.sfxVolume);
    }

    public void PlayDropSound()
    {
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(hardDropClip, SFXConfig.sfxVolume);
    }


    int swapCount = 0;
    public void PlaySwapSound()
    {
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        if (swapCount / 2 == 0)
        {
            audioSource.PlayOneShot(swapClipUp, SFXConfig.sfxVolume);

        }
        else 
        {
            audioSource.PlayOneShot(swapClipDown, SFXConfig.sfxVolume);
        }
    }
}
