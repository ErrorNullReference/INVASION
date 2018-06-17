using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SOPRO;
using GENUtility;
using Steamworks;
[CreateAssetMenu(menuName = "Network/PowerUpsMgr")]
public class PowerUpsMgr : ScriptableObject
{
    public SODictionaryTransformContainer NetObjs;
    public SOBasicEvIntCSteamID PoweuUpPicked;
    [SerializeField]
    private SOPool[] powerUps = new SOPool[0];

    private Dictionary<byte, SOPool> pools = new Dictionary<byte, SOPool>();

    private void OnEnable()
    {
        pools.Clear();
        for (int i = 0; i < powerUps.Length; i++)
        {
            SOPool p = powerUps[i];
            pools.Add((byte)p.Prefab.GetComponent<PowerUp>().Type.Value, p);
        }
    }
    //private void OnValidate()
    //{
    //    OnEnable();
    //}
    private void NetSpawnedPowUp(byte[] data, uint length, CSteamID sender)
    {
        Vector3 pos = new Vector3(ByteManipulator.ReadSingle(data, 5), ByteManipulator.ReadSingle(data, 9), ByteManipulator.ReadSingle(data, 13));
        GetPowUp((PowerUpType)data[0], ByteManipulator.ReadInt32(data, 1), null, pos, Quaternion.identity);
    }
    public void Init()
    {
        Client.AddCommand(PacketType.PowerUpSpawn, NetSpawnedPowUp);
        Client.AddCommand(PacketType.PowerUpDespawn, NetDespawnedPowUp);
    }
    private void NetDespawnedPowUp(byte[] data, uint length, CSteamID sender)
    {
        int id = ByteManipulator.ReadInt32(data, 0);
        if (length > 4)
        {
            ulong playerId = ByteManipulator.ReadUInt64(data, 4);
            PoweuUpPicked.Raise(id, (CSteamID)playerId);
        }
        NetObjs.Elements[id].GetComponent<PowerUp>().Recycle();
    }

    private PowerUp GetPowUp(PowerUpType type, int id, Transform parent, Vector3 pos, Quaternion rot)
    {
        if (!pools.ContainsKey((byte)type))
            return null;

        SOPool pool = pools[(byte)type];

        int nullObjsRemoved;
        bool parented;
        PowerUp powUp = pool.DirectGet(parent, pos, rot, out nullObjsRemoved, out parented).GetComponent<PowerUp>();
        powUp.Pool = pool;

        powUp.gameObject.SetActive(true);

        powUp.GetComponent<GameNetworkObject>().SetNetworkId(id);

        return powUp;
    }
}