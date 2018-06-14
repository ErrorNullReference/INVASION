using UnityEngine;
using Steamworks;
using GENUtility;
using SOPRO;
[CreateAssetMenu(menuName = "Network/TransformMgr")]
public class ClientTransformManager : ScriptableObject
{
    [SerializeField]
    private SODictionaryTransformContainer netEntities;

    public void RegisterTransformCommand()
    {
        Client.AddCommand(PacketType.NetObjTransform, NetObjTransformReceive);
    }

    private void NetObjTransformReceive(byte[] data, uint dataLength, CSteamID sender)
    {
        int id = ByteManipulator.ReadInt32(data, 0);

        if (!netEntities.Elements.ContainsKey(id))
            return;

        NetObjTransformSync sync = netEntities[id].GetComponent<NetObjTransformSync>();

        if (!sync)
            return;

        Vector3 position = new Vector3(ByteManipulator.ReadSingle(data, 4), ByteManipulator.ReadSingle(data, 8), ByteManipulator.ReadSingle(data, 12));
        Quaternion rotation = new Quaternion(ByteManipulator.ReadSingle(data, 16), ByteManipulator.ReadSingle(data, 20), ByteManipulator.ReadSingle(data, 24), ByteManipulator.ReadSingle(data, 28));

        sync.ReceiveTransform(position, rotation);
    }
}