using System;
using System.Collections.Generic;
using UnityEngine;
public class SelfDataTransport : MonoBehaviour, IAudioTransportLayer
{
    private const int FirstPacketByteAvailable = sizeof(uint) + sizeof(ushort) + sizeof(byte) + sizeof(bool);

    private const int pLength = 1024;

    public bool IsPacketAvailable { get { return packets.Count > 0; } }

    public uint MaxPacketLength { get { return pLength - FirstPacketByteAvailable; } }

    public ulong ReceiverId;

    private Queue<GamePacket> packets = new Queue<GamePacket>();
    public VoicePacketInfo Receive(GamePacket buffer)
    {
        //Debug.Log("Data packet received");
        GamePacket received = packets.Dequeue();

        VoicePacketInfo info = new VoicePacketInfo();
        info.NetId = received.ReadULong(0);
        info.Frequency = received.ReadUShort();
        info.Channels = received.ReadByte();
        info.Format = (AudioDataTypeFlag)received.ReadByte();
        info.ValidPacketInfo = true;

        buffer.WriteByteData(received, FirstPacketByteAvailable, 0, Mathf.Min(received.MaxCapacity, buffer.MaxCapacity));

        received.DisposePacket();

        return info;
    }
    public void SendToAllOthers(GamePacket data, VoicePacketInfo info)
    {
        //Debug.Log("packet sent to all others");
        GamePacket toSend = GamePacket.CreatePacket(pLength);
        toSend.Write(ReceiverId, 0);
        toSend.Write(info.Frequency);
        toSend.Write(info.Channels);
        toSend.Write((byte)info.Format);

        toSend.WriteByteData(data.Data, 0, data.CurrentLength);

        packets.Enqueue(toSend);
    }
    void Update()
    {
        //if (packets.Count != 0)
        //    Debug.Log(packets.Count);
        if (IsPacketAvailable && onPacketAvailable != null)
        {
            //Debug.Log("OnpacketAvailable");
            onPacketAvailable.Invoke();
        }
    }
    private Action onPacketAvailable;
    public void SetOnPacketAvailable(Action onPacketAvailable)
    {
        this.onPacketAvailable = onPacketAvailable;
    }
}