using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private const string gameName = "Game";
    private const string gameWithAIName = "GameWithAI";
    private const string gameWithRealPlayerName = "GameWithRealPlayer";
    private const string gameWithVirtualPlayerName = "GameWithVirtualPlayer";

    [SerializeField] private Toggle gameWithAI;
    [SerializeField] private Toggle gameWithRealPlayer;
    [SerializeField] private Toggle gameWithVirtualPlayer;

    public void GameWithAISelect()
    {
        gameWithRealPlayer.Select();
    }

    public void GameWithRealPlayerSelect()
    {
        gameWithRealPlayer.Select();
    }

    public void GameWithVirtualPlayerSelect()
    {
        gameWithVirtualPlayer.Select();
    }

    public void GoToGameScene()
    {
        if (gameWithAI.isOn)
        {
            PlayerPrefs.SetString(gameName, gameWithAIName);
        }
        if (gameWithRealPlayer.isOn)
        {
            PlayerPrefs.SetString(gameName, gameWithRealPlayerName);
        }
        if (gameWithVirtualPlayer.isOn)
        {
            PlayerPrefs.SetString(gameName, gameWithVirtualPlayerName);
        }
        PlayerPrefs.Save();

        SceneManager.LoadScene(1);
    }
}
