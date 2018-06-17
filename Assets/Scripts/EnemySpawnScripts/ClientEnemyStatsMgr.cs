using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using SOPRO;
using GENUtility;
[CreateAssetMenu(menuName = "Network/EnemyMgr")]
public class ClientEnemyStatsMgr : ScriptableObject
{
    public ReferenceFloat PlayerDamagePointsMultiplicator;
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

        Enemy enemy = netEntities[id].GetComponent<Enemy>();

        float damage = ByteManipulator.ReadSingle(data, 4);
        enemy.DecreaseLife(damage);

        ulong shooter = ByteManipulator.ReadUInt64(data, 8);

        int points = (int)(damage * PlayerDamagePointsMultiplicator);
        //if enemy dead give bonus points
        if (enemy.Life <= 0f)
            points += (int)(enemy.Stats.MaxHealth * PlayerDamagePointsMultiplicator);

        PlayersMgr.Players[(CSteamID)shooter].GetComponent<Player>().TotalPoints += points;
    }

    private void ShootCall(byte[] data, uint length, CSteamID sender)
    {
        int id = ByteManipulator.ReadInt32(data, 0);
        netEntities[id].GetComponent<EnemyShootSync>().ReceiveShotCall();
    }
}
