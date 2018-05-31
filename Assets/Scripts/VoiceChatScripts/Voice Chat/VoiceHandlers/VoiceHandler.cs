using System;
using UnityEngine;
[RequireComponent(typeof(INetworkIdentity), typeof(IVoiceReceiver), typeof(IVoiceRecorder))]
public class VoiceHandler : MonoBehaviour, IVoiceHandler
{
    private const AudioDataTypeFlag SelfFlag = AudioDataTypeFlag.Both;

    public bool IsRecorder { get { return Identity.IsLocalPlayer; } }

    public INetworkIdentity Identity { get; private set; }

    public uint MicDataAvailable { get { return Recorder.IsDisabled ? 0 : Recorder.MicDataAvailable; } }

    public AudioDataTypeFlag AvailableTypes { get; private set; }

    public bool IsSelfOutputMuted { get; set; }

    public bool IsOutputMuted { get { return IsSelfOutputMuted || Mathf.Approximately(0f, OutputVolume); } }

    public float SelfOutputVolume { get { return selfOutputVolume; } set { selfOutputVolume = Mathf.Clamp01(value); } }

    public float OutputVolume { get { return SelfOutputVolume * Manager.Settings.VoiceChatVolume; } }

    protected VoiceDataWorkflow Manager { get { return manager; } }

    protected IVoiceReceiver Receiver { get; private set; }

    protected IVoiceRecorder Recorder { get; private set; }

    [SerializeField]
    private VoiceDataWorkflow manager;

    private float selfOutputVolume = 1f;

    private Action<IVoiceHandler> onMicDataProcessed;

    private Action updatePtt;

    private bool initialized = false;

    public void SetOnMicDataProcessed(Action<IVoiceHandler> onMicDataProcessed)
    {
        //Sets action to be called on mic data available
        this.onMicDataProcessed = onMicDataProcessed;
    }
    public VoicePacketInfo GetMicData(float[] buffer, int bufferOffset, int micDataCount, out uint effectiveMicAudioData)
    {
        effectiveMicAudioData = 0;
        //Gets mic data from recorder if not disabled
        if (Recorder.IsDisabled)
            return VoicePacketInfo.InvalidPacket;

        VoicePacketInfo info = Recorder.GetMicData(buffer, bufferOffset, micDataCount, out effectiveMicAudioData);
        info.NetId = Identity.NetworkId;

        return info;
    }
    public VoicePacketInfo GetMicDataInt16(byte[] buffer, int bufferOffset, int micDataCount, out uint effectiveMicAudioData)
    {
        effectiveMicAudioData = 0;
        //Gets mic data from recorder if not disabled
        if (Recorder.IsDisabled)
            return VoicePacketInfo.InvalidPacket;

        VoicePacketInfo info = Recorder.GetMicData(buffer, bufferOffset, micDataCount, out effectiveMicAudioData);
        info.NetId = Identity.NetworkId;

        return info;
    }
    public void ReceiveAudioData(float[] audioData, int audioDataOffset, int audioDataCount, VoicePacketInfo info)
    {
        //Gives receiver the audio data for the output is not disabled
        if (Receiver.IsDisabled)
            return;
        Receiver.ReceiveAudioData(audioData, audioDataOffset, audioDataCount, info);
    }
    public void ReceiveAudioDataInt16(byte[] audioData, int audioDataOffset, int audioDataCount, VoicePacketInfo info)
    {
        //Gives receiver the audio data for the output is not disabled
        if (Receiver.IsDisabled)
            return;
        Receiver.ReceiveAudioData(audioData, audioDataOffset, audioDataCount, info);
    }

    void Start()
    {
        initialized = true;
        OnEnable();
    }
    void Update()
    {
        //If it is not a recorder update output volume
        if (!IsRecorder)
        {
            Receiver.Volume = OutputVolume;
            return;
        }

        //custom ptt update
        updatePtt.Invoke();

        //if the are mic data recorded available and there is an action setted call it
        if (MicDataAvailable > 0 && onMicDataProcessed != null)
            onMicDataProcessed.Invoke(this);
    }
    void OnEnable()
    {
        if (initialized)
        {
            //If this is a recorder && ptt is off start recording
            OnPushToTalkChanged();
            //Set receiver enable status
            Receiver.Enable(!IsRecorder);
            //Add self to the workflow
            Manager.AddVoiceHandler(this);
        }
    }
    void OnDisable()
    {
        if (initialized)
        {
            //If this is a recorder stop recording
            if (IsRecorder)
                Recorder.StopRecording();
            //Make sure to disables receiver
            Receiver.Enable(false);
            //Removes self from the workflow
            Manager.RemoveVoiceHandler(this);
        }
    }
    void Awake()
    {
        //Get all required components
        Identity = GetComponent<INetworkIdentity>();
        Receiver = GetComponent<IVoiceReceiver>();
        Recorder = GetComponent<IVoiceRecorder>();

        //Compatibility check between self, receiver and recorder
        AudioDataTypeFlag res = Receiver.AvailableTypes & Recorder.AvailableTypes & SelfFlag;

        if (res == AudioDataTypeFlag.None)
            throw new ArgumentException("The given handler type is incompatible with its underlying receiver and recorder components");

        //Set the compatibility value of this handler
        AvailableTypes = res;

        if (IsRecorder)
            Manager.Settings.PushToTalkChanged += OnPushToTalkChanged;
    }
    void OnDestroy()
    {
        Manager.Settings.PushToTalkChanged -= OnPushToTalkChanged;
    }

    void PTTOffUpdate()
    {
        if (Recorder.IsDisabled)
            Recorder.StartRecording();
    }
    void PTTOnUpdate()
    {
        if (Manager.Settings.IsPushToTalkKeyOpen())
        {
            //if ptt key is pressed and recorder is not recording start recording
            if (Recorder.IsDisabled)
                Recorder.StartRecording();
        }
        else if (!Recorder.IsDisabled) //if ptt key is not pressed and recorder is recording stop recording
            Recorder.StopRecording();
    }
    void OnPushToTalkChanged()
    {
        if (Manager.Settings.PushToTalk)
        {
            //if ptt is on set custom update and stop recording
            updatePtt = PTTOnUpdate;
            Recorder.StopRecording();
        }
        else
        {
            //if ptt is off remove custom update and start recording if neccessary
            updatePtt = PTTOffUpdate;
            if (Recorder.IsDisabled)
                Recorder.StartRecording();
        }
    }
}