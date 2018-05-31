using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;

public static class SteamCallbackReceiver
{
    #region

    public delegate void LobbyList(LobbyMatchList_t cb);

    public delegate void LobbyEnter(LobbyEnter_t cb);

    public delegate void LobbyCreate(LobbyCreated_t cb);

    public delegate void Chat(LobbyChatMsg_t cb);

    public delegate void ChatUpdate(LobbyChatUpdate_t cb);

    public delegate void LobbyInvite(GameLobbyJoinRequested_t cb);

    public delegate void LobbyData(LobbyDataUpdate_t cb);

    public delegate void PersonaState(PersonaStateChange_t cb);

    public delegate void AcceptP2P(P2PSessionRequest_t cb);

    public delegate void P2PFail(P2PSessionConnectFail_t cb);

    #endregion//Delegates

    #region

    public static event LobbyList LobbyListEvent;
    public static event LobbyEnter LobbyEnterEvent;
    public static event LobbyCreate LobbyCreateEvent;
    public static event Chat ChatMessageEvent;
    public static event ChatUpdate ChatUpdateEvent;
    public static event LobbyInvite LobbyInviteEvent;
    public static event LobbyData LobbyDataUpdateEvent;
    public static event PersonaState PersonaStateChangeEvent;
    public static event AcceptP2P AcceptP2PEvent;
    public static event P2PFail P2PFailEvent;

    #endregion//Events

    #region

    static CallResult<LobbyMatchList_t> lobbyList;
    static CallResult<LobbyEnter_t> lobbyEnter;
    static CallResult<LobbyCreated_t> lobbyCreate;
    static CallResult<PersonaStateChange_t> personaStateChange;

    #endregion//CallResults

    #region

    static Callback<LobbyChatMsg_t> chatMessage;
    static Callback<LobbyChatUpdate_t> chatUpdate;
    static Callback<GameLobbyJoinRequested_t> lobbyInvite;
    static Callback<LobbyDataUpdate_t> lobbyDataUpdate;
    static Callback<P2PSessionRequest_t> sessionRequest;
    static Callback<P2PSessionConnectFail_t> p2pFail;

    #endregion//CallBacks

    static Dictionary<Type, Action<SteamAPICall_t>> CallResults;

    public static void Init()
    {
        CallResults = new Dictionary<Type, Action<SteamAPICall_t>>();
        CallResults.Add(typeof(LobbyMatchList_t), (SteamAPICall_t cb) => lobbyList.Set(cb));
        CallResults.Add(typeof(LobbyEnter_t), (SteamAPICall_t cb) => lobbyEnter.Set(cb));
        CallResults.Add(typeof(LobbyCreated_t), (SteamAPICall_t cb) => lobbyCreate.Set(cb));
        CallResults.Add(typeof(PersonaStateChange_t), (SteamAPICall_t cb) => personaStateChange.Set(cb));


        //init callbacks
        lobbyList = CallResult<LobbyMatchList_t>.Create((cb, failure) =>
            {
                if (failure)
                {
                    #if UNITY_EDITOR
                    Debug.Log("Failed to received lobby list");
                    #endif
                    return;
                }

                if (LobbyListEvent != null)
                    LobbyListEvent.Invoke(cb);
            });
        
        lobbyEnter = CallResult<LobbyEnter_t>.Create((cb, failure) =>
            {
                if (failure)
                {
                    #if UNITY_EDITOR
                    Debug.Log("Failed to enter the specified lobby");
                    #endif
                    return;
                }

                if (LobbyEnterEvent != null)
                    LobbyEnterEvent.Invoke(cb);
            });

        lobbyCreate = CallResult<LobbyCreated_t>.Create((cb, failure) =>
            {
                if (failure)
                {
                    #if UNITY_EDITOR
                    Debug.Log("Failed to create a lobby");
                    #endif
                    return;
                }

                #if UNITY_EDITOR
                Debug.Log("Lobby creation resulted with: " + cb.m_eResult.ToString());
                #endif

                if (LobbyCreateEvent != null)
                    LobbyCreateEvent.Invoke(cb);
            });

        personaStateChange = CallResult<PersonaStateChange_t>.Create((cb, failure) =>
            {
                if (PersonaStateChangeEvent != null)
                    PersonaStateChangeEvent.Invoke(cb);
            });

        chatMessage = Callback<LobbyChatMsg_t>.Create(cb =>
            {
                if (ChatMessageEvent != null)
                    ChatMessageEvent.Invoke(cb);
            });

        chatUpdate = Callback<LobbyChatUpdate_t>.Create(cb =>
            {
                if (ChatUpdateEvent != null)
                    ChatUpdateEvent.Invoke(cb);
            });

        lobbyInvite = Callback<GameLobbyJoinRequested_t>.Create(cb =>
            {
                if (LobbyInviteEvent != null)
                    LobbyInviteEvent.Invoke(cb);
            });

        lobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(cb =>
            {
                if (LobbyDataUpdateEvent != null)
                    LobbyDataUpdateEvent.Invoke(cb);
            });

        sessionRequest = Callback<P2PSessionRequest_t>.Create(cb =>
            {
                if (AcceptP2PEvent != null)
                    AcceptP2PEvent.Invoke(cb);
            });

        p2pFail = Callback<P2PSessionConnectFail_t>.Create(cb =>
            {
                if (P2PFailEvent != null)
                    P2PFailEvent.Invoke(cb);
            });
    }

    public static bool Set<T>(SteamAPICall_t request)
    {
        if (CallResults.ContainsKey(typeof(T)))
        {
            CallResults[typeof(T)].Invoke(request);
            return true;
        }
        return false;
    }
}