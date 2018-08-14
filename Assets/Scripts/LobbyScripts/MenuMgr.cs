using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class MenuMgr : MonoBehaviour
{
	public Canvas EnterLobby, AvatarSelection, FriendsList, Chat;

	// Use this for initialization
	void Start ()
	{
		EnterLobby.gameObject.SetActive (true);

		SteamCallbackReceiver.LobbyEnterEvent += ChangeLobbyState;
		SteamCallbackReceiver.LobbyCreateEvent += ChangeLobbyState;
	}

	void ChangeLobbyState (LobbyEnter_t cb)
	{
		if (cb.m_ulSteamIDLobby != 0) {
			EnterLobby.gameObject.SetActive (false);
			AvatarSelection.gameObject.SetActive (true);
			Chat.gameObject.SetActive (true);
		}
	}

	void ChangeLobbyState (LobbyCreated_t cb)
	{
		if (cb.m_ulSteamIDLobby != 0) {
			EnterLobby.gameObject.SetActive (false);
			AvatarSelection.gameObject.SetActive (true);
			Chat.gameObject.SetActive (true);
		}
	}

	void OnDestroy ()
	{
		SteamCallbackReceiver.LobbyEnterEvent -= ChangeLobbyState;
		SteamCallbackReceiver.LobbyCreateEvent -= ChangeLobbyState;
	}

	public void OpenFriendList ()
	{
		//FriendsList.gameObject.SetActive(true);
		SteamFriends.ActivateGameOverlayInviteDialog (Client.Lobby.LobbyID);
	}

	public void ReturnToSelection ()
	{
		EnterLobby.gameObject.SetActive (true);
		AvatarSelection.gameObject.SetActive (false);
		Chat.gameObject.SetActive (false);
	}
}
