using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class ActivateVoiceChat : MonoBehaviour {

	// Use this for initialization
	void Start () {
        SteamCallbackReceiver.LobbyEnterEvent += VoiceChatActivation;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void VoiceChatActivation(LobbyEnter_t cb)
    {
        GetComponent<VoiceChatSpawner>().enabled = true;
    }
}
