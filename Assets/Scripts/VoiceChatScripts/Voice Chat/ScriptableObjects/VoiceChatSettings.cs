using UnityEngine;
using System.IO;
/// <summary>
/// Class that manages and holds voice chat settings
/// </summary>
[CreateAssetMenu(menuName = "Communication/Voice Chat/Settings")]
public class VoiceChatSettings : ScriptableObject
{
    /// <summary>
    /// Delegate used to subscribe to VoiceChatSettings events
    /// </summary>
    public delegate void OnEvent();
    /// <summary>
    /// Delegate used to subscribe to micChanged events. Takes previous mic device as argument
    /// </summary>
    public delegate void OnMicChanged(string previousDevice);
    /// <summary>
    /// Delegate used to subscribe to qualityChanged events. Takes previous quality as argument
    /// </summary>
    public delegate void OnQualityChanged(FrequencyType previousQuality);

    /// <summary>
    /// Minimum frequency possible
    /// </summary>
    public const ushort MinFrequency = (ushort)FrequencyType.LowerThanAverageQuality;
    /// <summary>
    /// Maximum frequency possible
    /// </summary>
    public const ushort MaxFrequency = (ushort)FrequencyType.BestQuality;

    /// <summary>
    /// Name of the folder used to store files
    /// </summary>
    public string FolderName { get { return folderName; } }
    /// <summary>
    /// Name of the file used to store settings
    /// </summary>
    public string SettingsFileName { get { return settingsFileName; } }
    /// <summary>
    /// File complete path name that contains saved settings.
    /// </summary>
    public string SavedCustomValuesPath { get; private set; }
    /// <summary>
    /// Directory full path that contains files.
    /// </summary>
    public string SavedCustomValuesDirectoryPath { get; private set; }
    /// <summary>
    /// Determines whenever self should be muted
    /// </summary>
    public bool MuteSelf
    {
        get { return muteSelf; }
        set
        {
            if (muteSelf != value)
            {
                muteSelf = value;
                if (MuteSelfChanged != null)
                    MuteSelfChanged.Invoke();
            }
        }
    }
    /// <summary>
    /// Determines if voice chat works in mode push to talk
    /// </summary>
    public bool PushToTalk
    {
        get { return pushToTalk; }
        set
        {
            if (pushToTalk != value)
            {
                pushToTalk = value;
                if (PushToTalkChanged != null)
                    PushToTalkChanged.Invoke();
            }
        }
    }
    /// <summary>
    /// Key used in push to talk mode
    /// </summary>
    public KeyCode PushToTalkKey { get { return pushToTalkKey; } set { pushToTalkKey = value; } }
    /// <summary>
    /// Audio quality used. Does not effect audio received from network
    /// </summary>
    public FrequencyType AudioQuality
    {
        get { return audioQuality; }
        set
        {
            FrequencyType newF = (FrequencyType)Mathf.Clamp((int)value, MinFrequency, MaxFrequency);
            if (audioQuality != newF)
            {
                FrequencyType prev = audioQuality;
                audioQuality = newF;
                if (AudioQualityChanged != null)
                    AudioQualityChanged.Invoke(prev);
            }
        }
    }
    /// <summary>
    /// Current microphone device to be used for recording
    /// </summary>
    public string MicrophoneDevice
    {
        get { return microphoneDevice; }
        set
        {
            if (microphoneDevice != value)
            {
                string prev = microphoneDevice;
                microphoneDevice = value;
                if (MicrophoneDeviceChanged != null)
                    MicrophoneDeviceChanged.Invoke(prev);
            }
        }
    }
    /// <summary>
    /// Determines whenever voice chat shoul dbe enabled
    /// </summary>
    public bool VoiceChatEnabled
    {
        get { return voiceChatEnabled; }
        set
        {
            if (voiceChatEnabled != value)
            {
                voiceChatEnabled = value;
                if (VoiceChatEnabledChanged != null)
                    VoiceChatEnabledChanged.Invoke();
            }
        }
    }
    /// <summary>
    /// Determines volume of voice chat data received from network
    /// </summary>
    public float VoiceChatVolume { get { return voiceChatVolume; } set { voiceChatVolume = value; } }

    /// <summary>
    /// Event called whenever push to talk mode has been changed
    /// </summary>
    public event OnEvent PushToTalkChanged;
    /// <summary>
    /// Event called whenever MuteSelf state has been changed
    /// </summary>
    public event OnEvent MuteSelfChanged;
    /// <summary>
    /// Event called whenever Audio Quality value has been changed
    /// </summary>
    public event OnQualityChanged AudioQualityChanged;
    /// <summary>
    /// Event called whenever the current microphone device has been changed
    /// </summary>
    public event OnMicChanged MicrophoneDeviceChanged;
    /// <summary>
    /// Event called whenever Voice Chat enbaled state has been changed
    /// </summary>
    public event OnEvent VoiceChatEnabledChanged;

    [SerializeField]
    private string folderName = "Communication";

    [SerializeField]
    private string settingsFileName = "VoiceChatSettings.txt";

    [SerializeField]
    private string microphoneDevice = string.Empty;

    [SerializeField]
    private bool voiceChatEnabled = true;

    [SerializeField]
    private bool pushToTalk = true;

    [SerializeField]
    private KeyCode pushToTalkKey = KeyCode.C;

    [SerializeField]
    private bool muteSelf = false;

    [SerializeField]
    private FrequencyType audioQuality = (FrequencyType)MaxFrequency;

    [Range(0f, 1f)]
    [SerializeField]
    private float voiceChatVolume = 1f;

    /// <summary>
    /// Restore the settings to the saved file values. If file is not found it is created with current settings values
    /// </summary>
    public void RestoreToSavedSettings()
    {
        if (File.Exists(SavedCustomValuesPath))
            JsonUtility.FromJsonOverwrite(File.ReadAllText(SavedCustomValuesPath), this);
        else
            SaveCurrentSettings();
    }
    /// <summary>
    /// Saves current settings to file. If it is not performed changes to the settings will not be recorded
    /// </summary>
    public void SaveCurrentSettings()
    {
        if (!Directory.Exists(SavedCustomValuesDirectoryPath))
            Directory.CreateDirectory(SavedCustomValuesDirectoryPath);

        File.WriteAllText(SavedCustomValuesPath, JsonUtility.ToJson(this));
    }
    /// <summary>
    /// Checks if the ptt key is pressed
    /// </summary>
    /// <returns>true if ptt key is pressed</returns>
    public bool IsPushToTalkKeyOpen()
    {
        return Input.GetKey(PushToTalkKey);
    }
    /// <summary>
    /// Checks if the ptt key has just been released
    /// </summary>
    /// <returns>true if ptt key has just been released</returns>
    public bool IsPushToTalkKeyReleased()
    {
        return Input.GetKeyUp(PushToTalkKey);
    }
    /// <summary>
    /// Checks if the ptt key has just been pressed
    /// </summary>
    /// <returns>true if ptt key has just been pressed</returns>
    public bool IsPushToTalkKeyDown()
    {
        return Input.GetKeyDown(PushToTalkKey);
    }
    void OnEnable()
    {
        SavedCustomValuesDirectoryPath = Path.Combine(Application.persistentDataPath, FolderName);

        SavedCustomValuesPath = Path.Combine(SavedCustomValuesDirectoryPath, SettingsFileName);

        RestoreToSavedSettings();
    }
}