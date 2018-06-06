using VOCASY.Common;
using UnityEngine;
using System.Collections.Generic;
using Steamworks;
using GENUtility;
[CreateAssetMenu(menuName = "VOCASY/DataTransports/Steam", fileName = "Steam Transport")]
public class SteamTransport : Transport
{
    private void OnEnable()
    {
        if (Application.isPlaying)
        {
            SendMsgTo = SendMsg;
            SendToAllAction = SendAll;
            Client.AddCommand(PacketType.VoiceChatData, ReceivePacketAudioCommand);
            Client.AddCommand(PacketType.VoiceChatMutedMessage, ReceivePacketMuteMsgCommand);
        }
    }
    private void SendAll(byte[] data, int startIndex, int length, List<ulong> receiversIds)
    {
        int lengthList = receiversIds.Count;
        for (int i = 0; i < lengthList; i++)
        {
            Client.SendPacket(data, PacketType.VoiceChatData, Client.MyID, (CSteamID)receiversIds[i], EP2PSend.k_EP2PSendUnreliableNoDelay);
        }
    }
    private void SendMsg(ulong targetID, bool isTargetMutedByLocal)
    {
        byte[] boolean = new byte[sizeof(bool)];
        ByteManipulator.Write(boolean, 0, isTargetMutedByLocal);
        Client.SendPacket(boolean, PacketType.VoiceChatMutedMessage, Client.MyID, (CSteamID)targetID, EP2PSend.k_EP2PSendReliable);
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