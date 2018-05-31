/// <summary>
/// Interface that represents a class that manipulates audio data
/// </summary>
public interface IAudioDataManipulator
{
    /// <summary>
    /// Flag that determines which types of data format this class can process
    /// </summary>
    AudioDataTypeFlag AvailableTypes { get; }
    /// <summary>
    /// Processes audio data in format Int16 into a GamePacket
    /// </summary>
    /// <param name="audioData">audio data to process</param>
    /// <param name="audioDataOffset">audio data startIndex</param>
    /// <param name="audioDataCount">number of bytes to process</param>
    /// <param name="info">data info</param>
    /// <param name="output">gamepacket on which data will be written</param>
    void FromAudioDataToPacketInt16(byte[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info , GamePacket output);
    /// <summary>
    /// Processes a Gamepacket into audio data in format Int16
    /// </summary>
    /// <param name="packet">GamePacket to process</param>
    /// <param name="info">data info</param>
    /// <param name="out_audioData">output array on which data will be written</param>
    /// <param name="out_audioDataOffset">output array start index</param>
    /// <param name="dataCount">total number of bytes written</param>
    void FromPacketToAudioDataInt16(GamePacket packet, ref VoicePacketInfo info, byte[] out_audioData, int out_audioDataOffset, out int dataCount);
    /// <summary>
    /// Processes audio data in format Single into a GamePacket
    /// </summary>
    /// <param name="audioData">audio data to process</param>
    /// <param name="audioDataOffset">audio data startIndex</param>
    /// <param name="audioDataCount">number of floats to process</param>
    /// <param name="info">data info</param>
    /// <param name="output">gamepacket on which data will be written</param>
    void FromAudioDataToPacket(float[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info, GamePacket output);
    /// <summary>
    /// Processes a Gamepacket into audio data in format Single
    /// </summary>
    /// <param name="packet">GamePacket to process</param>
    /// <param name="info">data info</param>
    /// <param name="out_audioData">output array on which data will be written</param>
    /// <param name="out_audioDataOffset">output array start index</param>
    /// <param name="dataCount">total number of floats written</param>
    void FromPacketToAudioData(GamePacket packet, ref VoicePacketInfo info, float[] out_audioData, int out_audioDataOffset, out int dataCount);
}