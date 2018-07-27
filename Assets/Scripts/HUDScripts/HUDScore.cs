using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDScore : HUD
{
    int prevPoints;

    void LateUpdate()
    {
        /*if (!player)
        {
            for (int i = 0; i < DataContainer.Elements.Count; i++)
            {
                Player p = DataContainer[i];
                if (p.Avatar.UserInfo.SteamID == Client.MyID)
                {
                    player = p;
                    break;
                }
            }
        }

        if (!player)
            return;

        if (player.TotalPoints != prevPoints)
        {
            textComponent.text = player.TotalPoints.ToString();
            prevPoints = player.TotalPoints;
        }*/

        if (Player == null)
            GetUserInfo();
        
        if (Player == null)
            return;

        if (Player.TotalPoints != prevPoints)
        {
            textComponent.text = Player.TotalPoints.ToString();
            prevPoints = Player.TotalPoints;
        }
    }
}
