using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using UnityEngine.SceneManagement;

public class StartGameMgr : MonoBehaviour
{
    public Button StartButton;
    List<CSteamID> readyUsers;

    void Start()
    {
        readyUsers = new List<CSteamID>();

        SteamCallbackReceiver.LobbyDataUpdateEvent += UpdateStartState;
        SteamCallbackReceiver.ChatUpdateEvent += UserLeftControl;
    }

    void OnDestroy()
    {
        SteamCallbackReceiver.LobbyDataUpdateEvent -= UpdateStartState;
        SteamCallbackReceiver.ChatUpdateEvent -= UserLeftControl;
    }

    void UpdateStartState(LobbyDataUpdate_t cb)
    {
        readyUsers.Clear();
        if (Client.Users == null || Client.Users.Count == 0)
            return;

        for (int i = 0; i < Client.Users.Count; i++)
        {
            string data = SteamMatchmaking.GetLobbyMemberData(Client.Lobby.LobbyID, Client.Users[i].SteamID, "Ready");
            if (data == "")
                continue;
            int index;
            if (int.TryParse(data, out index))
            {
                if (index == 1)
                {
                    readyUsers.Add(Client.Users[i].SteamID);
                }
            }
        }
        SetStartButtonState();
        ControlStart();
    }

    void UserLeftControl(LobbyChatUpdate_t cb)
    {
        if ((EChatMemberStateChange)cb.m_rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeLeft || (EChatMemberStateChange)cb.m_rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeDisconnected)
        {
            if (readyUsers.Contains((CSteamID)cb.m_ulSteamIDUserChanged))
                readyUsers.Remove((CSteamID)cb.m_ulSteamIDUserChanged);    
        }
    }

    void ControlStart()
    {
        if (readyUsers.Count == Client.Users.Count)
        {
            SceneManager.sceneLoaded += SetInGame;
            SceneManager.LoadScene(2);
        }
    }

    void SetInGame(Scene s, LoadSceneMode ls)
    {
        Server.Init();
        SteamMatchmaking.SetLobbyMemberData(Client.Lobby.LobbyID, "InGame", "1");
    }

    void SetStartButtonState()
    {
        StartButton.interactable = true;
        for (int i = 0; i < readyUsers.Count; i++)
        {
            if (readyUsers[i] == Client.MyID)
            {
                StartButton.interactable = false;
                return;
            }
        }
    }

    public void StartGame()
    {
        User u = Client.Lobby.GetUserFromID(Client.MyID);
        if (u != null && u.AvatarID != -1 && u.SteamAvatarImage != null)
            SteamMatchmaking.SetLobbyMemberData(Client.Lobby.LobbyID, "Ready", "1");
    }
}