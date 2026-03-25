using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBoard : MonoBehaviour
{
    [Header("核心系统引用")]
    public GridManager gridManager;
    public MishyManager mishyManager;
    public MatchSystem matchSystem;
    public MishyPlayerController playerController;
    public NextPushUpColumnMishy pushUpColumn;
    public MishyPreviewQueue previewQueue;
    public ScoreSystem scoreSystem;
    public SkillSystem skillSystem;


    [Header("特效引用")]
    public SkillBeamEffect skillBeamEffect;

    private void Awake()
    {
        // 自动去子物体身上找这些组件，省得你手动拖拽漏了
        gridManager = GetComponentInChildren<GridManager>();
        mishyManager = GetComponentInChildren<MishyManager>();
        matchSystem = GetComponentInChildren<MatchSystem>();
        playerController = GetComponentInChildren<MishyPlayerController>();
        pushUpColumn = GetComponentInChildren<NextPushUpColumnMishy>();
        previewQueue = GetComponentInChildren<MishyPreviewQueue>();
        scoreSystem = GetComponentInChildren<ScoreSystem>();
        skillSystem = GetComponentInChildren<SkillSystem>();
    }
}