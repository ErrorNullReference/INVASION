using UnityEngine;
using Steamworks;
public class SteamRecorder : MonoBehaviour, IVoiceRecorder
{
    public bool IsDisabled { get { return !isRecording; } }

    public uint MicDataAvailable
    {
        get
        {
            uint n;
            isRecording = SteamUser.GetAvailableVoice(out n) != EVoiceResult.k_EVoiceResultNotRecording;
            return n;
        }
    }

    public AudioDataTypeFlag AvailableTypes { get { return AudioDataTypeFlag.Int16; } }

    public byte Channels { get { return 1; } }

    public ushort Frequency { get { return (ushort)SteamUser.GetVoiceOptimalSampleRate(); } }

    private bool isRecording = false;

    public VoicePacketInfo GetMicData(byte[] buffer, int bufferOffset, int dataCount, out uint effectiveDataCount)
    {
        effectiveDataCount = 0;
        if (IsDisabled)
            return VoicePacketInfo.InvalidPacket;

        EVoiceResult eR = SteamUser.GetAvailableVoice(out effectiveDataCount);
        isRecording = eR != EVoiceResult.k_EVoiceResultNotRecording;
        if (eR == EVoiceResult.k_EVoiceResultOK)
        {
            eR = SteamUser.GetVoice(true, buffer, (uint)Mathf.Min(effectiveDataCount, buffer.Length - bufferOffset, dataCount), out effectiveDataCount);
            return new VoicePacketInfo(0, Frequency, Channels, AudioDataTypeFlag.Int16, true);
        }
        else
        {
            effectiveDataCount = 0;
            return VoicePacketInfo.InvalidPacket;
        }
    }
    public VoicePacketInfo GetMicData(float[] buffer, int bufferOffset, int dataCount, out uint effectiveDataCount)
    {
        effectiveDataCount = 0;
        return VoicePacketInfo.InvalidPacket;
    }
    public void StartRecording()
    {
        SteamUser.StartVoiceRecording();
        uint n;
        isRecording = SteamUser.GetAvailableVoice(out n) != EVoiceResult.k_EVoiceResultNotRecording;
    }
    public void StopRecording()
    {
        SteamUser.StopVoiceRecording();
        uint n;
        isRecording = SteamUser.GetAvailableVoice(out n) != EVoiceResult.k_EVoiceResultNotRecording;
    }
}