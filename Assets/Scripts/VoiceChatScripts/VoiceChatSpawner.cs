using UnityEngine;
using Steamworks;
using VOCASY.Common;
using SOPRO;
using VOCASY;
[CreateAssetMenu(menuName = "VOCASY/Steam/EntitiesSpawner")]
public class VoiceChatSpawner : ScriptableObject
{
    public SOPool SpeakerPool;
    public Workflow Workflow;

    public void DisableAndStoreSpeakers()
    {

    }
    public void RestoreAndEnableSpeakers()
    {

    }
    private void OnEnable()
    {
        Client.OnUserEnter += AddSpeaker;
        Client.OnUserLeave += RemoveSpeaker;
        Client.OnLobbyInitializationEvent += CreateSpeakers;
        Client.OnLobbyLeaveEvent += RemoveLocalSpeaker;
    }
    private void RemoveLocalSpeaker()
    {
        VoiceHandler handler = Workflow.GetTrackedHandlerById(Client.MyID.m_SteamID);

        if (handler)
        {
            SpeakerPool.Recycle(handler.gameObject);

            //Now that handler is disabled reset its initialization status
            Handler h = handler as Handler;
            if (h != null)
            {
                h.Reset();
                h.Identity.IsInitialized = false;
            }

        }
    }
    private void CreateSpeakers()
    {
        GameObject recorder = SpeakerPool.Get();
        Handler recorderIdentity = recorder.GetComponent<Handler>();
        recorderIdentity.Identity = new NetworkIdentity();
        recorderIdentity.Identity.NetworkId = (ulong)Client.MyID;
        recorderIdentity.Identity.IsLocalPlayer = true;
        recorderIdentity.Identity.IsInitialized = true;
        recorderIdentity.ForceInitializzation();

        foreach (User user in Client.Users)
        {
            if (user.SteamID != Client.MyID)
            {
                GameObject speaker = SpeakerPool.Get();
                Handler speakerIdentity = speaker.GetComponent<Handler>();
                speakerIdentity.Identity = new NetworkIdentity();
                speakerIdentity.Identity.NetworkId = (ulong)user.SteamID;
                speakerIdentity.Identity.IsLocalPlayer = false;
                speakerIdentity.Identity.IsInitialized = true;
                speakerIdentity.ForceInitializzation();
            }
        }
    }
    private void RemoveSpeaker(CSteamID exited)
    {
        VoiceHandler handler = Workflow.GetTrackedHandlerById(exited.m_SteamID);

        if (handler)
        {
            SpeakerPool.Recycle(handler.gameObject);

            //Now that handler is disabled reset its initialization status
            Handler h = handler as Handler;
            if (h != null)
            {
                h.Reset();
                h.Identity.IsInitialized = false;
            }

        }
    }
    private void AddSpeaker(CSteamID entered)
    {
        GameObject speaker = SpeakerPool.Get();
        Handler speakerIdentity = speaker.GetComponent<Handler>();
        speakerIdentity.Identity = new NetworkIdentity();
        speakerIdentity.Identity.NetworkId = entered.m_SteamID;
        speakerIdentity.Identity.IsLocalPlayer = false;
        speakerIdentity.Identity.IsInitialized = true;
        speakerIdentity.ForceInitializzation();
    }
    private void OnDisable()
    {
        Client.OnUserEnter -= AddSpeaker;
        Client.OnUserLeave -= RemoveSpeaker;
        Client.OnLobbyInitializationEvent -= CreateSpeakers;
        Client.OnLobbyLeaveEvent -= RemoveLocalSpeaker;
    }
}