using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class LobbyCreationQA : MonoBehaviour
{

    CallResult<LobbyCreated_t> lobbyCreated;

    // Use this for initialization
    void Start()
    {
        lobbyCreated = CallResult<LobbyCreated_t>.Create((cb, failure) =>
        {
            Debug.Log(failure);
            Debug.Log(cb.m_eResult);
        });
        SteamAPICall_t call = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePrivate, 4);
        lobbyCreated.Set(call);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
