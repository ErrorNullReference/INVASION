using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using VOCASY;
using VOCASY.Common;
public class VoiceChatInitializer : MonoBehaviour
{
   
    public Handler SpeakerPrefab;
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
        CreateSpeakers();
    }

    private void CreateSpeakers()
    {
        GameObject recorder = Instantiate(SpeakerPrefab.gameObject);
        Handler recorderIdentity = recorder.GetComponent<Handler>();
        recorderIdentity.Identity = new NetworkIdentity();
        recorderIdentity.Identity.NetworkId = (ulong)Client.MyID;
        recorderIdentity.Identity.IsLocalPlayer = true;
        recorderIdentity.Identity.IsInitialized = true;
        speakers.Add(recorder);
        foreach (var user in Client.Users)
        {
            if (user.SteamID != Client.MyID)
            {
                GameObject speaker = Instantiate(SpeakerPrefab.gameObject);
                Handler speakerIdentity = speaker.GetComponent<Handler>();
                speakerIdentity.Identity = new NetworkIdentity();
                speakerIdentity.Identity.NetworkId = (ulong)user.SteamID;
                speakerIdentity.Identity.IsLocalPlayer = false;
                speakerIdentity.Identity.IsInitialized = true;
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
                ulong Id = speakers[i].GetComponent<Handler>().NetID;
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
            GameObject speaker = Instantiate(SpeakerPrefab.gameObject);
            Handler speakerIdentity = speaker.GetComponent<Handler>();
            speakerIdentity.Identity = new NetworkIdentity();
            speakerIdentity.Identity.NetworkId = (ulong)cb.m_ulSteamIDUserChanged;
            speakerIdentity.Identity.IsLocalPlayer = false;
            speakerIdentity.Identity.IsInitialized = true;
            speakers.Add(speaker);
        }
    }
}