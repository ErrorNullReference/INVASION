using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUDPlayerImage : HUD
{
    private RawImage playerImg;

    void Start ()
    {
        playerImg = gameObject.GetComponent<RawImage>();
    }
	
	void Update ()
    {
        if (Server.Users != null && Server.Users.Count > 0)
            playerImg.texture = Server.LocalPlayer.SteamAvatarImage;

    }
}
