using UnityEngine;
using Steamworks;
using VOCASY.Common;
using SOPRO;
using VOCASY;
public class VoiceChatSpawner : MonoBehaviour
{

    public SOPool SpeakerPool;
    public Workflow Workflow;
    private Lobby lobby;

    private void OnEnable()
    {
        lobby = Client.Lobby;
        SteamCallbackReceiver.ChatUpdateEvent += RemoveSpeaker;
        SteamCallbackReceiver.ChatUpdateEvent += AddSpeaker;
        Client.OnLobbyInitializationEvent += CreateSpeakers;
    }

    private void CreateSpeakers()
    {
        GameObject recorder = SpeakerPool.Get();
        Handler recorderIdentity = recorder.GetComponent<Handler>();
        recorderIdentity.Identity = new NetworkIdentity();
        recorderIdentity.Identity.NetworkId = (ulong)Client.MyID;
        recorderIdentity.Identity.IsLocalPlayer = true;
        recorderIdentity.Identity.IsInitialized = true;

        foreach (var user in Client.Users)
        {
            if (user.SteamID != Client.MyID)
            {
                GameObject speaker = SpeakerPool.Get();
                Handler speakerIdentity = speaker.GetComponent<Handler>();
                speakerIdentity.Identity = new NetworkIdentity();
                speakerIdentity.Identity.NetworkId = (ulong)user.SteamID;
                speakerIdentity.Identity.IsLocalPlayer = false;
                speakerIdentity.Identity.IsInitialized = true;
            }
        }
    }

    private void RemoveSpeaker(LobbyChatUpdate_t cb)
    {
        if ((EChatMemberStateChange)cb.m_rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeLeft || (EChatMemberStateChange)cb.m_rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeDisconnected)
        {
            //workflow will not find handler if the handler didn't have time to initialize itself (requires a single handler update cycle if Identity is initialized)
            //In this case if an handler is created and then it is requested to be removed within a single frame the handler most likely didn't have the time to initialize and no handler will be returned.
            VoiceHandler handler = Workflow.GetTrackedHandlerById(cb.m_ulSteamIDUserChanged);

            if (!handler)
            {
                SpeakerPool.Recycle(handler.gameObject);

                //Now that handler is disabled reset its initialization status
                Handler h = handler as Handler;
                if(h != null)
                {
                    h.Reset();
                    h.Identity.IsInitialized = false;
                }
            }
        }
    }

    private void AddSpeaker(LobbyChatUpdate_t cb)
    {
        if ((EChatMemberStateChange)cb.m_rgfChatMemberStateChange == EChatMemberStateChange.k_EChatMemberStateChangeEntered)
        {
            GameObject speaker = SpeakerPool.Get();
            Handler speakerIdentity = speaker.GetComponent<Handler>();
            speakerIdentity.Identity = new NetworkIdentity();
            speakerIdentity.Identity.NetworkId = (ulong)cb.m_ulSteamIDUserChanged;
            speakerIdentity.Identity.IsLocalPlayer = false;
            speakerIdentity.Identity.IsInitialized = true;
        }
    }

    private void OnDisable()
    {
        SteamCallbackReceiver.ChatUpdateEvent -= RemoveSpeaker;
        SteamCallbackReceiver.ChatUpdateEvent -= AddSpeaker;
        Client.OnLobbyInitializationEvent -= CreateSpeakers;
    }
}
