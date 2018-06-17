using UnityEngine;
using SOPRO;
using GENUtility;
using Steamworks;
[CreateAssetMenu(menuName = "Network/PowerUpsMgr")]
public class PowerUpsMgr : FactoryObj<byte>
{
    public SODictionaryTransformContainer NetObjs;
    public SOBasicEvIntCSteamID PoweuUpPicked;

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
        if (!organizedPools.ContainsKey((byte)type))
            return null;

        SOPool pool = organizedPools[(byte)type];

        int nullObjsRemoved;
        bool parented;
        PowerUp powUp = pool.DirectGet(parent, pos, rot, out nullObjsRemoved, out parented).GetComponent<PowerUp>();
        powUp.Pool = pool;

        powUp.gameObject.SetActive(true);

        powUp.GetComponent<GameNetworkObject>().SetNetworkId(id);

        return powUp;
    }
    protected override byte ExtractIdentifier(GameObject obj)
    {
        return (byte)obj.GetComponent<PowerUp>().Type.Value;
    }
}