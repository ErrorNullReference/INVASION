using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
using System.IO;

public class Settings : MonoBehaviour
{
    const string fileName = "settings";

    public bool SaveFile, LoadFile;
    public GameObject RootPanel;
    public bool OnEscIpnut;
    string path;
    bool modified;

    void OnEnable()
    {
        path = Path.Combine(Application.streamingAssetsPath, fileName);

        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return null;
        yield return null;

        Load(path);

        InitAudio();
        InitScreen();
    }

    void Update()
    {
        if (OnEscIpnut && Input.GetKeyDown(KeyCode.Escape))
            OpenCloseMenu();
    }

    public void OpenCloseMenu()
    {
        RootPanel.SetActive(!RootPanel.activeSelf);
        if (RootPanel.activeSelf)
        {
            OpenMenu(0);
            MenuEvents.OnMenuOpen.Invoke();
        }
        else
        {
            MenuEvents.OnMenuClose.Invoke();
            SaveCurrentSettings();
        }
    }

    void Save(string path)
    {
        if (!modified || !SaveFile)
            return;

        byte[] audio = SaveAudio();
        byte[] screen = SaveScreen();

        byte[] data = new byte[audio.Length + screen.Length];
        Array.Copy(audio, 0, data, 0, audio.Length);
        Array.Copy(screen, 0, data, audio.Length, screen.Length);

        File.WriteAllBytes(path, data);

        modified = false;
    }

    public void SaveCurrentSettings()
    {
        Save(path);
    }

    void Load(string path)
    {
        if (!LoadFile || !File.Exists(path))
            return;

        byte[] data = File.ReadAllBytes(path);

        int index = 0;
        index = LoadAudio(data, index);
        index = LoadScreen(data, index);
    }

    byte[] ComposeBytes(byte[][] arrays)
    {
        int length = 0;
        for (int i = 0; i < arrays.Length; i++)
            length += arrays[i].Length;
        byte[] data = new byte[length];

        length = 0;
        for (int i = 0; i < arrays.Length; i++)
        {
            Array.Copy(arrays[i], 0, data, length, arrays[i].Length);
            length += arrays[i].Length;
        }

        return data;
    }

    //SETTINGS//
    public GameObject[] Menues;

    public void OpenMenu(int id)
    {
        for (int i = 0; i < Menues.Length; i++)
        {
            if (i == id)
                Menues[i].SetActive(true);
            else if (Menues[i].activeSelf)
                Menues[i].SetActive(false);
        }
    }

    //AUDIO//
    public AudioMixer Mixer;
    public Slider[] Sliders;

    void InitAudio()
    {
        SetMasterVolume(Sliders[0].value);
        SetMusicVolume(Sliders[1].value);
        SetAmbientVolume(Sliders[2].value);
        SetSFXVolume(Sliders[3].value);
    }

    void SetVolume(string parameterName, float value)
    {
        modified = true;
        Mixer.SetFloat(parameterName, value == 0 ? -80 : Mathf.Log10(value) * 20);
    }

    void SetSliderValue(int index, float value)
    {
        Sliders[index].value = value;
    }

    public void SetMasterVolume(float value)
    {
        SetVolume("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        SetVolume("MusicVolume", value);
    }

    public void SetAmbientVolume(float value)
    {
        SetVolume("AmbientVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        SetVolume("SFXVolume", value);
    }

    byte[] SaveAudio()
    {
        byte[] data = new byte[sizeof(float) * Sliders.Length];
        int length = 0;

        for (int i = 0; i < Sliders.Length; i++)
        {
            byte[] b = BitConverter.GetBytes(Sliders[i].value);
            Array.Copy(b, 0, data, length, b.Length);
            length += b.Length;
        }

        return data;
    }

    int LoadAudio(byte[] data, int startIndex)
    {
        for (int i = 0; i < Sliders.Length; i++)
        {
            float volume = BitConverter.ToSingle(data, startIndex);
            SetSliderValue(i, volume);
            startIndex += sizeof(float);
        }

        return startIndex;
    }

    //SCREEN//

    public Dropdown WindowDropdown, ResolutionsDropdown, QualityDropdown;
    public Toggle VsyncToggle;

    void InitScreen()
    {
        //Window
        if (Screen.fullScreen)
            WindowDropdown.value = 0;
        else
            WindowDropdown.value = 1;

        //Resolutions
        ResolutionsDropdown.ClearOptions();
        List<string> resolutions = new List<string>(Screen.resolutions.Length);
        int current = 0;
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            resolutions.Add(Screen.resolutions[i].width + " x " + Screen.resolutions[i].height);
            if (Screen.resolutions[i].width == Screen.currentResolution.width && Screen.resolutions[i].height == Screen.currentResolution.height)
                current = i;
        }
        ResolutionsDropdown.AddOptions(resolutions);
        ResolutionsDropdown.value = current;

        //Quality
        QualityDropdown.ClearOptions();
        List<string> qualities = new List<string>(QualitySettings.names.Length);
        for (int i = 0; i < QualitySettings.names.Length; i++)
            qualities.Add(QualitySettings.names[i]);
        QualityDropdown.AddOptions(qualities);
        QualityDropdown.value = QualitySettings.GetQualityLevel();

        //VSync
        if (QualitySettings.vSyncCount == 0)
            VsyncToggle.isOn = false;
        else
            VsyncToggle.isOn = true;
    }

    public void SetScreen(int option)
    {
        if (option == 0)
            Screen.fullScreen = true;
        else
            Screen.fullScreen = false;

        modified = true;
    }

    public void SetResolutions(int option)
    {
        Screen.SetResolution(Screen.resolutions[option].width, Screen.resolutions[option].height, Screen.fullScreen);

        modified = true;
    }

    int GetResolutionIndex(Resolution res)
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].width == res.width && Screen.resolutions[i].height == res.height)
                return i;
        }

        return 0;
    }

    public void SetVSync(bool value)
    {
        if (!value)
            QualitySettings.vSyncCount = 0;
        else
            QualitySettings.vSyncCount = 1;

        modified = true;
    }

    public void SetQualityLevel(int level)
    {
        if (QualitySettings.GetQualityLevel() != level)
            QualitySettings.SetQualityLevel(level, true);

        modified = true;
    }

    byte[] SaveScreen()
    {
        byte[] window = BitConverter.GetBytes(Screen.fullScreen);

        byte[] resolution = BitConverter.GetBytes(GetResolutionIndex(Screen.currentResolution));

        byte[] quality = BitConverter.GetBytes(QualitySettings.GetQualityLevel());

        byte[] vsync = BitConverter.GetBytes(QualitySettings.vSyncCount);

        return ComposeBytes(new byte[][]{ window, resolution, quality, vsync });
    }

    int LoadScreen(byte[] data, int startIndex)
    {
        bool window = BitConverter.ToBoolean(data, startIndex);
        startIndex += sizeof(bool);
        int resolution = BitConverter.ToInt32(data, startIndex);
        startIndex += sizeof(int);
        int quality = BitConverter.ToInt32(data, startIndex);
        startIndex += sizeof(int);
        int vsync = BitConverter.ToInt32(data, startIndex);
        startIndex += sizeof(int);

        Screen.fullScreen = window;
        SetResolutions(resolution);
        SetQualityLevel(quality);
        QualitySettings.vSyncCount = vsync;

        return startIndex;
    }
}
