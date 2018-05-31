using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

[CreateAssetMenu]
public class Lobby : ScriptableObject
{
    public CSteamID Owner { get; private set; }

    public CSteamID LobbyID;
    public List<User> Users;

    public void Reset()
    {
        SteamMatchmaking.LeaveLobby(LobbyID);
        LobbyID = new CSteamID(0);
        if (Users != null)
            Users.Clear();
        Owner = new CSteamID(0);
    }

    public User GetUserFromID(CSteamID id)
    {
        for (int i = 0; i < Users.Count; i++)
        {
            if (Users[i].SteamID == id)
                return Users[i];
        }
        return null;
    }

    public void SetOwner(CSteamID ID)
    {
        Owner = ID;
    }

    public bool ContainsCSteamID(CSteamID id)
    {
        for (int i = 0; i < Users.Count; i++)
        {
            if (Users[i].SteamID == id)
                return true;
        }
        return false;
    }
}
