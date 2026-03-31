using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneUI : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private PauseUI pauseUI;

    [SerializeField] private PlayerManager playerManager;

    private void Start()
    {
        pauseButton.onClick.AddListener(() =>
        {
            playerManager.TogglePause();
        });
    }

}
