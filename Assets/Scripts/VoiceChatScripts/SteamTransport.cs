using VOCASY.Common;
using UnityEngine;
using System.Collections.Generic;
using Steamworks;
using GENUtility;
[CreateAssetMenu(menuName = "VOCASY/DataTransports/Steam", fileName = "Steam Transport")]
public class SteamTransport : Transport
{
    public const int MaxAudioPacketSize = 1200;
    public override int MaxDataLength { get { return MaxAudioPacketSize - Transport.FirstPacketByteAvailable - Client.HeaderLength; } }
    private readonly byte[] boolean = new byte[sizeof(bool)];
    protected override void OnEnable()
    {
        this.toSend = new BytePacket(MaxAudioPacketSize - Client.HeaderLength);
    }
    public void Init()
    {
        SendMsgTo = SendMsg;
        SendToAllAction = SendAll;
        Client.AddCommand(PacketType.VoiceChatData, ReceivePacketAudioCommand);
        Client.AddCommand(PacketType.VoiceChatMutedMessage, ReceivePacketMuteMsgCommand);
    }
    private void SendAll(byte[] data, int startIndex, int length, List<ulong> receiversIds)
    {
        int lengthList = receiversIds.Count;
        for (int i = 0; i < lengthList; i++)
        {
            Client.SendPacket(data, startIndex, length, PacketType.VoiceChatData, Client.MyID, (CSteamID)receiversIds[i], EP2PSend.k_EP2PSendUnreliableNoDelay);
        }
    }
    private void SendMsg(ulong targetID, bool isTargetMutedByLocal)
    {
        ByteManipulator.Write(boolean, 0, isTargetMutedByLocal);
        Client.SendPacket(boolean, 0, sizeof(bool), PacketType.VoiceChatMutedMessage, Client.MyID, (CSteamID)targetID, EP2PSend.k_EP2PSendReliable);
    }
    private void ReceivePacketAudioCommand(byte[] data, uint dataLength, CSteamID sender)
    {
        Workflow.ProcessReceivedPacket(data, 0, (int)dataLength, sender.m_SteamID);
    }
    private void ReceivePacketMuteMsgCommand(byte[] data, uint dataLength, CSteamID sender)
    {
        if (dataLength < sizeof(bool))
            return;
        Workflow.ProcessIsMutedMessage(ByteManipulator.ReadBoolean(data, 0), sender.m_SteamID);
    }
}