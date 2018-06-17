using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDScore : HUD
{
	void Update ()
    {
        for (int i = 0; i < DataContainer.Elements.Count; i++)
        {
            if (DataContainer.Elements[i].Avatar.UserInfo.SteamID == Client.MyID)
            {
                textComponent.text = DataContainer.Elements[i].TotalPoints.ToString();
            }
        }
    }
}
