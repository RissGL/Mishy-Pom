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

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        board = GetComponent<PlayerBoard>();

        audioSource.playOnAwake = false;
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
    }

    private void PushUpColumn_OnMultiMishyPushUp(object sender, MishyType[][] e)
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(pushUpClip);
    }

    private void SkillSystem_OnSkillUse(object sender, SkillSystem.SkillUseInfo e)
    {
        if (e.isUpToEnemy)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(pushSkillClip);
        }
        else 
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(matchKillClip);
        }
    }

    private void MatchSystem_OnGameOver(object sender, PlayerBoard e)
    {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(gameOver);
    }

    private void MatchSystem_OnBadMishyClear(object sender, Vector3 e)
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(badMishyMatchClip);
    }

    private void MatchSystem_OnMatchCleared(object sender, MatchSystem.MatchInfo e)
    {
        float pitch = 1.0f + (e.matchCombo - 1) * 0.15f;

        pitch = Mathf.Clamp(pitch,1.0f, 2.2f);

        audioSource.PlayOneShot(matchClip, 0.8f);

        if (e.matchCombo > 1)
        {
            audioSource.PlayOneShot(comboClip, 1.0f);
        }
    }

    private void MatchSystem_OnSkillMatch(object sender, int e)
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(matchClip, 0.8f);
    }

    public void PlayMoveSound()
    {
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(moveClip, 0.5f);
    }

    public void PlayErrorSound()
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(errorClip, 0.6f);
    }

    public void PlayDropSound()
    {
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(hardDropClip, 0.5f);
    }


    int swapCount = 0;
    public void PlaySwapSound()
    {
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        if (swapCount / 2 == 0)
        {
            audioSource.PlayOneShot(swapClipUp, 0.5f);

        }
        else 
        {
            audioSource.PlayOneShot(swapClipDown, 0.5f);
        }
    }
}
