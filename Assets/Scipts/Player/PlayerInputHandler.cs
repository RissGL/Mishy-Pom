using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(MishyPlayerController))]
public class PlayerInputHandler : MonoBehaviour
{
    private MishyPlayerController controller;
    private SkillSystem skillSystem;

    [Header("移动控制")]
    public InputActionReference leftMoveAction;
    public InputActionReference rightMoveAction;
    public InputActionReference swapAction;
    public InputActionReference fastFallAction;

    [Header("技能按键")]
    public InputActionReference attackSkill;
    public InputActionReference defSkill;


    private void Awake()
    {
        controller= GetComponent<MishyPlayerController>();

        PlayerBoard board=GetComponentInParent<PlayerBoard>();
        skillSystem=board.skillSystem;
    }

    private void OnEnable()
    {
        leftMoveAction?.action.Enable();
        rightMoveAction?.action.Enable();
        swapAction?.action.Enable();
        fastFallAction?.action.Enable();

        attackSkill?.action.Enable();
        defSkill?.action.Enable();
    }

    private void OnDisable()
    {
        rightMoveAction?.action.Disable();
        swapAction?.action.Disable();
        fastFallAction?.action.Disable();
        leftMoveAction?.action.Disable();

        attackSkill?.action.Disable();
        defSkill?.action.Disable();
    }

    private void Update()
    {
        if (PlayerManager.CurrentState != PlayerManager.GameState.Playing)
        {
            controller.IsHoldingFastFall = false;
            return;
        }

        if (leftMoveAction.action.WasPressedThisFrame()) controller.CmdMoveLeft();
        if (rightMoveAction.action.WasPressedThisFrame()) controller.CmdMoveRight();
        if (swapAction.action.WasPressedThisFrame()) controller.CmdSwap();
        if (fastFallAction.action.WasPressedThisFrame()) controller.CmdFastFallTrigger();

        controller.IsHoldingFastFall = fastFallAction.action.IsPressed();

        if (defSkill.action.WasPressedThisFrame()) skillSystem.UseSkill(false);
        if (attackSkill.action.WasPressedThisFrame()) skillSystem.UseSkill(true);
    }
}
