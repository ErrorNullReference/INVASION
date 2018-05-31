using System;
/// <summary>
/// Interface that represents a class that transports packets
/// </summary>
public interface IAudioTransportLayer
{
    /// <summary>
    /// Max data length that should be sent to this class
    /// </summary>
    uint MaxPacketLength { get; }
    /// <summary>
    /// True as long as there are packets available to receive
    /// </summary>
    bool IsPacketAvailable { get; }
    /// <summary>
    /// Sets an action to be called when a packet is available
    /// </summary>
    /// <param name="onPacketAvailable">Action called on packet available</param>
    void SetOnPacketAvailable(Action onPacketAvailable);
    /// <summary>
    /// Receive packet data
    /// </summary>
    /// <param name="buffer">GamePacket of which data will be stored</param>
    /// <returns>data info</returns>
    VoicePacketInfo Receive(GamePacket buffer);
    /// <summary>
    /// Sends a packet to all the other clients that need it
    /// </summary>
    /// <param name="data">GamePacket that stores the data to send</param>
    /// <param name="info">data info</param>
    void SendToAllOthers(GamePacket data, VoicePacketInfo info);
}