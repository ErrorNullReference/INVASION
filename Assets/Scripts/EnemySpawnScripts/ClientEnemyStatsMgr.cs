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
    public ReferenceInt PlayerKillPointsMultiplicator;
    [SerializeField]
    public NetIdDispenser dispenser;
    [SerializeField]
    private PowerUpsMgr powUpMgr;
    [SerializeField]
    private SODictionaryTransformContainer netEntities;
    public void Init()
    {
        Client.AddCommand(PacketType.ShootHit, ManageEnemyHit);

        Client.AddCommand(PacketType.ShootCall, ShootCall);

        Client.AddCommand(PacketType.ShootHitServer, OnNetObjHit);
    }

    private void ManageEnemyHit(byte[] data, uint length, CSteamID sender)
    {
        int id = ByteManipulator.ReadInt32(data, 0);

        Enemy enemy = netEntities[id].GetComponent<Enemy>();

        if (!enemy)
            return;

        float damage = ByteManipulator.ReadSingle(data, 4);
        enemy.DecreaseLife(damage);

        ulong shooter = ByteManipulator.ReadUInt64(data, 8);

        int points = (int)(damage * PlayerDamagePointsMultiplicator);
        //if enemy dead give bonus points
        if (enemy.Life <= 0f)
        {
            points += (int)(enemy.Stats.MaxHealth * PlayerKillPointsMultiplicator);

            //only if this is host spawn energy power up
            if (Client.IsHost)
            {
                Vector3 position = enemy.transform.position;

                int netId = dispenser.GetNewNetId();

                byte[] addData = ArrayPool<byte>.Get(8);

                ByteManipulator.Write(addData, 0, shooter);

                powUpMgr.SendMsgSpawnPowerUp(PowerUpType.Energy, netId, position, addData, true);

                ArrayPool<byte>.Recycle(addData);
            }
        }

        PlayersMgr.Players[(CSteamID)shooter].GetComponent<Player>().TotalPoints += points; //TODO: rimuovere questo getcomponent
    }
    private void OnNetObjHit(byte[] data, uint length, CSteamID sender)
    {
        Client.SendPacketToInGameUsers(data, 0, (int)length, PacketType.ShootHit, EP2PSend.k_EP2PSendReliable, true);
    }
    private void ShootCall(byte[] data, uint length, CSteamID sender)
    {
        int id = ByteManipulator.ReadInt32(data, 0);
        netEntities[id].GetComponent<EnemyShootSync>().ReceiveShotCall();
    }
}
