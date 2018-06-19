using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDEnergy : HUD
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

        if (player.Energy != prevEnergy)
        {
            textComponent.text = player.Energy.ToString();
            prevEnergy = player.Energy;
        }
    }
}
