using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class Server : MonoBehaviour
{
    static Server Instance;
    public List<User> InGameUsers;

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
        Host = Client.Host;
        Client.AddCommand(PacketType.LatencyServer, LatencyResponse);
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
        Client.SendPacket(new byte[]{ }, PacketType.Latency, Client.MyID, id, EP2PSend.k_EP2PSendReliable);
    }
}
