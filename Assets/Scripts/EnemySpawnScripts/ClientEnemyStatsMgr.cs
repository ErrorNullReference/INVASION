using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class ClientEnemyStatsMgr : MonoBehaviour {

    private void Awake()
    {
        Client.AddCommand(PacketType.ShootHit, SetHealthStat);
    }

    public void SetHealthStat(byte[] data,uint length,CSteamID sender)
    {
        int id = data[0];
        ClientTransformManager.IdEnemies[id].gameObject.GetComponent<Enemy>().DecreaseLife();
    }
}
