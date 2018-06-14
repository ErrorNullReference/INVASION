using Steamworks;
using VOCASY;
using GENUtility;
using UnityEngine;
[CreateAssetMenu(menuName = "VOCASY/DataManipulators/Steam")]
public class SteamVoiceDataManipulator : VoiceDataManipulator
{
    private const int defaultBufferSize = 1200;

    public override AudioDataTypeFlag AvailableTypes { get { return AudioDataTypeFlag.Int16; } }

    private BytePacket decompressBuffer = new BytePacket(defaultBufferSize);

    public override void FromAudioDataToPacket(float[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info, BytePacket output)
    {
        //this method is not supported
        info.ValidPacketInfo = false;
        return;
    }
    public override void FromAudioDataToPacketInt16(byte[] audioData, int audioDataOffset, int audioDataCount, ref VoicePacketInfo info, BytePacket output)
    {
        //writes audio data length
        output.Write(audioDataCount);

        //data is written on the output.
        int n = audioDataCount > output.MaxCapacity - output.CurrentSeek ? output.MaxCapacity - output.CurrentSeek : audioDataCount;
        output.WriteByteData(audioData, audioDataOffset, n);
    }

    public override int FromPacketToAudioData(BytePacket packet, ref VoicePacketInfo info, float[] out_audioData, int out_audioDataOffset)
    {
        //this method is not supported
        info.ValidPacketInfo = false;
        return -1;
    }

    public override int FromPacketToAudioDataInt16(BytePacket packet, ref VoicePacketInfo info, byte[] out_audioData, int out_audioDataOffset)
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

        //if an error occurred packet is invalid
        if (res != EVoiceResult.k_EVoiceResultOK)
            info.ValidPacketInfo = false;

        return (int)b;
    }
}