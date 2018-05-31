/// <summary>
/// Interface that represents a class that manages audio input
/// </summary>
public interface IVoiceRecorder
{
    /// <summary>
    /// Flag that determines which types of data format this class can process
    /// </summary>
    AudioDataTypeFlag AvailableTypes { get; }
    /// <summary>
    /// Is this input disabled?
    /// </summary>
    bool IsDisabled { get; }
    /// <summary>
    /// Amount of mic data recorded currently available
    /// </summary>
    uint MicDataAvailable { get; }
    /// <summary>
    /// Gets recorded data and stores it in format Single
    /// </summary>
    /// <param name="buffer">buffer to fill with audio data recorded</param>
    /// <param name="bufferOffset">buffer start index</param>
    /// <param name="dataCount">amount of data to store</param>
    /// <param name="effectiveDataCount">effective amount of data stored</param>
    /// <returns>data info</returns>
    VoicePacketInfo GetMicData(float[] buffer, int bufferOffset, int dataCount , out uint effectiveDataCount);
    /// <summary>
    /// Gets recorded data and stores it in format Int16
    /// </summary>
    /// <param name="buffer">buffer to fill with audio data recorded</param>
    /// <param name="bufferOffset">buffer start index</param>
    /// <param name="dataCount">amount of data to store</param>
    /// <param name="effectiveDataCount">effective amount of data stored</param>
    /// <returns>data info</returns>
    VoicePacketInfo GetMicData(byte[] buffer, int bufferOffset, int dataCount , out uint effectiveDataCount);
    /// <summary>
    /// Starts recording
    /// </summary>
    void StartRecording();
    /// <summary>
    /// Stops recording
    /// </summary>
    void StopRecording();
}