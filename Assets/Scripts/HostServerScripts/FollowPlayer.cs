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
    public CSteamID CurrentIndex = -1;
    private int index;

    public void SetFollowLocalPlayer()
    {
        if (!Debug)
        {
            for (int i = 0; i < Players.Elements.Count; i++)
            {
                Player p = Players[i];
                if (!p.Dead && p.Avatar.UserInfo.SteamID == Client.MyID)
                {
                    CurrentIndex = Client.MyID;
                    break;
                }
            }
        }
        else
        {
            index = 0;
        }
    }

    private void Start()
    {
        SetFollowLocalPlayer();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Debug)
        {
            transform.position = Players[index].transform.position + Offset;
        }
        else
        {
            int i = GetIndex();
            if (i != -1)
                transform.position = Players[i].transform.position + Offset;
        }
    }

    int GetIndex()
    {
        for (int i = 0; i < Players.Elements.Count; i++)
        {
            if (Players.Elements[i].Avatar.UserInfo.SteamID == CurrentIndex)
                return i;
        }
        return -1;
    }
}
