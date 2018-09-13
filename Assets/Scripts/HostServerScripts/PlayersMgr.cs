using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using GENUtility;
using SOPRO;

public class PlayersMgr : MonoBehaviour
{
    public SimpleAvatar[] ControllableAvatars;
    public SimpleAvatar[] Avatars;
    public Transform SpawnPosition;
    public Material[] Materials;

    public SOEvCSteamID PlayerDeath;

    static PlayersMgr instance;

    Dictionary<CSteamID, SimpleAvatar> avatars;

    public static Dictionary<CSteamID, SimpleAvatar> Players { get { return instance != null ? instance.avatars : null; } }

    public void Init()
    {
        avatars = new Dictionary<CSteamID, SimpleAvatar>();
        #if UNITY_EDITOR
        Debug.Log("Users count when init playerManager" + Client.Users.Count + ", using server list : " + (Client.Lobby.LobbyID.m_SteamID == 0));
        #endif

        for (int i = 0; i < Client.Users.Count; i++)
        {
            SimpleAvatar a;
            if (Client.Users[i].SteamID == Client.MyID)
                a = Instantiate(ControllableAvatars[Client.Users[i].AvatarID]);
            else
                a = Instantiate(Avatars[Client.Users[i].AvatarID]);

            a.UserInfo = Client.Users[i];
            a.transform.position = SpawnPosition.position;
            a.transform.forward = new Vector3(-Camera.main.transform.forward.x, 0, -Camera.main.transform.forward.z);
            avatars.Add(Client.Users[i].SteamID, a);
        }

        Client.AddCommand(PacketType.PlayerDataServer, ManageServerTransform);
        Client.AddCommand(PacketType.PlayerData, ManageTransform);
        Client.AddCommand(PacketType.PlayerStatus, ManagePlayerStatus);
        Client.AddCommand(PacketType.PlayerDeath, ManagePlayerDeath);

        Client.OnUserDisconnected += PlayerDisconnected;
    }

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
    }

    void ManagePlayerDeath(byte[] data, uint length, CSteamID sender)
    {
        CSteamID dead = (CSteamID)ByteManipulator.ReadUInt64(data, 0);
        SimpleAvatar avatar = avatars[dead];
        PlayerDeath.Raise(dead);
        avatar.gameObject.SetActive(false);
    }

    void ManagePlayerStatus(byte[] data, uint dataLength, CSteamID sender)
    {
        SetHealth(data);
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

        bool setDirectTransform = false;

        int overLength = (int)dataLength - 28;

        if (overLength > 0)
        {
            setDirectTransform = SetHealth(data);
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

        if (setDirectTransform)
            avatar.transform.SetPositionAndRotation(pos, rot);
        else
            avatar.SetTransform(pos, rot);
    }

    private bool SetHealth(byte[] data)
    {
        CSteamID target = (CSteamID)ByteManipulator.ReadUInt64(data, 0);

        if (avatars.ContainsKey(target) && avatars[target] != null)
        {
            Player player = avatars[target].GetComponent<Player>();

            player.SetLife(ByteManipulator.ReadSingle(data, 8));

            if (player.Life <= 0f)
            {
                if (!player.Dead)
                    player.Die();
                return true;
            }
            else if (player.Dead)
            {
                player.Resurrect(player.Life);
            }
        }

        return false;
    }

    void PlayerDisconnected(CSteamID id)
    {
        if (avatars == null)
            return;

        SimpleAvatar avatar = avatars[id];
        if (avatar == null)
            return;

        Destroy(avatar.gameObject);
        avatars.Remove(id);
    }

    void OnDestroy()
    {
        Client.OnUserDisconnected -= PlayerDisconnected;
        instance = null;
    }
}
