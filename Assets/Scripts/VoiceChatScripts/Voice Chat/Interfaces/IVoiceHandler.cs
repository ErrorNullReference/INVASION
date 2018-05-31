using System;
/// <summary>
/// Interface that represents a class that handles the voice input/output. Each handler should either record input or play output, not both
/// </summary>
public interface IVoiceHandler
{
    /// <summary>
    /// Flag that determines which types of data format this class can process
    /// </summary>
    AudioDataTypeFlag AvailableTypes { get; }
    /// <summary>
    /// True if this handler is recording input
    /// </summary>
    bool IsRecorder { get; }
    /// <summary>
    /// Mute condition specific for this output source
    /// </summary>
    bool IsSelfOutputMuted { get; set; }
    /// <summary>
    /// Is output source muted?
    /// </summary>
    bool IsOutputMuted { get; }
    /// <summary>
    /// Volume specific for this output source
    /// </summary>
    float SelfOutputVolume { get; set; }
    /// <summary>
    /// Effective volume of this output source
    /// </summary>
    float OutputVolume { get; }
    /// <summary>
    /// The INetworkIdentity associated with this object
    /// </summary>
    INetworkIdentity Identity { get; }
    /// <summary>
    /// Amount of mic data recorded available
    /// </summary>
    uint MicDataAvailable { get; }
    /// <summary>
    /// Processes audio data in format Single and plays it
    /// </summary>
    /// <param name="audioData">audio data to process</param>
    /// <param name="audioDataOffset">audio data start index</param>
    /// <param name="audioDataCount">audio data amount to process</param>
    /// <param name="info">data info</param>
    void ReceiveAudioData(float[] audioData, int audioDataOffset, int audioDataCount, VoicePacketInfo info);
    /// <summary>
    /// Gets recorded data and stores it in format Single
    /// </summary>
    /// <param name="buffer">buffer to fill with audio data recorded</param>
    /// <param name="bufferOffset">buffer start index</param>
    /// <param name="micDataCount">amount of data to store</param>
    /// <param name="effectiveMicDataCount">effective amount of data stored</param>
    /// <returns>data info</returns>
    VoicePacketInfo GetMicData(float[] buffer, int bufferOffset, int micDataCount, out uint effectiveMicDataCount);
    /// <summary>
    /// Processes audio data in format Int16 and plays it
    /// </summary>
    /// <param name="audioData">audio data to process</param>
    /// <param name="audioDataOffset">audio data start index</param>
    /// <param name="audioDataCount">audio data amount to process</param>
    /// <param name="info">data info</param>
    void ReceiveAudioDataInt16(byte[] audioData, int audioDataOffset, int audioDataCount, VoicePacketInfo info);
    /// <summary>
    /// Gets recorded data and stores it in format Int16
    /// </summary>
    /// <param name="buffer">buffer to fill with audio data recorded</param>
    /// <param name="bufferOffset">buffer start index</param>
    /// <param name="micDataCount">amount of data to store</param>
    /// <param name="effectiveMicDataCount">effective amount of data stored</param>
    /// <returns>data info</returns>
    VoicePacketInfo GetMicDataInt16(byte[] buffer, int bufferOffset, int micDataCount, out uint effectiveMicDataCount);
    /// <summary>
    /// Sets an action to be called whenever there is mic data available
    /// </summary>
    /// <param name="onMicDataProcessed">action called on mic data ready</param>
    void SetOnMicDataProcessed(Action<IVoiceHandler> onMicDataProcessed);
}