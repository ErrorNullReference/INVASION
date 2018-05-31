using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class VoiceChatSpawner : MonoBehaviour
{
    
    public GameObject SpeakerPrefab;
    private Lobby lobby;
    private List<GameObject> speakers;
    private List<GameObject> speakersToRemove;

    private void OnEnable()
    {
        speakersToRemove = new List<GameObject>();
        speakers = new List<GameObject>();
        lobby = Client.Lobby;
        SteamCallbackReceiver.ChatUpdateEvent += RemoveSpeaker;
        SteamCallbackReceiver.ChatUpdateEvent += AddSpeaker;
        Client.OnLobbyInitializationEvent += CreateSpeakers;
    }

    private void CreateSpeakers()
    {
        GameObject recorder = Instantiate(SpeakerPrefab);
        TestNetworkIdentity recorderIdentity = recorder.GetComponent<TestNetworkIdentity>();
        recorderIdentity.NetworkId = (ulong)Client.MyID;
        recorderIdentity.IsLocalPlayer = true;
        speakers.Add(recorder);
        foreach (var user in Client.Users)
        {
            if (user.SteamID != Client.MyID)
            {
                GameObject speaker = Instantiate(SpeakerPrefab);
                TestNetworkIdentity speakerIdentity = speaker.GetComponent<TestNetworkIdentity>();
                speakerIdentity.NetworkId = (ulong)user.SteamID;
                speakerIdentity.IsLocalPlayer = false;
                speakers.Add(speaker);
            }
        }
    }

    private void RemoveSpeaker(LobbyChatUpdate_t cb)
    {
        if ((EChatMemberStateChange)cb.m_rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeLeft || (EChatMemberStateChange)cb.m_rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeDisconnected)
        {
            speakersToRemove.Clear();
            for (int i = 0; i < speakers.Count; i++)
            {
                ulong Id = speakers[i].GetComponent<TestNetworkIdentity>().NetworkId;
                if (Id == cb.m_ulSteamIDUserChanged)
                {
                    speakersToRemove.Add(speakers[i]);
                    break;
                }
            }

            foreach (GameObject s in speakersToRemove)
            {
                speakers.Remove(s);
                Destroy(s);
            }
        }
    }

    private void AddSpeaker(LobbyChatUpdate_t cb)
    {
        if ((EChatMemberStateChange)cb.m_rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeEntered)
        {
            GameObject speaker = Instantiate(SpeakerPrefab);
            TestNetworkIdentity speakerIdentity = speaker.GetComponent<TestNetworkIdentity>();
            speakerIdentity.NetworkId = (ulong)cb.m_ulSteamIDUserChanged;
            speakerIdentity.IsLocalPlayer = false;
            speakers.Add(speaker);
        }
    }

    private void OnDisable()
    {
        SteamCallbackReceiver.ChatUpdateEvent -= RemoveSpeaker;
        SteamCallbackReceiver.ChatUpdateEvent -= AddSpeaker;
        Client.OnLobbyInitializationEvent -= CreateSpeakers;
    }
}
