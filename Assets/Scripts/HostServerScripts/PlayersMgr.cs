using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;

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
            a.gameObject.GetComponent<Renderer>().material = Materials[Client.Users[i].AvatarID];
            avatars.Add(Client.Users[i].SteamID, a);
        }

        Client.Commands[(int)PacketType.ServerTransform] = ManageServerTransform;
        Client.Commands[(int)PacketType.Transform] = ManageTransform;
    }

    void ManageServerTransform(byte[] data, uint dataLength, CSteamID sender)
    {
        Client.SendPacketToInGameUsers(data, PacketType.Transform, sender, EP2PSend.k_EP2PSendUnreliable);
    }

    void ManageTransform(byte[] data, uint dataLength, CSteamID sender)
    {
        if (avatars.ContainsKey(sender))
        {
            int offset = 0;
            float posX = BitConverter.ToSingle(data, offset);
            offset += sizeof(float);
            float posY = BitConverter.ToSingle(data, offset);
            offset += sizeof(float);
            float posZ = BitConverter.ToSingle(data, offset);
            offset += sizeof(float);
            float rotX = BitConverter.ToSingle(data, offset);
            offset += sizeof(float);
            float rotY = BitConverter.ToSingle(data, offset);
            offset += sizeof(float);
            float rotZ = BitConverter.ToSingle(data, offset);
            offset += sizeof(float);
            float rotW = BitConverter.ToSingle(data, offset);

            Vector3 pos = new Vector3(posX, posY, posZ);
            Quaternion rot = new Quaternion(rotX, rotY, rotZ, rotW);

            if (sender != Client.MyID)
                avatars[sender].SetTransform(pos, rot);
        }
    }
}
