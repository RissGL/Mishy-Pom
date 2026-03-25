using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerBoard player_one;
    [SerializeField] private PlayerBoard player_two;


    private void Awake()
    {
        player_one.matchSystem.OnGameOver += Player1_MatchSystem_OnGameOver;
        player_two.matchSystem.OnGameOver += Player2_MatchSystem_OnGameOver;

        player_one.skillSystem.OnSkillUse += Player1_SkillSystem_OnSkillUse;
        player_two.skillSystem.OnSkillUse += Player2_SkillSystem_OnSkillUse;
    }

    private void Player2_SkillSystem_OnSkillUse(object sender, SkillSystem.SkillUseInfo e)
    {
        player_one.pushUpColumn.PushMultiColumnMishyUp(e.matchColumnCount);
    }

    private void Player1_SkillSystem_OnSkillUse(object sender, SkillSystem.SkillUseInfo e)
    {
        player_two.pushUpColumn.PushMultiColumnMishyUp(e.matchColumnCount);
    }


    private void Player2_MatchSystem_OnGameOver(object sender, PlayerBoard e)
    {

    }

    private void Player1_MatchSystem_OnGameOver(object sender, PlayerBoard e)
    {
    }
}
