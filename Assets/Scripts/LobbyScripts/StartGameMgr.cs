using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using UnityEngine.SceneManagement;
using SOPRO;
public class StartGameMgr : MonoBehaviour
{
    private static readonly byte[] emptyArray = new byte[0];
    public Button StartButton;
    public SOEvVoid EnteredGame;
    public int GameSceneId = 2;
    List<CSteamID> readyUsers;

    void Awake()
    {
        Client.AddCommand(PacketType.EnterGame, ControlStart);
        readyUsers = new List<CSteamID>();

        SteamCallbackReceiver.LobbyDataUpdateEvent += UpdateStartState;
        SteamCallbackReceiver.ChatUpdateEvent += UserLeftControl;
    }

    void OnDestroy()
    {
        Client.AddCommand(PacketType.EnterGame, null);
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

        if (Client.IsHost && readyUsers.Count == Client.Users.Count)
            Client.SendPacketToLobby(emptyArray, 0, 0, PacketType.EnterGame, EP2PSend.k_EP2PSendReliable, true);
    }

    void UserLeftControl(LobbyChatUpdate_t cb)
    {
        if ((EChatMemberStateChange)cb.m_rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeLeft || (EChatMemberStateChange)cb.m_rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeDisconnected)
        {
            if (readyUsers.Contains((CSteamID)cb.m_ulSteamIDUserChanged))
                readyUsers.Remove((CSteamID)cb.m_ulSteamIDUserChanged);
        }
    }

    void ControlStart(byte[] data, uint length, CSteamID sender)
    {
        SceneManager.sceneLoaded += SetInGame;
        Server.Init();
        Client.LeaveCurrentLobby();
        SceneManager.LoadScene(GameSceneId);
    }

    void SetInGame(Scene s, LoadSceneMode ls)
    {
        SteamMatchmaking.SetLobbyMemberData(Client.Lobby.LobbyID, "InGame", "1");
        EnteredGame.Raise();
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