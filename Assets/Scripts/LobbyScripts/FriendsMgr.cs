using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class FriendsMgr : MonoBehaviour
{
    public Transform FriendsList, Content;
    public FriendInvite FriendTemplate;
    public ContentFitter fitter;
    List<FriendInvite> friends;
    int currIndex;

    void Start()
    {
        SteamCallbackReceiver.LobbyInviteEvent += AcceptInvite;
    }

    void OnDestroy()
    {
        SteamCallbackReceiver.LobbyInviteEvent -= AcceptInvite;
    }

    void OnEnable()
    {
        OpenFriendsList();
    }

    public void OpenFriendsList()
    {
        SetFriends();
    }

    public void CloseFriendsList()
    {
        gameObject.SetActive(false);
        ClearFriends();
    }

    void SetFriends()
    {
        if (friends == null)
            friends = new List<FriendInvite>();

        int n = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
        for (int i = 0; i < n; i++)
        {
            CSteamID friendID = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
            EPersonaState friendState = SteamFriends.GetFriendPersonaState(friendID);
            if (friendState != EPersonaState.k_EPersonaStateOffline)
            {
                GetFreeFriend().Create(SteamFriends.GetFriendPersonaName(friendID), friendID, this);
            }
        }
        fitter.Init();
    }

    void ClearFriends()
    {
        currIndex = 0;
    }

    void AcceptInvite(GameLobbyJoinRequested_t cb)
    {
        Client.LeaveCurrentLobby();
        FindObjectOfType<MenuMgr>().ReturnToSelection();
        SteamAPICall_t request = SteamMatchmaking.JoinLobby((CSteamID)cb.m_steamIDLobby);
        SteamCallbackReceiver.Set<LobbyEnter_t>(request);
    }

    FriendInvite GetFreeFriend()
    {
        int index = currIndex;
        if (index >= friends.Count)
        {
            FriendInvite friend = Instantiate(FriendTemplate, Content.transform);
            friends.Add(friend);
        }

        currIndex++;
        return friends[index];
    }
}
