using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;

public class MenuMgr : MonoBehaviour
{
    public static Action OnStart, OnAvatarSelection;

    public Canvas StartCanvas, AvatarSelection, Chat;

    // Use this for initialization
    void Start()
    {
        StartCanvas.gameObject.SetActive(true);

        SteamCallbackReceiver.LobbyEnterEvent += ChangeLobbyState;
        SteamCallbackReceiver.LobbyCreateEvent += ChangeLobbyState;
    }

    void ChangeLobbyState(LobbyEnter_t cb)
    {
        if (cb.m_ulSteamIDLobby != 0)
        {
            StartCanvas.gameObject.SetActive(false);
            AvatarSelection.gameObject.SetActive(true);
            Chat.gameObject.SetActive(true);

            OnAvatarSelection.Invoke();
        }
    }

    void ChangeLobbyState(LobbyCreated_t cb)
    {
        if (cb.m_ulSteamIDLobby != 0)
        {
            StartCanvas.gameObject.SetActive(false);
            AvatarSelection.gameObject.SetActive(true);
            Chat.gameObject.SetActive(true);

            OnAvatarSelection.Invoke();
        }
    }

    void OnDestroy()
    {
        SteamCallbackReceiver.LobbyEnterEvent -= ChangeLobbyState;
        SteamCallbackReceiver.LobbyCreateEvent -= ChangeLobbyState;
    }

    public void OpenFriendList()
    {
        //FriendsList.gameObject.SetActive(true);
        SteamFriends.ActivateGameOverlayInviteDialog(Client.Lobby.LobbyID);
    }

    public void ReturnToSelection()
    {
        StartCanvas.gameObject.SetActive(true);
        AvatarSelection.gameObject.SetActive(false);
        Chat.gameObject.SetActive(false);

        OnStart.Invoke();
    }

    public void UpdateCanvas()
    {
        StartCanvas.gameObject.SetActive(false);
        Chat.gameObject.SetActive(false);
        StartCanvas.gameObject.SetActive(true);
        Chat.gameObject.SetActive(true);
    }
}
