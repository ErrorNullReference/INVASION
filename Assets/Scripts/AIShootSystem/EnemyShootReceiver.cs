using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShootReceiver : MonoBehaviour
{
    public EnemySpawner Spawner;

    // Use this for initialization
    void Start()
    {
        Client.AddCommand(PacketType.EnemyShoot, ReceiveShoot);	
    }

    void ReceiveShoot(byte[] data, uint length, Steamworks.CSteamID id)
    {
        int netID = System.BitConverter.ToInt32(data, 0);
        for (int i = 0; i < Spawner.enemyPool.objs.Values.Count; i++)
        {
            Enemy m = Spawner.enemyPool.objs[i];
            if (m.NetObj.NetworkId == netID)
                m.Shoot(0);
        }
    }
}
