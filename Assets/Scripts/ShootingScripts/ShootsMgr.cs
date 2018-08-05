using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Steamworks;
using GENUtility;

public class ShootsMgr : MonoBehaviour
{
    static ShootsMgr Instance;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        Client.AddCommand(PacketType.PlayerShoot, ReceiveShoot);
        Client.AddCommand(PacketType.PlayerShootServer, ReceiveShootServer);
    }

    void ReceiveShoot(byte[] data, uint lenght, CSteamID sender)
    {
        if (sender != Client.MyID)
            PlayersMgr.Players[sender].Shoot();
    }

    void ReceiveShootServer(byte[] data, uint lenght, CSteamID sender)
    {
        Client.SendPacketToInGameUsers(data, 0, (int)lenght, PacketType.PlayerShoot, sender, EP2PSend.k_EP2PSendReliable, false);
        if (sender != Client.MyID)
            PlayersMgr.Players[sender].Shoot();
    }

    void OnDestroy()
    {
        Instance = null;
    }
}
