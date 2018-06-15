using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;
using GENUtility;
using SOPRO;
public enum PacketType : byte
{
    Test = 0,
    PlayerData,
    PlayerDataServer,
    PlayerStatus,
    NetObjTransform,
    NetObjTransformServer,
    Chat,
    Serverchat,
    VoiceChatData,
    VoiceChatMutedMessage,
    EnemyDeath,
    EnemySpawn,
    RequestAvatarSelection,
    AnswerAvatarSelection,
    LeaveLobby,
    LatencyServer,
    Latency,
    Shoot,
    ShootServer,
    ShootHit,
    ShootHitServer,
    ShootCall
}

public enum PacketOffset
{
    Command = 0,
    ID = 1,
    Payload = 9
}

public class Client : MonoBehaviour
{
    public static int MaxNumOfPlayers;

    public static Lobby Lobby;

    //public delegate void ClientStatus();

    //public delegate void UserStatus(CSteamID ID);

    public BaseSOEvVoid OnLobbyInitializationEvent;
    public BaseSOEvVoid OnLobbyLeaveEvent;
    public BaseSOEvVoid OnClientInitialized;
    public BaseSOEvCSteamID OnUserEnter;
    public BaseSOEvCSteamID OnUserLeave;

    //public static event ClientStatus OnLobbyInitializationEvent;
    //public static event ClientStatus OnLobbyLeaveEvent;
    //public static event UserStatus OnUserEnter;
    //public static event UserStatus OnUserLeave;

    /// <summary>
    /// Return true if you are the lobby owner.
    /// </summary>
    public static bool IsHost { get { return Host == MyID ? true : false; } }

    public static CSteamID MyID;

    public static CSteamID Host;

    private static readonly byte[] emptyArray = new byte[0];

    /// <summary>
    /// Returns the users in the lobby while in lobby or the users in game while in game. Can return null.
    /// </summary>
    public static List<User> Users
    {
        get
        {
            if (instance.lobby.LobbyID != (CSteamID)0)
                return instance.lobby.Users;
            else if (Server.Host != (CSteamID)0)
                return Server.Users;
            return null;
        }
    }

    public delegate void Command(byte[] data, uint dataLength, CSteamID sender);

    public static Command[] Commands;

    public const int HeaderLength = (int)PacketOffset.Payload;

    [SerializeField]
    private int NumOfPlayers;
    [SerializeField]
    private Lobby lobby;

    BytePacket Packet;

    static Client instance;

    public static float Latency;
    float latencyTimer;
    WaitForSeconds waitForSeconds;

    // Use this for initialization
    void Start()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }

        //DontDestroyOnLoad(this.gameObject);

        SteamCallbackReceiver.Init();

        Lobby = lobby;
        MaxNumOfPlayers = NumOfPlayers;

        SteamCallbackReceiver.LobbyEnterEvent += EnterLobby;
        SteamCallbackReceiver.LobbyCreateEvent += CreateLobby;
        SteamCallbackReceiver.ChatUpdateEvent += UpdateUsers;
        SteamCallbackReceiver.AcceptP2PEvent += AcceptP2P;
        SteamCallbackReceiver.P2PFailEvent += P2PStatus;

        MyID = SteamUser.GetSteamID();

        Packet = new BytePacket(4096);

        Commands = new Command[Enum.GetNames(typeof(PacketType)).Length];
        AddCommands(PacketType.LeaveLobby, LeaveLobbyCommand);
        AddCommands(PacketType.Latency, LatencyResponse);

        StartCoroutine(LatencyTest());

        waitForSeconds = new WaitForSeconds(0.1f);

        if (OnClientInitialized)
            OnClientInitialized.Raise();
    }

    IEnumerator LatencyTest()
    {
        while (true)
        {
            Client.SendPacketToHost(emptyArray, 0, 0, PacketType.LatencyServer, EP2PSend.k_EP2PSendReliable);
            latencyTimer = Time.time;
            yield return waitForSeconds;
        }
    }

    void LatencyResponse(byte[] data, uint lenght, CSteamID id)
    {
        Latency = Time.time - latencyTimer;
    }

    void AcceptP2P(P2PSessionRequest_t cb)
    {
        SteamNetworking.AcceptP2PSessionWithUser((CSteamID)cb.m_steamIDRemote);
    }

    void P2PStatus(P2PSessionConnectFail_t cb)
    {
        #if UNITY_EDITOR
                Debug.Log(cb.m_eP2PSessionError);
        #endif
    }

    void AddCommands(PacketType commandType, Command command)
    {
        Commands[(int)commandType] = command;
    }

    /// <summary>
    /// Link a PacketType to a specific Command. The Command will be called if the appropriate packet is received.
    /// </summary>
    /// <typeparam name="T">type</typeparam>
    /// <param name="command">The method to link to the PacketType</param>
    public static void AddCommand(PacketType commandType, Command command)
    {
        instance.AddCommands(commandType, command);
    }

    void Update()
    {
        uint dataLenght;
        while (SteamNetworking.IsP2PPacketAvailable(out dataLenght))
            Receive(dataLenght);

        SetOwner();
    }

    void SetOwner()
    {
        if (lobby.LobbyID != (CSteamID)0)
        {
            CSteamID newOwner = SteamMatchmaking.GetLobbyOwner(lobby.LobbyID);
            if (newOwner != lobby.Owner)
            {
                Host = newOwner;
                lobby.SetOwner(newOwner);
            }
        }
    }

    void InitLobby(CSteamID id)
    {
        lobby.LobbyID = id;
        if (Users == null)
            lobby.Users = new List<User>();
        else
            Users.Clear();

        int num = SteamMatchmaking.GetNumLobbyMembers(lobby.LobbyID);
        for (int i = 0; i < num; i++)
        {
            CSteamID userID = SteamMatchmaking.GetLobbyMemberByIndex(lobby.LobbyID, i);
            lobby.Users.Add(new User(userID));
        }

        SendPacketToLobby(emptyArray, 0, 0, PacketType.Test, EP2PSend.k_EP2PSendReliable, false);


        if (OnLobbyInitializationEvent)
            OnLobbyInitializationEvent.Raise();
    }

    void EnterLobby(LobbyEnter_t cb)
    {
        InitLobby((CSteamID)cb.m_ulSteamIDLobby);
    }

    void CreateLobby(LobbyCreated_t cb)
    {
        InitLobby((CSteamID)cb.m_ulSteamIDLobby);
    }

    void OnDestroy()
    {

        SteamCallbackReceiver.LobbyEnterEvent -= EnterLobby;
        SteamCallbackReceiver.ChatUpdateEvent -= UpdateUsers;
    }

    /// <summary>
    /// Leaves the lobby.
    /// </summary>
    public void LeaveLobby()
    {
        CSteamID lobbyID = lobby.LobbyID;
        lobby.Reset();
        if (OnLobbyLeaveEvent)
            OnLobbyLeaveEvent.Raise();
    }

    void LeaveLobbyCommand(byte[] data, uint dataLenght, CSteamID sender)
    {
        LeaveLobby();
    }

    void ResetClientInstance()
    {
        Host = new CSteamID(0);
        ClearP2PSession();
    }

    /// <summary>
    /// Reset the client. Should be called only at the end of a game.
    /// </summary>
    public static void ResetClient()
    {
        instance.ResetClientInstance();
        Server.ResetServer();
    }

    public void ClearP2PSession()
    {
        for (int i = 0; i < Users.Count; i++)
            SteamNetworking.CloseP2PSessionWithUser(Users[i].SteamID);
    }

    /// <summary>
    /// Leaves the lobby.
    /// </summary>
    public static void LeaveCurrentLobby()
    {
        instance.LeaveLobby();
    }

    void UpdateUsers(LobbyChatUpdate_t cb)
    {
        if ((EChatMemberStateChange)cb.m_rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeEntered)
        {
            if (lobby.GetUserFromID((CSteamID)cb.m_ulSteamIDUserChanged) == null)
            {
                Users.Add(new User((CSteamID)cb.m_ulSteamIDUserChanged));
                if (OnUserEnter)
                    OnUserEnter.Raise((CSteamID)cb.m_ulSteamIDUserChanged);
            }
        }
        else if ((EChatMemberStateChange)cb.m_rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeLeft || (EChatMemberStateChange)cb.m_rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeDisconnected)
        {
            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i].SteamID == (CSteamID)cb.m_ulSteamIDUserChanged)
                {
                    Users.Remove(Users[i]);
                    if (OnUserLeave)
                        OnUserLeave.Raise((CSteamID)cb.m_ulSteamIDUserChanged);
                    return;
                }
            }
        }
    }

    void InvokeCommand(int command, byte[] data, uint dataLength, CSteamID sender)
    {
        if (command >= Commands.Length)
            return;
        if (Commands[command] != null)
            Commands[command].Invoke(data, dataLength, sender);
    }

    void Receive(uint lenght)
    {
        byte[] receiver = ArrayPool<byte>.Get((int)lenght);

        uint dataLenght;
        CSteamID sender;
        SteamNetworking.ReadP2PPacket(receiver, lenght, out dataLenght, out sender);

        int command = receiver[0];
        CSteamID packetSender = (CSteamID)ByteManipulator.ReadUInt64(receiver, 1);

        int finalLength = (int)lenght - HeaderLength;

        ByteManipulator.Write<byte>(receiver, HeaderLength, receiver, 0, finalLength);

        InvokeCommand(command, receiver, (uint)finalLength, packetSender);

        ArrayPool<byte>.Recycle(receiver);
    }

    void Send(byte[] data, int startIndex, int length, PacketType command, CSteamID sender, CSteamID receiver, EP2PSend sendType)
    {
        if (MyID == receiver)
        {
            InvokeCommand((int)command, data, (uint)length, sender);
            return;
        }

        byte[] toSend = ArrayPool<byte>.Get(length + HeaderLength);

        ByteManipulator.Write(toSend, 0, (byte)command);
        ByteManipulator.Write(toSend, sizeof(byte), (ulong)sender);
        ByteManipulator.Write<byte>(data, startIndex, toSend, HeaderLength, length);

        SteamNetworking.SendP2PPacket(receiver, toSend, (uint)toSend.Length, sendType);

        ArrayPool<byte>.Recycle(toSend);
    }

    void SendAllLobby(byte[] data, int startIndex, int length, PacketType command, CSteamID sender, EP2PSend sendType, bool sendToSender = true)
    {
        if (Users == null)
            return;

        for (int i = 0; i < Users.Count; i++)
        {
            User user = Users[i];
            if (!sendToSender && user.SteamID == MyID)
                continue;
            Send(data, startIndex, length, command, sender, user.SteamID, sendType);
        }
    }

    void SendAllInGameUsers(byte[] data, int startIndex, int length, PacketType command, CSteamID sender, EP2PSend sendType, bool sendToSender = true)
    {
        if (Users == null)
            return;

        for (int i = 0; i < Users.Count; i++)
        {
            User user = Users[i];
            if (!sendToSender && user.SteamID == MyID)
                continue;
            Send(data, startIndex, length, command, sender, user.SteamID, sendType);
        }
    }

    void SendToHost(byte[] data, int startIndex, int length, PacketType command, CSteamID sender, EP2PSend sendType)
    {
        if (lobby.Owner != (CSteamID)0)
            Send(data, startIndex, length, command, sender, lobby.Owner, sendType);
        else if (Server.Host != (CSteamID)0)
            Send(data, startIndex, length, command, sender, Server.Host, sendType);
    }

    /// <summary>
    /// Send the given packet to another steam user using steam's protocol.
    /// </summary>
    /// <typeparam name="T">type</typeparam>
    /// <param name="data">Payload of the packet</param>
    /// <param name="command">The command representing the data packet</param>
    /// <param name="sender">CSteamID of the user sending the data</param>
    /// <param name="receiver">CSteamID of the user that will receive the data</param>
    /// <param name="sendType">EP2PSend type of the packet</param>
    public static void SendPacket(byte[] data, int startIndex, int length, PacketType command, CSteamID sender, CSteamID receiver, EP2PSend sendType)
    {
        instance.Send(data, startIndex, length, command, sender, receiver, sendType);
    }

    /// <summary>
    /// Send the given packet to all users in the lobby.
    /// </summary>
    /// <typeparam name="T">type</typeparam>
    /// <param name="data">Payload of the packet</param>
    /// <param name="command">The command representing the data packet</param>
    /// <param name="sender">CSteamID of the user sending the data</param>
    /// <param name="sendType">EP2PSend type of the packet</param>
    /// <param name="sendToSender">True if the sender have to receive the packet, false otherwise</param>
    public static void SendPacketToLobby(byte[] data, int startIndex, int length, PacketType command, CSteamID sender, EP2PSend sendType, bool sendToSender = true)
    {
        instance.SendAllLobby(data, startIndex, length, command, sender, sendType, sendToSender);
    }

    /// <summary>
    /// Send the given packet to all users in game.
    /// </summary>
    /// <typeparam name="T">type</typeparam>
    /// <param name="data">Payload of the packet</param>
    /// <param name="command">The command representing the data packet</param>
    /// <param name="sender">CSteamID of the user sending the data</param>
    /// <param name="sendType">EP2PSend type of the packet</param>
    /// <param name="sendToSender">True if the sender have to receive the packet, false otherwise</param>
    public static void SendPacketToInGameUsers(byte[] data, int startIndex, int length, PacketType command, CSteamID sender, EP2PSend sendType, bool sendToSender = true)
    {
        instance.SendAllInGameUsers(data, startIndex, length, command, sender, sendType, sendToSender);
    }

    /// <summary>
    /// Send the given packet to all users in the lobby.
    /// </summary>
    /// <typeparam name="T">type</typeparam>
    /// <param name="data">Payload of the packet</param>
    /// <param name="command">The command representing the data packet</param>
    /// <param name="sendType">EP2PSend type of the packet</param>
    /// <param name="sendToSender">True if the sender have to receive the packet, false otherwise</param>
    public static void SendPacketToLobby(byte[] data, int startIndex, int length, PacketType command, EP2PSend sendType, bool sendToSender = true)
    {
        instance.SendAllLobby(data, startIndex, length, command, Client.MyID, sendType, sendToSender);
    }

    /// <summary>
    /// Send the given packet to all users in game.
    /// </summary>
    /// <typeparam name="T">type</typeparam>
    /// <param name="data">Payload of the packet</param>
    /// <param name="command">The command representing the data packet</param>
    /// <param name="sendType">EP2PSend type of the packet</param>
    /// <param name="sendToSender">True if the sender have to receive the packet, false otherwise</param>
    public static void SendPacketToInGameUsers(byte[] data, int startIndex, int length, PacketType command, EP2PSend sendType, bool sendToSender = true)
    {
        instance.SendAllInGameUsers(data, startIndex, length, command, Client.MyID, sendType, sendToSender);
    }

    /// <summary>
    /// Send the given packet to the lobby owner.
    /// </summary>
    /// <typeparam name="T">type</typeparam>
    /// <param name="data">Payload of the packet</param>
    /// <param name="command">The command representing the data packet</param>
    /// <param name="sender">CSteamID of the user sending the data</param>
    /// <param name="sendType">EP2PSend type of the packet</param>
    public static void SendPacketToHost(byte[] data, int startIndex, int length, PacketType command, CSteamID sender, EP2PSend sendType)
    {
        instance.SendToHost(data, startIndex, length, command, sender, sendType);
    }

    /// <summary>
    /// Send the given packet to the lobby owner.
    /// </summary>
    /// <typeparam name="T">type</typeparam>
    /// <param name="data">Payload of the packet</param>
    /// <param name="command">The command representing the data packet</param>
    /// <param name="sendType">EP2PSend type of the packet</param>
    public static void SendPacketToHost(byte[] data, int startIndex, int length, PacketType command, EP2PSend sendType)
    {
        instance.SendToHost(data, startIndex, length, command, MyID, sendType);
    }

    /// <summary>
    /// Writes the rotation on packet data.
    /// </summary>
    /// <param name="transform">Tranform to write</param>
    /// <param name="resetPacketSeek">If set to <c>true</c> reset packet seek.</param>
    public void WriteTransformOnPacketData(Transform transform, bool resetPacketSeek)
    {
        if (resetPacketSeek)
        {
            Packet.CurrentLength = 0;
            Packet.CurrentSeek = 0;
        }

        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        Packet.Write(pos.x);
        Packet.Write(pos.y);
        Packet.Write(pos.z);

        Packet.Write(rot.x);
        Packet.Write(rot.y);
        Packet.Write(rot.z);
        Packet.Write(rot.w);
    }
    //TODO: most of these methods are redundant. Why are there so many?
    /// <summary>
    /// Send the transform given to the host.
    /// </summary>
    public static void SendTransformToHost(Transform transform, EP2PSend sendType)
    {
        instance.WriteTransformOnPacketData(transform, true);

        Client.SendPacketToHost(instance.Packet.Data, 0, instance.Packet.CurrentLength, PacketType.PlayerDataServer, sendType);
    }

    /// <summary>
    /// Send the transform given to the host.
    /// </summary>
    /// /// <param name="data">buffer to include in the final packet in addition to the transform.</param>
    public static void SendTransformToHost(byte[] data, Transform transform, EP2PSend sendType)
    {
        instance.Packet.CurrentSeek = 0;
        instance.Packet.CurrentLength = 0;

        instance.Packet.WriteByteData(data, 0, data.Length);

        instance.WriteTransformOnPacketData(transform, false);

        Client.SendPacketToHost(instance.Packet.Data, 0, instance.Packet.CurrentLength, PacketType.PlayerDataServer, sendType);
    }

    /// <summary>
    /// Send the transform given to the host.
    /// </summary>
    public static void SendTransformToHost(Transform transform, PacketType command, EP2PSend sendType)
    {
        instance.WriteTransformOnPacketData(transform, true);

        Client.SendPacketToHost(instance.Packet.Data, 0, instance.Packet.CurrentLength, command, sendType);
    }

    /// <summary>
    /// Send the transform given to the host.
    /// </summary>
    /// /// <param name="data">buffer to include in the final packet in addition to the transform.</param>
    public static void SendTransformToHost(byte[] data, Transform transform, PacketType command, EP2PSend sendType)
    {
        instance.Packet.CurrentSeek = 0;
        instance.Packet.CurrentLength = 0;

        instance.Packet.WriteByteData(data, 0, data.Length);

        instance.WriteTransformOnPacketData(transform, false);

        Client.SendPacketToHost(instance.Packet.Data, 0, instance.Packet.CurrentLength, command, sendType);
    }

    /// <summary>
    /// Send the transform given to the lobby.
    /// </summary>
    public static void SendTransformToLobby(Transform transform, EP2PSend sendType, bool sendToSender = true)
    {
        instance.WriteTransformOnPacketData(transform, true);

        Client.SendPacketToLobby(instance.Packet.Data, 0, instance.Packet.CurrentLength, PacketType.PlayerDataServer, sendType, sendToSender);
    }

    /// <summary>
    /// Send the transform given to the lobby.
    /// </summary>
    /// /// <param name="data">buffer to include in the final packet in addition to the transform. Data will be written before the transform.</param>
    public static void SendTransformToLobby(byte[] data, Transform transform, EP2PSend sendType, bool sendToSender = true)
    {
        instance.Packet.CurrentSeek = 0;
        instance.Packet.CurrentLength = 0;

        instance.Packet.WriteByteData(data, 0, data.Length);

        instance.WriteTransformOnPacketData(transform, false);

        Client.SendPacketToLobby(instance.Packet.Data, 0, instance.Packet.CurrentLength, PacketType.PlayerDataServer, sendType, sendToSender);
    }

    /// <summary>
    /// Send the transform given to the lobby.
    /// </summary>
    public static void SendTransformToLobby(Transform transform, PacketType command, EP2PSend sendType, bool sendToSender = true)
    {
        instance.WriteTransformOnPacketData(transform, true);

        Client.SendPacketToLobby(instance.Packet.Data, 0, instance.Packet.CurrentLength, command, sendType, sendToSender);
    }

    /// <summary>
    /// Send the transform given to the lobby.
    /// </summary>
    /// /// <param name="data">buffer to include in the final packet in addition to the transform. Data will be written before the transform.</param>
    public static void SendTransformToLobby(byte[] data, Transform transform, PacketType command, EP2PSend sendType, bool sendToSender = true)
    {
        instance.Packet.CurrentSeek = 0;
        instance.Packet.CurrentLength = 0;

        instance.Packet.WriteByteData(data, 0, data.Length);

        instance.WriteTransformOnPacketData(transform, false);

        Client.SendPacketToLobby(instance.Packet.Data, 0, instance.Packet.CurrentLength, command, sendType, sendToSender);
    }

    /// <summary>
    /// Send the transform given to game.
    /// </summary>
    public static void SendTransformToInGameUsers(Transform transform, EP2PSend sendType, bool sendToSender = true)
    {
        instance.WriteTransformOnPacketData(transform, true);

        Client.SendPacketToInGameUsers(instance.Packet.Data, 0, instance.Packet.CurrentLength, PacketType.PlayerDataServer, sendType, sendToSender);
    }

    /// <summary>
    /// Send the transform given to game.
    /// </summary>
    /// /// <param name="data">buffer to include in the final packet in addition to the transform. Data will be written before the transform.</param>
    public static void SendTransformToInGameUsers(byte[] data, Transform transform, EP2PSend sendType, bool sendToSender = true)
    {
        instance.Packet.CurrentSeek = 0;
        instance.Packet.CurrentLength = 0;

        instance.Packet.WriteByteData(data, 0, data.Length);

        instance.WriteTransformOnPacketData(transform, false);

        Client.SendPacketToInGameUsers(instance.Packet.Data, 0, instance.Packet.CurrentLength, PacketType.PlayerDataServer, sendType, sendToSender);
    }

    /// <summary>
    /// Send the transform given to game.
    /// </summary>
    public static void SendTransformToInGameUsers(Transform transform, PacketType command, EP2PSend sendType, bool sendToSender = true)
    {
        instance.WriteTransformOnPacketData(transform, true);

        Client.SendPacketToInGameUsers(instance.Packet.Data, 0, instance.Packet.CurrentLength, command, sendType, sendToSender);
    }

    /// <summary>
    /// Send the transform given to the lobby.
    /// </summary>
    /// /// <param name="data">buffer to include in the final packet in addition to the transform. Data will be written before the transform.</param>
    public static void SendTransformToInGameUsers(byte[] data, Transform transform, PacketType command, EP2PSend sendType, bool sendToSender = true)
    {
        instance.Packet.CurrentSeek = 0;
        instance.Packet.CurrentLength = 0;

        instance.Packet.WriteByteData(data, 0, data.Length);

        instance.WriteTransformOnPacketData(transform, false);

        Client.SendPacketToInGameUsers(instance.Packet.Data, 0, instance.Packet.CurrentLength, command, sendType, sendToSender);
    }

    void OnGUI()
    {
        GUILayout.Label("Latency " + Latency.ToString());
    }
}