using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;
using GENUtility;
using SOPRO;
public class PlayersMgr : MonoBehaviour
{
    public SimpleAvatar AvatarTemplate, ControllableAvatarTemplate;
    public Vector3 SpawnPosition;
    public Material[] Materials;

    static PlayersMgr instance;

    Dictionary<CSteamID, SimpleAvatar> avatars;

    public static Dictionary<CSteamID, SimpleAvatar> Players { get { return instance.avatars; } }

    // Use this for initialization
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        avatars = new Dictionary<CSteamID, SimpleAvatar>();

        for (int i = 0; i < Client.Users.Count; i++)
        {
            SimpleAvatar a;
            if (Client.Users[i].SteamID == Client.MyID)
                a = Instantiate(ControllableAvatarTemplate);
            else
                a = Instantiate(AvatarTemplate);

            a.UserInfo = Client.Users[i];
            a.transform.position = SpawnPosition;
            //a.gameObject.GetComponent<Renderer>().material = Materials[Client.Users[i].AvatarID];
            avatars.Add(Client.Users[i].SteamID, a);
        }

        Client.AddCommand(PacketType.PlayerDataServer, ManageServerTransform);
        Client.AddCommand(PacketType.PlayerData, ManageTransform);
        Client.AddCommand(PacketType.PlayerStatus, ManagePlayerStatus);
    }
    void ManagePlayerStatus(byte[] data, uint dataLength, CSteamID sender)
    {
        CSteamID target = (CSteamID)ByteManipulator.ReadUInt64(data, 0);

        if (!avatars.ContainsKey(target))
            return;

        Player player = avatars[target].GetComponent<Player>();

        player.Life = ByteManipulator.ReadSingle(data, 8);

        if (player.Life <= 0f)
            player.Die();
    }

    void ManageServerTransform(byte[] data, uint dataLength, CSteamID sender)
    {
        Client.SendPacketToInGameUsers(data, 0, (int)dataLength, PacketType.PlayerData, sender, EP2PSend.k_EP2PSendUnreliable);
    }

    void ManageTransform(byte[] data, uint dataLength, CSteamID sender)
    {
        if (!avatars.ContainsKey(sender) || Client.MyID == sender)
            return;

        SimpleAvatar avatar = avatars[sender];

        int overLength = (int)dataLength - 28;
        if (overLength > 0)
        {
            //Manage attached data
        }

        int offset = overLength;
        float posX = ByteManipulator.ReadSingle(data, offset);
        offset += sizeof(float);
        float posY = ByteManipulator.ReadSingle(data, offset);
        offset += sizeof(float);
        float posZ = ByteManipulator.ReadSingle(data, offset);
        offset += sizeof(float);
        float rotX = ByteManipulator.ReadSingle(data, offset);
        offset += sizeof(float);
        float rotY = ByteManipulator.ReadSingle(data, offset);
        offset += sizeof(float);
        float rotZ = ByteManipulator.ReadSingle(data, offset);
        offset += sizeof(float);
        float rotW = ByteManipulator.ReadSingle(data, offset);

        Vector3 pos = new Vector3(posX, posY, posZ);
        Quaternion rot = new Quaternion(rotX, rotY, rotZ, rotW);

        avatars[sender].SetTransform(pos, rot);
    }
}
