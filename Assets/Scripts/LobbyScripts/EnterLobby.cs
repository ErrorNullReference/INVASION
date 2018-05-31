using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class EnterLobby : MonoBehaviour
{
    bool enterLobby;

    void OnEnable()
    {
        enterLobby = false;

        SteamCallbackReceiver.LobbyListEvent += ListCall;
        SteamCallbackReceiver.LobbyEnterEvent += EnterCall;
        SteamCallbackReceiver.LobbyCreateEvent += CreateCall;
    }

    public void SearchLobby()
    {
        if (!enterLobby)
        {
            enterLobby = true;

            SteamAPICall_t requestLobbyList = SteamMatchmaking.RequestLobbyList();
            SteamCallbackReceiver.Set<LobbyMatchList_t>(requestLobbyList);
        }   
    }

    public void EnterPrivateLobby()
    {
        if (!enterLobby)
        {
            enterLobby = true;
            CreateLobby(ELobbyType.k_ELobbyTypePrivate);
        }    
    }

    void JoinLobby(CSteamID lobbyID)
    {
        SteamAPICall_t requestEnter = SteamMatchmaking.JoinLobby(lobbyID);

        SteamCallbackReceiver.Set<LobbyEnter_t>(requestEnter);
    }

    void CreateLobby(ELobbyType type)
    {
        SteamAPICall_t requestCreate = SteamMatchmaking.CreateLobby(type, Client.MaxNumOfPlayers);
        SteamCallbackReceiver.Set<LobbyCreated_t>(requestCreate);
    }

    void ListCall(LobbyMatchList_t cb)
    {
        if (cb.m_nLobbiesMatching != 0)
            JoinLobby(SteamMatchmaking.GetLobbyByIndex(0));
        else
            CreateLobby(ELobbyType.k_ELobbyTypePublic);
    }

    void EnterCall(LobbyEnter_t cb)
    {
        enterLobby = false;
        #if UNITY_EDITOR
        Debug.Log("Lobby ID : " + cb.m_ulSteamIDLobby);
        #endif
    }

    void CreateCall(LobbyCreated_t cb)
    {
        enterLobby = false;
        #if UNITY_EDITOR
        Debug.Log("Lobby ID : " + cb.m_ulSteamIDLobby);
        #endif
    }
}
