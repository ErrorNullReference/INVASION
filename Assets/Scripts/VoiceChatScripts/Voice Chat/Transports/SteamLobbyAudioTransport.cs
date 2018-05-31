using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class SteamLobbyAudioTransport : MonoBehaviour, IAudioTransportLayer
{
    public const int FirstPacketByteAvailable = sizeof(uint) + sizeof(ushort) + sizeof(byte) + sizeof(bool);

    public bool IsPacketAvailable
    {
        get
        {
            uint pack;
            return SteamNetworking.IsP2PPacketAvailable(out pack);
        }
    }

    public uint MaxPacketLength { get { return (uint)temp1024.MaxCapacity - FirstPacketByteAvailable; } }

    [SerializeField]
    private SteamPlayer voiceUserPrefab;

    private Action onPacketAvailable;

    private GamePacket temp1024;
    private GamePacket temp768;
    private GamePacket temp512;
    private GamePacket temp256;

    private Dictionary<ulong, CSteamID> others = new Dictionary<ulong, CSteamID>(3);

    private CSteamID self;

    public VoicePacketInfo Receive(GamePacket packet)
    {
        CSteamID id;
        uint packSize;
        if (SteamNetworking.IsP2PPacketAvailable(out packSize))
        {
            if (packSize > packet.MaxCapacity)
            {
                return VoicePacketInfo.InvalidPacket;
            }

            uint b;

            SteamNetworking.ReadP2PPacket(temp1024.Data, packSize, out b, out id);

            VoicePacketInfo info = new VoicePacketInfo();
            info.NetId = temp1024.ReadUInt(0);
            info.Frequency = temp1024.ReadUShort();
            info.Channels = temp1024.ReadByte();
            info.Format = (AudioDataTypeFlag)temp1024.ReadByte();
            info.ValidPacketInfo = true;

            packet.WriteByteData(temp1024.Data, FirstPacketByteAvailable, (int)b);

            return info;
        }

        return VoicePacketInfo.InvalidPacket;
    }

    public void SendToAllOthers(GamePacket data, VoicePacketInfo info)
    {
        int l = data.CurrentLength + FirstPacketByteAvailable;

        GamePacket toUse = l <= temp256.MaxCapacity ? temp256 : (l <= temp512.MaxCapacity ? temp512 : (l <= temp768.MaxCapacity ? temp768 : temp1024));

        toUse.Write(info.NetId, 0);
        toUse.Write(info.Frequency);
        toUse.Write(info.Channels);
        toUse.Write((byte)info.Format);

        toUse.WriteByteData(data.Data, data.CurrentSeek, data.CurrentLength - data.CurrentSeek);

        foreach (KeyValuePair<ulong, CSteamID> item in others)
        {
            SteamNetworking.SendP2PPacket(item.Value, toUse.Data, (uint)toUse.MaxCapacity, EP2PSend.k_EP2PSendReliable);
        }
    }

    public void SetOnPacketAvailable(Action onPacketAvailable)
    {
        this.onPacketAvailable = onPacketAvailable;
    }

    void Update()
    {
        if (IsPacketAvailable && onPacketAvailable != null)
            onPacketAvailable.Invoke();
    }

    void Awake()
    {
        self = SteamUser.GetSteamID();
        Callback<P2PSessionRequest_t>.Create((cb) =>
            {
                if (others.ContainsKey(cb.m_steamIDRemote.m_SteamID))
                    SteamNetworking.AcceptP2PSessionWithUser(cb.m_steamIDRemote);
            });

        temp1024 = GamePacket.CreatePacket(1024);
        temp768 = GamePacket.CreatePacket(768);
        temp512 = GamePacket.CreatePacket(512);
        temp256 = GamePacket.CreatePacket(256);

        SteamCallbackReceiver.Init();

        SteamCallbackReceiver.ChatUpdateEvent += OnLobbyChatUpdate;
        SteamCallbackReceiver.LobbyEnterEvent += OnLobbyEnter;
    }

    void OnDestroy()
    {
        SteamCallbackReceiver.ChatUpdateEvent -= OnLobbyChatUpdate;
        SteamCallbackReceiver.LobbyEnterEvent -= OnLobbyEnter;
        //TODO: destroy all prefabs when leaving lobby
    }

    void OnLobbyEnter(LobbyEnter_t cb)
    {
        INetworkIdentity id = GameObject.Instantiate(voiceUserPrefab.gameObject).GetComponent<INetworkIdentity>();
        id.IsLocalPlayer = true;
        id.NetworkId = self.m_SteamID;

        if ((EChatRoomEnterResponse)cb.m_EChatRoomEnterResponse == EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
        {
            CSteamID lobbyId = new CSteamID(cb.m_ulSteamIDLobby);
            int n = SteamMatchmaking.GetNumLobbyMembers(lobbyId);

            for (int i = 0; i < n; i++)
            {
                CSteamID client = SteamMatchmaking.GetLobbyMemberByIndex(lobbyId, i);
                if (client.m_SteamID != self.m_SteamID)
                {
                    others.Add(client.m_SteamID, client);
                    id = GameObject.Instantiate(voiceUserPrefab.gameObject).GetComponent<INetworkIdentity>();
                    id.IsLocalPlayer = false;
                    id.NetworkId = client.m_SteamID;
                }
            }
        }
    }

    void OnLobbyChatUpdate(LobbyChatUpdate_t upd)
    {
        CSteamID changed = new CSteamID(upd.m_ulSteamIDUserChanged);

        if ((EChatMemberStateChange)upd.m_rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeEntered)
        {
            others.Add(changed.m_SteamID, changed);
            INetworkIdentity id = GameObject.Instantiate(voiceUserPrefab.gameObject).GetComponent<INetworkIdentity>();
            id.IsLocalPlayer = false;
            id.NetworkId = changed.m_SteamID;
        }
        else
        {
            others.Remove(changed.m_SteamID);
            SteamNetworking.CloseP2PSessionWithUser(changed);
        }
    }
}