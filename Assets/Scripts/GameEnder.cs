using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SOPRO;

public class GameEnder : MonoBehaviour
{
    [SerializeField]
    private SOListPlayerContainer Players, ActivePlayers;
    [SerializeField]
    private Transform EndGamePanel;
    bool gameEnd;

    void Enable()
    {
        gameEnd = false;
    }

    void Update()
    {
        if (!gameEnd && Players.Elements.Count != 0 && ActivePlayers.Elements.Count == 0)
        {
            gameEnd = true;
            EndGamePanel.gameObject.SetActive(true);
            if (Client.OnGameEnd != null)
                Client.OnGameEnd.Invoke();
        }
    }

    public void LoadGameStart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
