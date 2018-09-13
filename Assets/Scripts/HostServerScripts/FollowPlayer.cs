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
    public Vector3 StartOffset, StartRotOffset;
    public bool Debug;
    public float PauseDuration, AnimationDuration;

    [NonSerialized]
    public CSteamID CurrentID;
    [NonSerialized]
    public int CurrentIndex;
    bool onAnimation;
    float timer, frac;
    Vector3 startPos;
    Quaternion startRot, endRot;
    int state;

    public void SetFollowLocalPlayer()
    {
        if (!Debug)
        {
            for (int i = 0; i < Players.Elements.Count; i++)
            {
                Player p = Players[i];
                if (!p.Dead && p.Avatar.UserInfo != null && p.Avatar.UserInfo.SteamID == Client.MyID)
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
       
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return null;

        if (!Debug)
            CurrentIndex = GetIndex();
        Players[CurrentIndex].Avatar.Controller.Disable();
        onAnimation = true;
        timer = 0;
        startPos = transform.position = Players[CurrentIndex].transform.position + Players[CurrentIndex].transform.forward * StartOffset.z + new Vector3(StartOffset.x, StartOffset.y, 0);
        endRot = transform.rotation;
        startRot = transform.rotation = Quaternion.LookRotation((Players[CurrentIndex].transform.position + new Vector3(StartRotOffset.x, StartRotOffset.y, 0) - transform.position).normalized, Vector3.up);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CurrentIndex = GetIndex();

        if (state == 0)
        {
            timer += Time.deltaTime;
            if (timer >= PauseDuration)
            {
                timer = 0;
                state = 1;
            }
        }
        else if (state == 1)
        {
            timer += Time.deltaTime;
            frac = timer / AnimationDuration;
            transform.position = Vector3.Lerp(startPos, Players[CurrentIndex].transform.position + Offset, frac);
            transform.rotation = Quaternion.Slerp(startRot, endRot, frac);

            if (frac >= 1)
            {
                timer = 0;
                state = 2;
                Players[CurrentIndex].Avatar.Controller.Activate();
            }
        }
        else if (state == 2 && CurrentIndex != -1)
            transform.position = Players[CurrentIndex].transform.position + Offset;
    }

    int GetIndex()
    {
        if (Debug)
            return 0;

        for (int i = 0; i < Players.Elements.Count; i++)
        {
            if (Players.Elements[i].Avatar.UserInfo != null && Players.Elements[i].Avatar.UserInfo.SteamID == CurrentID)
                return i;
        }
        return -1;
    }
}
