using Steamworks;
public class SteamVoiceDataManipulator : IAudioDataManipulator
{
    private const int defaultBufferSize = 20000;

    public AudioDataTypeFlag AvailableTypes { get { return AudioDataTypeFlag.Int16; } }

    private GamePacket decompressBuffer = GamePacket.CreatePacket(defaultBufferSize);

    public void FromAudioDataToPacket(float[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info, GamePacket output)
    {
        //this method is not supported
        info.ValidPacketInfo = false;
        return;
    }

    public void FromAudioDataToPacketInt16(byte[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info, GamePacket output)
    {
        //writes audio data length
        output.Write(audioDataCount);

        //data is written on the output.
        int n = audioDataCount > output.MaxCapacity - output.CurrentSeek ? output.MaxCapacity - output.CurrentSeek : audioDataCount;
        output.WriteByteData(audioData, audioDataOffset, n);
    }

    public void FromPacketToAudioData(GamePacket packet, ref VoicePacketInfo info, float[] out_audioData, int out_audioDataOffset, out int dataCount)
    {
        //this method is not supported
        info.ValidPacketInfo = false;
        dataCount = -1;
        return;
    }

    public void FromPacketToAudioDataInt16(GamePacket packet, ref VoicePacketInfo info, byte[] out_audioData, int out_audioDataOffset, out int dataCount)
    {
        //reads audio data length
        int count = packet.ReadInt();

        //Restarts packet buffer to use
        decompressBuffer.ResetSeekLength();

        //fills buffer with only audio data from given packet
        decompressBuffer.WriteByteData(packet.Data, packet.CurrentSeek, count);

        EVoiceResult res = EVoiceResult.k_EVoiceResultUnsupportedCodec;

        //number of bytes written
        uint b = 0;
        //audio data is decompressed
        res = SteamUser.DecompressVoice(decompressBuffer.Data, (uint)decompressBuffer.CurrentLength, out_audioData, (uint)out_audioData.Length, out b, info.Frequency);

        dataCount = (int)b;

        //if an error occurred packet is invalid
        if (res != EVoiceResult.k_EVoiceResultOK)
            info.ValidPacketInfo = false;
    }
}