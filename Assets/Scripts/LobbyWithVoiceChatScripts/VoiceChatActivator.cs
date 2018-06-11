using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class VoiceChatActivator : MonoBehaviour {

	// Use this for initialization
	void Start () {
        SteamCallbackReceiver.LobbyCreateEvent += ActivateVoiceChatAtCreation;
        SteamCallbackReceiver.LobbyEnterEvent += ActivateVoiceChat;
       
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void ActivateVoiceChat(LobbyEnter_t cb)
    {
        GetComponent<VoiceChatSpawner>().enabled = true;
    }
    private void ActivateVoiceChatAtCreation(LobbyCreated_t cb)
    {
        GetComponent<VoiceChatSpawner>().enabled = true;
    }
}
