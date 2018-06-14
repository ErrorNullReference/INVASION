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
        throw new System.NotImplementedException();
    }
    public void RestoreAndEnableSpeakers()
    {
        throw new System.NotImplementedException();
    }
    public void RemoveLocalSpeaker()
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
    public void CreateSpeakers()
    {
        int nullObjsRemovedFromPool;
        GameObject recorder = SpeakerPool.Get(out nullObjsRemovedFromPool);
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
                GameObject speaker = SpeakerPool.Get(out nullObjsRemovedFromPool);
                Handler speakerIdentity = speaker.GetComponent<Handler>();
                speakerIdentity.Identity = new NetworkIdentity();
                speakerIdentity.Identity.NetworkId = (ulong)user.SteamID;
                speakerIdentity.Identity.IsLocalPlayer = false;
                speakerIdentity.Identity.IsInitialized = true;
                speakerIdentity.ForceInitializzation();
            }
        }
    }
    public void RemoveSpeaker(CSteamID exited)
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
    public void AddSpeaker(CSteamID entered)
    {
        int nullObjsRemovedFromPool;
        GameObject speaker = SpeakerPool.Get(out nullObjsRemovedFromPool);
        Handler speakerIdentity = speaker.GetComponent<Handler>();
        speakerIdentity.Identity = new NetworkIdentity();
        speakerIdentity.Identity.NetworkId = entered.m_SteamID;
        speakerIdentity.Identity.IsLocalPlayer = false;
        speakerIdentity.Identity.IsInitialized = true;
        speakerIdentity.ForceInitializzation();
    }
}