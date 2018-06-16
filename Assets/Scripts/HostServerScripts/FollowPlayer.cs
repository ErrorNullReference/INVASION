using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
public class FollowPlayer : MonoBehaviour
{
    public SOListPlayerContainer Players;
    public Vector3 Offset;

    public int CurrentIndex;
    public void SetFollowLocalPlayer()
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
    private void Start()
    {
        SetFollowLocalPlayer();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Players[CurrentIndex].transform.position + Offset;
    }
}
