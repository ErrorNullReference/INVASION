using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDPlayerName : HUD
{
	void Update ()
    {
        if (Server.Users == null && Server.Users.Count <= 0)
            textComponent.text = "Player Name";
        else
            textComponent.text = Server.LocalPlayer.SteamUsername;  //Server.Users[InputAssetHUD.ClientID].SteamUsername;     
            //get player current name from server
    }
}
