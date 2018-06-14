using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using SOPRO;
using GENUtility;

[CreateAssetMenu(menuName = "Network/EnemyHitMgr")]
public class EnemyHitMgr : ScriptableObject
{
    [SerializeField]
    private SODictionaryTransformContainer netEntities;
    public void Init()
    {
        if (Client.IsHost)
            return;
        Client.AddCommand(PacketType.ShootHitServer, OnNetObjHit);
    }

    private void OnNetObjHit(byte[] data, uint length, CSteamID sender)
    {
        int id = ByteManipulator.ReadInt32(data, 0);

        Enemy e = netEntities[id].GetComponent<Enemy>();
        //Debug.Log("hit received");
        e.DecreaseLife();
        if (CheckLife(e))
            Client.SendPacketToInGameUsers(data, 0, (int)length, PacketType.ShootHit, EP2PSend.k_EP2PSendReliable, false);
        //send to all but this client a packet with hit command only if e life is not 0 
        //else send enemy death;
    }

    private bool CheckLife(Enemy e)
    {
        //will then send status info to players and they'll update their enemies lives
        if (e.Life <= 0)
        {
            e.DestroyAndRecycle();
            HostEnemyDestroyer.EnemyToRecycleToAdd.Add(e);
            return false;
        }
        return true;
    }
}
