using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using SOPRO;
using GENUtility;
[CreateAssetMenu(menuName = "ClientEnemyStatsMgr")]
public class ClientEnemyStatsMgr : ScriptableObject
{

    [SerializeField]
    private SODictionaryTransformContainer netEntities;
    public void Init()
    {
        Client.AddCommand(PacketType.ShootHit, SetHealthStat);

        Client.AddCommand(PacketType.ShootCall, ShootCall);
    }

    private void SetHealthStat(byte[] data, uint length, CSteamID sender)
    {
        int id = ByteManipulator.ReadInt32(data, 0);
        netEntities[id].GetComponent<Enemy>().DecreaseLife();
    }

    private void ShootCall(byte[] data, uint length, CSteamID sender)
    {
        int id = ByteManipulator.ReadInt32(data, 0);
        netEntities[id].GetComponent<EnemyShootSync>().ReceiveShotCall();
    }
}
