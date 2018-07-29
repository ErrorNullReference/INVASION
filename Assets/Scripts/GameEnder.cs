using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SOPRO;
using Steamworks;

public class GameEnder : MonoBehaviour
{
    [SerializeField]
    private int SceneID;
    [SerializeField]
    private SOListPlayerContainer Players, ActivePlayers;
    [SerializeField]
    private Transform EndGamePanel;
    bool gameEnd, startControl;

    void Start()
    {
        Client.AddCommand(PacketType.GameOver, ActivateEnd);
    }

    void Enable()
    {
        gameEnd = false;
    }

    void Update()
    {
        if (!Client.IsHost)
            return;

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
            Client.SendPacketToInGameUsers(new byte[]{ 0 }, 0, 0, PacketType.GameOver, Steamworks.EP2PSend.k_EP2PSendReliable, false);
            ActivateEnd(null, 0, new CSteamID(0));
        }
    }

    void ActivateEnd(byte[] data, uint length, CSteamID id)
    {
        EndGamePanel.gameObject.SetActive(true);
        if (Client.OnGameEnd != null)
            Client.OnGameEnd.Invoke();
    }

    public void LoadGameStart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneID);
        if (Client.OnGameEnd != null)
            Client.OnGameEnd.Invoke();
    }
}
