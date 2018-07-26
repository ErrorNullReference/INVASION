using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SOPRO;

public class GameEnder : MonoBehaviour
{
    [SerializeField]
    private int SceneID;
    [SerializeField]
    private SOListPlayerContainer Players, ActivePlayers;
    [SerializeField]
    private Transform EndGamePanel;
    bool gameEnd, startControl;

    void Enable()
    {
        gameEnd = false;
    }

    void Update()
    {
        if (!startControl)
        {
            if (Players.Elements.Count != 0 && ActivePlayers.Elements.Count != 0)
            {
                startControl = true;
            }
            return;
        }

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
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneID);
        if (Client.OnGameEnd != null)
            Client.OnGameEnd.Invoke();
    }
}
