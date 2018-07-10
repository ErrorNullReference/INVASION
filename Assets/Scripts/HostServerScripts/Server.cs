using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class Server : MonoBehaviour
{
    private static readonly byte[] emptyArray = new byte[0];
    static Server Instance;
    public List<User> InGameUsers;
    public User MyPlayer;

    public static User LocalPlayer { get { return Instance.MyPlayer; } }

    public static List<User> Users { get { return Instance.InGameUsers; } }

    public static bool Initialized;
    public static CSteamID Host;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
    }

    void InitServer()
    {
        InGameUsers = new List<User>(Client.Users);
        for (int i = 0; i < InGameUsers.Count; i++)
        {
            if (InGameUsers[i].SteamID == Client.MyID)
                MyPlayer = InGameUsers[i];
        }

        Host = Client.Host;
        Client.AddCommand(PacketType.LatencyServer, LatencyResponse);
        Client.OnUserDisconnected += RemoveUser;
        Client.OnGameEnd += ResetServerinstance;
    }

    void ResetServerinstance()
    {
        InGameUsers.Clear();
        Host = new CSteamID(0);
        Initialized = false;
    }

    public static void Init()
    {
        if (!Initialized)
        {
            Instance.InitServer();
            Initialized = true;
        }
    }

    public static void ResetServer()
    {
        Instance.ResetServerinstance();
    }

    void LatencyResponse(byte[] data, uint lenght, CSteamID id)
    {
        Client.SendPacket(emptyArray, 0, 0, PacketType.Latency, Client.MyID, id, EP2PSend.k_EP2PSendReliable);
    }

    void RemoveUser(CSteamID id)
    {
        if (InGameUsers == null)
            return;

        for (int i = 0; i < InGameUsers.Count; i++)
        {
            if (InGameUsers[i].SteamID == id)
            {
                InGameUsers.RemoveAt(i);
            }
        }
    }

    void OnDestroy()
    {
        Client.OnUserDisconnected -= RemoveUser;
        Client.OnGameEnd -= ResetServerinstance;
    }
}
