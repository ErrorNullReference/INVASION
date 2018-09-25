using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
	public GameObject RootPanel;
	public bool OnEscIpnut;

	void OnEnable ()
	{
		InitScreen ();
	}

	void Update ()
	{
		if (OnEscIpnut && Input.GetKeyDown (KeyCode.Escape))
			OpenCloseMenu ();
	}

	public void OpenCloseMenu ()
	{
		RootPanel.SetActive (!RootPanel.activeSelf);
		if (RootPanel.activeSelf) {
			OpenMenu (0);
			MenuEvents.OnMenuOpen.Invoke ();
		} else
			MenuEvents.OnMenuClose.Invoke ();
	}

	//SETTINGS//
	public GameObject[] Menues;

	public void OpenMenu (int id)
	{
		for (int i = 0; i < Menues.Length; i++) {
			if (i == id)
				Menues [i].SetActive (true);
			else if (Menues [i].activeSelf)
				Menues [i].SetActive (false);
		}
	}

	//AUDIO//
	public AudioMixer Mixer;

	void SetVolume (string parameterName, float value)
	{
		Mixer.SetFloat (parameterName, value == 0 ? -80 : Mathf.Log10 (value) * 20);
	}

	public void SetMasterVolume (float value)
	{
		SetVolume ("MasterVolume", value);
	}

	public void SetMusicVolume (float value)
	{
		SetVolume ("MusicVolume", value);
	}

	public void SetAmbientVolume (float value)
	{
		SetVolume ("AmbientVolume", value);
	}

	public void SetSFXVolume (float value)
	{
		SetVolume ("SFXVolume", value);
	}

	//SCREEN//

	public Dropdown WindowDropdown, ResolutionsDropdown, QualityDropdown;
	public Toggle VsyncToggle;

	void InitScreen ()
	{
		//Window
		if (Screen.fullScreen)
			WindowDropdown.value = 0;
		else
			WindowDropdown.value = 1;

		//Resolutions
		ResolutionsDropdown.ClearOptions ();
		List<string> resolutions = new List<string> (Screen.resolutions.Length);
		int current = 0;
		for (int i = 0; i < Screen.resolutions.Length; i++) 
		{
			resolutions.Add (Screen.resolutions [i].width + " x " + Screen.resolutions [i].height);
			if (Screen.resolutions [i].width == Screen.currentResolution.width && Screen.resolutions [i].height == Screen.currentResolution.height)
				current = i;
		}
		ResolutionsDropdown.AddOptions (resolutions);
		ResolutionsDropdown.value = current;

		//Quality
		QualityDropdown.ClearOptions();
		List<string> qualities = new List<string> (QualitySettings.names.Length);
		for (int i = 0; i < QualitySettings.names.Length; i++) 
			qualities.Add (QualitySettings.names [i]);
		QualityDropdown.AddOptions (qualities);
		QualityDropdown.value = QualitySettings.GetQualityLevel ();

		//VSync
		if (QualitySettings.vSyncCount == 0)
			VsyncToggle.isOn = false;
		else
			VsyncToggle.isOn = true;
	}

	public void SetScreen (int option)
	{
		if (option == 0)
			Screen.fullScreen = true;
		else
			Screen.fullScreen = false;
	}

	public void SetResolutions (int option)
	{
		Screen.SetResolution (Screen.resolutions [option].width, Screen.resolutions [option].height, Screen.fullScreen);
	}

	public void SetVSync (bool value)
	{
		if (!value)
			QualitySettings.vSyncCount = 0;
		else
			QualitySettings.vSyncCount = 1;
	}

	public void SetQualityLevel (int level)
	{
		if (QualitySettings.GetQualityLevel () != level)
			QualitySettings.SetQualityLevel (level, true);
	}
}
