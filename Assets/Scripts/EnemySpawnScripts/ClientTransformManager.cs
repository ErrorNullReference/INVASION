using UnityEngine;
using Steamworks;
using GENUtility;
using SOPRO;
[CreateAssetMenu(menuName = "ClientTransformManager")]
public class ClientTransformManager : ScriptableObject
{
    [SerializeField]
    private SODictionaryTransformContainer netEntities;

    public void RegisterTransformCommand()
    {
        Client.AddCommand(PacketType.EnemyTransform, EnemyTransformReceive);
    }

    private void EnemyTransformReceive(byte[] data, uint dataLength, CSteamID sender)
    {
        int index = 0;

        int id = ByteManipulator.ReadInt32(data, index);
        index += sizeof(int);

        float x = ByteManipulator.ReadSingle(data, index);
        index += sizeof(float);
        float y = ByteManipulator.ReadSingle(data, index);
        index += sizeof(float);
        float z = ByteManipulator.ReadSingle(data, index);
        index += sizeof(float);

        Vector3 position = new Vector3(x, y, z);

        x = ByteManipulator.ReadSingle(data, index);
        index += sizeof(float);
        y = ByteManipulator.ReadSingle(data, index);
        index += sizeof(float);
        z = ByteManipulator.ReadSingle(data, index);
        index += sizeof(float);
        float w = ByteManipulator.ReadSingle(data, index);

        Quaternion rotation = new Quaternion(x, y, z, w);

        if (netEntities.Elements.ContainsKey(id))
            netEntities[id].GetComponent<EnemyTransformSync>().ReceiveTransform(position, rotation);
    }
}