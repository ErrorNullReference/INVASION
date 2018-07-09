using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
using System;

public class FollowPlayer : MonoBehaviour
{
    public SOListPlayerContainer Players;
    public ReferenceVector3 Offset;
    public bool Debug;

    [NonSerialized]
    public int CurrentIndex = -1;

    public void SetFollowLocalPlayer()
    {
        if (!Debug)
        {
            for (int i = 0; i < Players.Elements.Count; i++)
            {
                Player p = Players[i];
                if (!p.Dead && p.Avatar.UserInfo.SteamID == Client.MyID)
                {
                    CurrentIndex = i;
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
        if (CurrentIndex >= 0)
            transform.position = Players[CurrentIndex].transform.position + Offset;
    }
}
