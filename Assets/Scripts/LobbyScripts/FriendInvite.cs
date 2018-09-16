using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class FriendInvite : MonoBehaviour
{
    public Text Name;
    public Button InviteButton;
    public CSteamID id;
    FriendsMgr mgr;

    public void Create(string name, CSteamID id, FriendsMgr mgr)
    {
        Name.text = name;
        this.id = id;
        this.mgr = mgr;
        InviteButton.onClick.AddListener(Invite);
    }

    void Invite()
    {
        SteamMatchmaking.InviteUserToLobby(Client.Lobby.LobbyID, id);

        //mgr.CloseFriendsList();
    }
}
