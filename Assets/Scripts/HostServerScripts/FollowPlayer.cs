using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
using System;
using Steamworks;

public class FollowPlayer : MonoBehaviour
{
    public SOListPlayerContainer Players;
    public ReferenceVector3 Offset;
    public bool Debug;

    [NonSerialized]
    public CSteamID CurrentID;
    [NonSerialized]
    public int CurrentIndex;

    public void SetFollowLocalPlayer()
    {
        if (!Debug)
        {
            for (int i = 0; i < Players.Elements.Count; i++)
            {
                Player p = Players[i];
                if (!p.Dead && p.Avatar.UserInfo.SteamID == Client.MyID)
                {
                    CurrentID = Client.MyID;
                    break;
                }
            }
        }
        else
        {
            CurrentIndex = 0;
        }
    }

    private void Start()
    {
        SetFollowLocalPlayer();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CurrentIndex = GetIndex();
        if (CurrentIndex != -1)
            transform.position = Players[CurrentIndex].transform.position + Offset;
    }

    int GetIndex()
    {
        if (Debug)
            return 0;

        for (int i = 0; i < Players.Elements.Count; i++)
        {
            if (Players.Elements[i].Avatar.UserInfo.SteamID == CurrentID)
                return i;
        }
        return -1;
    }
}
