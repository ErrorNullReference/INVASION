using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;
using GENUtility;

public enum PacketType
{
    Test,
    Transform,
    ServerTransform,
    EnemyTransform,
    ServerEnemyTransform,
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

    public delegate void ClientStatus();

    public delegate void UserStatus(CSteamID ID);

    public static event ClientStatus OnLobbyInitializationEvent;
    public static event ClientStatus OnLobbyLeaveEvent;
    public static event UserStatus OnUserEnter;
    public static event UserStatus OnUserLeave;

    /// <summary>
    /// Return true if you are the lobby owner.
    /// </summary>
    public static bool IsHost { get { return Host == MyID ? true : false; } }

    public static CSteamID MyID;

    public static CSteamID Host;

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
    }

    IEnumerator LatencyTest()
    {
        while (true)
        {
            Client.SendPacketToHost(new byte[] { }, PacketType.LatencyServer, EP2PSend.k_EP2PSendReliable);
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
        SendPacketToLobby(new byte[] { }, PacketType.Test, EP2PSend.k_EP2PSendReliable, false);

        if (OnLobbyInitializationEvent != null)
            OnLobbyInitializationEvent.Invoke();
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
        if (OnLobbyLeaveEvent != null)
            OnLobbyLeaveEvent.Invoke();
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
                if (OnUserEnter != null)
                    OnUserEnter.Invoke((CSteamID)cb.m_ulSteamIDUserChanged);
            }
        }
        else if ((EChatMemberStateChange)cb.m_rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeLeft || (EChatMemberStateChange)cb.m_rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeDisconnected)
        {
            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i].SteamID == (CSteamID)cb.m_ulSteamIDUserChanged)
                {
                    Users.Remove(Users[i]);
                    if (OnUserLeave != null)
                        OnUserLeave.Invoke((CSteamID)cb.m_ulSteamIDUserChanged);
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
        BytePacket receiver = new BytePacket(ArrayPool<byte>.Get((int)lenght));
        //prepare packet to receive data by setting seek and lenght to 0
        receiver.ResetSeekLength();

        uint dataLenght;
        CSteamID sender;
        SteamNetworking.ReadP2PPacket(receiver.Data, lenght, out dataLenght, out sender);

        int command = receiver.ReadByte();
        CSteamID packetSender = (CSteamID)receiver.ReadULong();

        //byte[] data = new byte[receiver.CurrentLength - (int)PacketOffset.Payload];
        //ByteManipulator.Write(receiver.Data, (int)PacketOffset.Payload, data, 0, data.Length);

        receiver.CurrentLength = (int)lenght - receiver.CurrentSeek;

        receiver.WriteByteData(receiver.Data, 0, receiver.CurrentSeek, receiver.CurrentLength);

        InvokeCommand(command, receiver.Data, (uint)receiver.CurrentLength, packetSender);

        ArrayPool<byte>.Recycle(receiver.Data);
    }

    void Send(byte[] data, PacketType command, CSteamID sender, CSteamID receiver, EP2PSend sendType)
    {
        BytePacket toSend = new BytePacket((ArrayPool<byte>.Get(data.Length + HeaderLength)));
        //prepare packet to send data by setting seek and lenght to 0
        toSend.ResetSeekLength();

        toSend.Write((byte)command);
        toSend.Write((ulong)sender);
        toSend.WriteByteData(data, 0, data.Length);

        if (MyID != receiver)
            SteamNetworking.SendP2PPacket(receiver, toSend.Data, (uint)toSend.CurrentLength, sendType);
        else
            InvokeCommand((int)command, data, (uint)data.Length, sender);

        ArrayPool<byte>.Recycle(toSend.Data);
    }

    void SendAllLobby(byte[] data, PacketType command, CSteamID sender, EP2PSend sendType, bool sendToSender = true)
    {
        if (Users == null)
            return;

        for (int i = 0; i < Users.Count; i++)
        {
            if (!sendToSender && Users[i].SteamID == MyID)
                continue;
            Send(data, command, sender, Users[i].SteamID, sendType);
        }
    }

    void SendAllInGameUsers(byte[] data, PacketType command, CSteamID sender, EP2PSend sendType, bool sendToSender = true)
    {
        if (Users == null)
            return;

        for (int i = 0; i < Users.Count; i++)
        {
            if (!sendToSender && Users[i].SteamID == MyID)
                continue;
            Send(data, command, sender, Users[i].SteamID, sendType);
        }
    }

    void SendToHost(byte[] data, PacketType command, CSteamID sender, EP2PSend sendType)
    {
        if (lobby.Owner != (CSteamID)0)
            Send(data, command, sender, lobby.Owner, sendType);
        else if (Server.Host != (CSteamID)0)
            Send(data, command, sender, Server.Host, sendType);
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
    public static void SendPacket(byte[] data, PacketType command, CSteamID sender, CSteamID receiver, EP2PSend sendType)
    {
        instance.Send(data, command, sender, receiver, sendType);
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
    public static void SendPacketToLobby(byte[] data, PacketType command, CSteamID sender, EP2PSend sendType, bool sendToSender = true)
    {
        instance.SendAllLobby(data, command, sender, sendType, sendToSender);
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
    public static void SendPacketToInGameUsers(byte[] data, PacketType command, CSteamID sender, EP2PSend sendType, bool sendToSender = true)
    {
        instance.SendAllInGameUsers(data, command, sender, sendType, sendToSender);
    }

    /// <summary>
    /// Send the given packet to all users in the lobby.
    /// </summary>
    /// <typeparam name="T">type</typeparam>
    /// <param name="data">Payload of the packet</param>
    /// <param name="command">The command representing the data packet</param>
    /// <param name="sendType">EP2PSend type of the packet</param>
    /// <param name="sendToSender">True if the sender have to receive the packet, false otherwise</param>
    public static void SendPacketToLobby(byte[] data, PacketType command, EP2PSend sendType, bool sendToSender = true)
    {
        instance.SendAllLobby(data, command, Client.MyID, sendType, sendToSender);
    }

    /// <summary>
    /// Send the given packet to all users in game.
    /// </summary>
    /// <typeparam name="T">type</typeparam>
    /// <param name="data">Payload of the packet</param>
    /// <param name="command">The command representing the data packet</param>
    /// <param name="sendType">EP2PSend type of the packet</param>
    /// <param name="sendToSender">True if the sender have to receive the packet, false otherwise</param>
    public static void SendPacketToInGameUsers(byte[] data, PacketType command, EP2PSend sendType, bool sendToSender = true)
    {
        instance.SendAllInGameUsers(data, command, Client.MyID, sendType, sendToSender);
    }

    /// <summary>
    /// Send the given packet to the lobby owner.
    /// </summary>
    /// <typeparam name="T">type</typeparam>
    /// <param name="data">Payload of the packet</param>
    /// <param name="command">The command representing the data packet</param>
    /// <param name="sender">CSteamID of the user sending the data</param>
    /// <param name="sendType">EP2PSend type of the packet</param>
    public static void SendPacketToHost(byte[] data, PacketType command, CSteamID sender, EP2PSend sendType)
    {
        instance.SendToHost(data, command, sender, sendType);
    }

    /// <summary>
    /// Send the given packet to the lobby owner.
    /// </summary>
    /// <typeparam name="T">type</typeparam>
    /// <param name="data">Payload of the packet</param>
    /// <param name="command">The command representing the data packet</param>
    /// <param name="sendType">EP2PSend type of the packet</param>
    public static void SendPacketToHost(byte[] data, PacketType command, EP2PSend sendType)
    {
        instance.SendToHost(data, command, MyID, sendType);
    }

    /// <summary>
    /// Writes the rotation on packet data.
    /// </summary>
    /// <param name="rotation">Rotation.</param>
    /// <param name="resetPacketSeek">If set to <c>true</c> reset packet seek.</param>
    public void WriteTransformOnPacketData(Transform transform, bool resetPacketSeek)
    {
        if (resetPacketSeek)
            Packet.CurrentSeek = 0;

        Packet.Write(transform.position.x);
        Packet.Write(transform.position.y);
        Packet.Write(transform.position.z);

        Packet.Write(transform.rotation.x);
        Packet.Write(transform.rotation.y);
        Packet.Write(transform.rotation.z);
        Packet.Write(transform.rotation.w);
    }

    /// <summary>
    /// Send the transform given to the host.
    /// </summary>
    public static void SendTransformToHost(Transform transform, EP2PSend sendType)
    {
        instance.WriteTransformOnPacketData(transform, true);

        byte[] packet = ArrayPool<byte>.Get(instance.Packet.CurrentSeek);

        Array.Copy(instance.Packet.Data, 0, packet, 0, packet.Length);
        Client.SendPacketToHost(packet, PacketType.ServerTransform, sendType);

        ArrayPool<byte>.Recycle(packet);
    }

    /// <summary>
    /// Send the transform given to the host.
    /// </summary>
    /// /// <param name="data">buffer to include in the final packet in addition to the transform.</param>
    public static void SendTransformToHost(byte[] data, Transform transform, EP2PSend sendType)
    {
        instance.Packet.CurrentSeek = 0;

        instance.Packet.WriteByteData(data, 0, data.Length);

        instance.WriteTransformOnPacketData(transform, false);

        byte[] packet = ArrayPool<byte>.Get(instance.Packet.CurrentSeek);

        Array.Copy(instance.Packet.Data, 0, packet, 0, packet.Length);
        Client.SendPacketToHost(packet, PacketType.ServerTransform, sendType);

        ArrayPool<byte>.Recycle(packet);
    }

    /// <summary>
    /// Send the transform given to the host.
    /// </summary>
    public static void SendTransformToHost(Transform transform, PacketType command, EP2PSend sendType)
    {
        instance.WriteTransformOnPacketData(transform, true);

        byte[] packet = ArrayPool<byte>.Get(instance.Packet.CurrentSeek);

        Array.Copy(instance.Packet.Data, 0, packet, 0, packet.Length);
        Client.SendPacketToHost(packet, command, sendType);

        ArrayPool<byte>.Recycle(packet);
    }

    /// <summary>
    /// Send the transform given to the host.
    /// </summary>
    /// /// <param name="data">buffer to include in the final packet in addition to the transform.</param>
    public static void SendTransformToHost(byte[] data, Transform transform, PacketType command, EP2PSend sendType)
    {
        instance.Packet.CurrentSeek = 0;

        instance.Packet.WriteByteData(data, 0, data.Length);

        instance.WriteTransformOnPacketData(transform, false);

        byte[] packet = ArrayPool<byte>.Get(instance.Packet.CurrentSeek);

        Array.Copy(instance.Packet.Data, 0, packet, 0, packet.Length);
        Client.SendPacketToHost(packet, command, sendType);

        ArrayPool<byte>.Recycle(packet);
    }

    /// <summary>
    /// Send the transform given to the lobby.
    /// </summary>
    public static void SendTransformToLobby(Transform transform, EP2PSend sendType, bool sendToSender = true)
    {
        instance.WriteTransformOnPacketData(transform, true);

        byte[] packet = ArrayPool<byte>.Get(instance.Packet.CurrentSeek);

        Array.Copy(instance.Packet.Data, 0, packet, 0, packet.Length);
        Client.SendPacketToLobby(packet, PacketType.ServerTransform, sendType, sendToSender);

        ArrayPool<byte>.Recycle(packet);
    }

    /// <summary>
    /// Send the transform given to the lobby.
    /// </summary>
    /// /// <param name="data">buffer to include in the final packet in addition to the transform. Data will be written before the transform.</param>
    public static void SendTransformToLobby(byte[] data, Transform transform, EP2PSend sendType, bool sendToSender = true)
    {
        instance.Packet.CurrentSeek = 0;

        instance.Packet.WriteByteData(data, 0, data.Length);

        instance.WriteTransformOnPacketData(transform, false);

        byte[] packet = ArrayPool<byte>.Get(instance.Packet.CurrentSeek);

        Array.Copy(instance.Packet.Data, 0, packet, 0, packet.Length);
        Client.SendPacketToLobby(packet, PacketType.ServerTransform, sendType, sendToSender);

        ArrayPool<byte>.Recycle(packet);
    }

    /// <summary>
    /// Send the transform given to the lobby.
    /// </summary>
    public static void SendTransformToLobby(Transform transform, PacketType command, EP2PSend sendType, bool sendToSender = true)
    {
        instance.WriteTransformOnPacketData(transform, true);

        byte[] packet = ArrayPool<byte>.Get(instance.Packet.CurrentSeek);

        Array.Copy(instance.Packet.Data, 0, packet, 0, packet.Length);
        Client.SendPacketToLobby(packet, command, sendType, sendToSender);

        ArrayPool<byte>.Recycle(packet);
    }

    /// <summary>
    /// Send the transform given to the lobby.
    /// </summary>
    /// /// <param name="data">buffer to include in the final packet in addition to the transform. Data will be written before the transform.</param>
    public static void SendTransformToLobby(byte[] data, Transform transform, PacketType command, EP2PSend sendType, bool sendToSender = true)
    {
        instance.Packet.CurrentSeek = 0;

        instance.Packet.WriteByteData(data, 0, data.Length);

        instance.WriteTransformOnPacketData(transform, false);

        byte[] packet = ArrayPool<byte>.Get(instance.Packet.CurrentSeek);

        Array.Copy(instance.Packet.Data, 0, packet, 0, packet.Length);
        Client.SendPacketToLobby(packet, command, sendType, sendToSender);

        ArrayPool<byte>.Recycle(packet);
    }

    /// <summary>
    /// Send the transform given to game.
    /// </summary>
    public static void SendTransformToInGameUsers(Transform transform, EP2PSend sendType, bool sendToSender = true)
    {
        instance.WriteTransformOnPacketData(transform, true);

        byte[] packet = ArrayPool<byte>.Get(instance.Packet.CurrentSeek);

        Array.Copy(instance.Packet.Data, 0, packet, 0, packet.Length);
        Client.SendPacketToInGameUsers(packet, PacketType.ServerTransform, sendType, sendToSender);

        ArrayPool<byte>.Recycle(packet);
    }

    /// <summary>
    /// Send the transform given to game.
    /// </summary>
    /// /// <param name="data">buffer to include in the final packet in addition to the transform. Data will be written before the transform.</param>
    public static void SendTransformToInGameUsers(byte[] data, Transform transform, EP2PSend sendType, bool sendToSender = true)
    {
        instance.Packet.CurrentSeek = 0;

        instance.Packet.WriteByteData(data, 0, data.Length);

        instance.WriteTransformOnPacketData(transform, false);

        byte[] packet = ArrayPool<byte>.Get(instance.Packet.CurrentSeek);

        Array.Copy(instance.Packet.Data, 0, packet, 0, packet.Length);
        Client.SendPacketToInGameUsers(packet, PacketType.ServerTransform, sendType, sendToSender);

        ArrayPool<byte>.Recycle(packet);
    }

    /// <summary>
    /// Send the transform given to game.
    /// </summary>
    public static void SendTransformToInGameUsers(Transform transform, PacketType command, EP2PSend sendType, bool sendToSender = true)
    {
        instance.WriteTransformOnPacketData(transform, true);

        byte[] packet = ArrayPool<byte>.Get(instance.Packet.CurrentSeek);

        Array.Copy(instance.Packet.Data, 0, packet, 0, packet.Length);
        Client.SendPacketToInGameUsers(packet, command, sendType, sendToSender);

        ArrayPool<byte>.Recycle(packet);
    }

    /// <summary>
    /// Send the transform given to the lobby.
    /// </summary>
    /// /// <param name="data">buffer to include in the final packet in addition to the transform. Data will be written before the transform.</param>
    public static void SendTransformToInGameUsers(byte[] data, Transform transform, PacketType command, EP2PSend sendType, bool sendToSender = true)
    {
        instance.Packet.CurrentSeek = 0;

        instance.Packet.WriteByteData(data, 0, data.Length);

        instance.WriteTransformOnPacketData(transform, false);

        byte[] packet = ArrayPool<byte>.Get(instance.Packet.CurrentSeek);

        Array.Copy(instance.Packet.Data, 0, packet, 0, packet.Length);
        Client.SendPacketToInGameUsers(packet, command, sendType, sendToSender);

        ArrayPool<byte>.Recycle(packet);
    }

    void OnGUI()
    {
        GUILayout.Label("Latency " + Latency.ToString());
    }
}