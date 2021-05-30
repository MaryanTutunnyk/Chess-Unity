using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Toggle twoPlayersGame;
    [SerializeField] private Toggle gameWithComputer;
    private void Start()
    {
        twoPlayersGame.Select();
    }
    public void TwoPlayersGameSelect()
    {
        twoPlayersGame.Select();
    }
    public void GameWithComputerSelect()
    {
        gameWithComputer.Select();
    }

    public void GoToGameScene()
    {
        if (twoPlayersGame.isOn)
        {
            SceneManager.LoadScene(1);
        }
        if (gameWithComputer.isOn)
        {
            SceneManager.LoadScene(2);
        }
    }
}
