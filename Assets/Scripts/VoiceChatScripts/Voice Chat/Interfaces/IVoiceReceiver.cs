/// <summary>
/// Interface that represents a class that manages audio output
/// </summary>
public interface IVoiceReceiver
{
    /// <summary>
    /// Flag that determines which types of data format this class can process
    /// </summary>
    AudioDataTypeFlag AvailableTypes { get; }
    /// <summary>
    /// Volume specific for this output source
    /// </summary>
    float Volume { get; set; }
    /// <summary>
    /// Is this output source disabled?
    /// </summary>
    bool IsDisabled { get; }
    /// <summary>
    /// Updates the enable status of this output source
    /// </summary>
    /// <param name="isEnabled">enable value</param>
    void Enable(bool isEnabled);
    /// <summary>
    /// Processes audio data in format Single and plays it
    /// </summary>
    /// <param name="audioData">audio data to process</param>
    /// <param name="audioDataOffset">audio data start index</param>
    /// <param name="audioDataCount">audio data amount to process</param>
    /// <param name="info">data info</param>
    void ReceiveAudioData(float[] audioData, int audioDataOffset, int audioDataCount, VoicePacketInfo info);
    /// <summary>
    /// Processes audio data in format Int16 and plays it
    /// </summary>
    /// <param name="audioData">audio data to process</param>
    /// <param name="audioDataOffset">audio data start index</param>
    /// <param name="audioDataCount">audio data amount to process</param>
    /// <param name="info">data info</param>
    void ReceiveAudioData(byte[] audioData, int audioDataOffset, int audioDataCount, VoicePacketInfo info);
}