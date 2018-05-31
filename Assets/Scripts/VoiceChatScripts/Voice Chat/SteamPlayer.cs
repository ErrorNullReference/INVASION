using UnityEngine;
using Steamworks;

[RequireComponent(typeof(IVoiceHandler))]
public class SteamPlayer : MonoBehaviour
{
    private IVoiceHandler handler;

    void Awake()
    {
        SteamCallbackReceiver.ChatUpdateEvent += OnLobbyUpdate;
        handler = GetComponent<IVoiceHandler>();
    }

    void OnDestroy()
    {
        SteamCallbackReceiver.ChatUpdateEvent -= OnLobbyUpdate;
    }

    void OnLobbyUpdate(LobbyChatUpdate_t cb)
    {
        if ((EChatMemberStateChange)cb.m_rgfChatMemberStateChange != EChatMemberStateChange.k_EChatMemberStateChangeEntered && cb.m_ulSteamIDUserChanged == handler.Identity.NetworkId)
            Destroy(this.gameObject);
    }
}