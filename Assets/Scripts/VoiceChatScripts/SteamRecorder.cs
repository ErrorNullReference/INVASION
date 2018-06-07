using UnityEngine;
using Steamworks;
using VOCASY;
public class SteamRecorder : VoiceRecorder
{
    public override bool IsEnabled { get { return isRecording; } }

    public override int MicDataAvailable
    {
        get
        {
            uint n;
            EVoiceResult res = SteamUser.GetAvailableVoice(out n);
            isRecording = res != EVoiceResult.k_EVoiceResultNotRecording;
            return (int)n;
        }
    }

    public override AudioDataTypeFlag AvailableTypes { get { return AudioDataTypeFlag.Int16; } }

    public byte Channels { get { return 1; } }

    public ushort Frequency { get; private set; }

    private bool isRecording = false;

    public override VoicePacketInfo GetMicData(byte[] buffer, int bufferOffset, int dataCount, out int effectiveDataCount)
    {
        effectiveDataCount = 0;
        if (!IsEnabled)
            return VoicePacketInfo.InvalidPacket;

        uint n;

        EVoiceResult eR = SteamUser.GetAvailableVoice(out n);

        effectiveDataCount = (int)n;

        isRecording = eR != EVoiceResult.k_EVoiceResultNotRecording;
        if (eR == EVoiceResult.k_EVoiceResultOK)
        {
            eR = SteamUser.GetVoice(true, buffer, (uint)Mathf.Min(effectiveDataCount, buffer.Length - bufferOffset, dataCount), out n);
            effectiveDataCount = (int)n;
            return new VoicePacketInfo(Frequency, Channels, AudioDataTypeFlag.Int16, true);
        }
        else
        {
            effectiveDataCount = 0;
            return VoicePacketInfo.InvalidPacket;
        }
    }
    public override VoicePacketInfo GetMicData(float[] buffer, int bufferOffset, int dataCount, out int effectiveDataCount)
    {
        effectiveDataCount = 0;
        return VoicePacketInfo.InvalidPacket;
    }
    public override void StartRecording()
    {
        SteamUser.StartVoiceRecording();
        uint n;
        isRecording = SteamUser.GetAvailableVoice(out n) != EVoiceResult.k_EVoiceResultNotRecording;
    }
    public override void StopRecording()
    {
        SteamUser.StopVoiceRecording();
        uint n;
        isRecording = SteamUser.GetAvailableVoice(out n) != EVoiceResult.k_EVoiceResultNotRecording;
    }
    void Awake()
    {
        Frequency = (ushort)SteamUser.GetVoiceOptimalSampleRate();
    }
}