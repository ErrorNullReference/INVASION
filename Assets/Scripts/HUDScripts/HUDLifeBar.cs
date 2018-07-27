using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDLifeBar : HUD
{
    int prevEnergy;
    public Transform transform;
    public Image image;
    public Gradient colorOverLife;

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

        if (player.Life != prevEnergy)
        {
            if (image != null)
                image.localScale = new Vector3(player.Life / player.MaxLife, 1, 1);
            prevEnergy = (int)player.Life;
        }*/
        if (Player == null)
            GetUserInfo();
        
        if (Player == null)
            return;

        if (Player.Life != prevEnergy)
        {
            if (transform != null)
                transform.localScale = new Vector3(Player.Life / Player.MaxLife, 1, 1);
            if (image != null)
                image.color = Player.Stats.PlayerHealthBarGradient.Evaluate(Player.Life / Player.MaxLife);
            prevEnergy = (int)Player.Life;
        }
    }
}
