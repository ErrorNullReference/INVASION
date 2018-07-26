using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDLife : HUD
{
    int prevEnergy;
    Player player;

    void LateUpdate()
    {
        if (!player)
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

        if (player.Life != prevEnergy)
        {
            textComponent.text = player.Life.ToString();
            prevEnergy = (int)player.Life;
        }
    }
}
