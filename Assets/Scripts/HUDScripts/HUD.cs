using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SOPRO;

public abstract class HUD : MonoBehaviour
{
    public SOListPlayerContainer DataContainer;
    public HeadsUpDisplay InputAssetHUD;

    protected Text textComponent;
    protected Player Player;
    bool userInfoNull;

    void Start()
    {
        try
        {
            textComponent = gameObject.GetComponent<Text>();
        }
        catch
        {
        }
    }

    public void GetUserInfo()
    {
        if (userInfoNull)
            return;

        UserInfo info = GetComponentInParent<UserInfo>();
        if (info != null)
        {
            for (int i = 0; i < DataContainer.Elements.Count; i++)
            {
                if (DataContainer.Elements[i].Avatar.UserInfo.SteamID == info.ID)
                    Player = DataContainer.Elements[i];
            }
        }
        else
            userInfoNull = true;
    }
}
