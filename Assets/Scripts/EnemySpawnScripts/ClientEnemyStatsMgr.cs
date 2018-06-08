using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class ClientEnemyStatsMgr : MonoBehaviour {

    private void Awake()
    {
        Client.AddCommand(PacketType.ShootHit, SetHealthStat);

        Client.AddCommand(PacketType.ShootCall, ShootCall);
    }

    public void SetHealthStat(byte[] data,uint length,CSteamID sender)
    {
        int id = data[0];
        ClientTransformManager.IdEnemies[id].gameObject.GetComponent<Enemy>().DecreaseLife();
    }

    public void ShootCall(byte[] data, uint length, CSteamID sender)
    {
        int id = data[0];
        ClientTransformManager.IdEnemies[id].gameObject.GetComponent<EnemyShootSync>().ReceiveShotCall();
    }
}
