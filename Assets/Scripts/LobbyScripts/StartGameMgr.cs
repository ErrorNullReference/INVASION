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
    public int GameSceneId = 1;
    List<CSteamID> readyUsers;
    List<CSteamID> confirmedUsers;

    void Awake()
    {
        Client.AddCommand(PacketType.ReceivedEnterGame, ReceivedEnterGameConfirmation);
        Client.AddCommand(PacketType.EnterGame, PrepareGameStart);
        Client.AddCommand(PacketType.ConfirmEnterGame, ControlStart);
        readyUsers = new List<CSteamID>();


        SteamCallbackReceiver.LobbyDataUpdateEvent += UpdateStartState;
        SteamCallbackReceiver.ChatUpdateEvent += UserLeftControl;
    }

    void OnDestroy()
    {
        SteamCallbackReceiver.LobbyDataUpdateEvent -= UpdateStartState;
        SteamCallbackReceiver.ChatUpdateEvent -= UserLeftControl;
    }

    void ReceivedEnterGameConfirmation(byte[] data, uint length, CSteamID sender)
    {
        if (!Client.IsHost)
            return;
        confirmedUsers.Add(sender);
        if (confirmedUsers.Count == readyUsers.Count)
        {
            Client.SendPacketToInGameUsers(emptyArray, 0, 0, PacketType.ConfirmEnterGame, EP2PSend.k_EP2PSendReliable, true);
        }
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

    void PrepareGameStart(byte[] data, uint length, CSteamID sender)
    {
        confirmedUsers = new List<CSteamID>();
        Server.Init();
        Client.SendPacketToHost(emptyArray, 0, 0, PacketType.ReceivedEnterGame, EP2PSend.k_EP2PSendReliable);
    }

    void ControlStart(byte[] data, uint length, CSteamID sender)
    {
        SceneManager.sceneLoaded += SetInGame;
        Client.LeaveCurrentLobby();
        SceneManager.LoadScene(GameSceneId);
    }

    void SetInGame(Scene s, LoadSceneMode ls)
    {
        SteamMatchmaking.SetLobbyMemberData(Client.Lobby.LobbyID, "InGame", "1");
        Client.SendPacketToHost(emptyArray, 0, 0, PacketType.GameEntered, EP2PSend.k_EP2PSendReliable);
        if (!Client.IsHost)
            Client.SendPacket(emptyArray, 0, 0, PacketType.GameEntered, Client.MyID, Client.MyID, EP2PSend.k_EP2PSendReliable);
        SceneManager.sceneLoaded -= SetInGame;
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