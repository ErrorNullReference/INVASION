using UnityEngine;
using SOPRO;
using GENUtility;
using Steamworks;
[CreateAssetMenu(menuName = "Network/PowerUpsMgr")]
public class PowerUpsMgr : Factory<byte>
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
    public void SendMsgSpawnPowerUp(PowerUpType type, int id, Vector3 pos, bool sendToSender = true)
    {
        byte[] data = ArrayPool<byte>.Get(17);

        data[0] = (byte)type;

        ByteManipulator.Write(data, 1, id);

        ByteManipulator.Write(data, 5, pos.x);
        ByteManipulator.Write(data, 9, pos.y);
        ByteManipulator.Write(data, 13, pos.z);

        Client.SendPacketToInGameUsers(data, 0, data.Length, PacketType.PowerUpSpawn, EP2PSend.k_EP2PSendReliable, sendToSender);

        ArrayPool<byte>.Recycle(data);
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

    public PowerUp GetPowUp(PowerUpType type, int id, Transform parent, Vector3 pos, Quaternion rot)
    {
        if (!organizedPools.ContainsKey((byte)type))
            return null;

        SOPool pool = organizedPools[(byte)type];

        int nullObjsRemoved;
        bool parented;
        PowerUp powUp = pool.DirectGet(parent, pos, rot, out nullObjsRemoved, out parented).GetComponent<PowerUp>();
        powUp.Pool = pool;

        powUp.GetComponent<GameNetworkObject>().SetNetworkId(id);

        powUp.gameObject.SetActive(true);

        return powUp;
    }
    protected override byte ExtractIdentifier(GameObject obj)
    {
        return (byte)obj.GetComponent<PowerUp>().Type.Value;
    }
}