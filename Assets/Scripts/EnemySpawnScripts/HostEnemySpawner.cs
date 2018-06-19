﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using GENUtility;
using SOPRO;
public class HostEnemySpawner : MonoBehaviour
{
    public SOVariableVector3 NearestSpawnPointOutsideView;
    public SOListVector3Container AllSpawnPointsOutsideView;
    public ReferenceInt MaxEnemyCount;
    public SOVariableInt CurrentEnemyCount;
    public NetIdDispenser IdDispenser;

    public ReferenceFloat TimeForLittleSquadSpawn;
    public ReferenceInt EnemyCountForBigSquadSpawn;

    public ReferenceInt MinLittleSquadSpawn;
    public ReferenceInt MaxLittleSquadSpawn;

    public ReferenceInt BigSquadSpawnMultiplier;

    int waveCount;
    WaitForEndOfFrame waitForFrame;
    WaitForSeconds waitForSeconds;
    private Vector3 spawnPos;
    bool coroutineOver = true;
    bool continuousCoroutineStarted;
    private static readonly BytePacket idAndPos = new BytePacket(17);
    private static readonly byte[] emptyArray = new byte[0];
    // Use this for initialization
    void Start()
    {
        if (!Client.IsHost)
        {
            this.enabled = false;
            return;
        }

        waitForFrame = new WaitForEndOfFrame();
    }
    private void Update()
    {
        if (AllSpawnPointsOutsideView.Elements.Count > 0 && coroutineOver && CurrentEnemyCount < EnemyCountForBigSquadSpawn)
        {
            spawnPos = AllSpawnPointsOutsideView[Random.Range(0, AllSpawnPointsOutsideView.Elements.Count)];
            waveCount = Random.Range(MinLittleSquadSpawn, MaxLittleSquadSpawn) * BigSquadSpawnMultiplier;
            StartCoroutine(SpawnEnemyWave());
        }
        if (!continuousCoroutineStarted)
        {
            continuousCoroutineStarted = true;
            StartCoroutine(SpawnEnemyContinuous());
        }
    }

    void OnEnable()
    {
        if (!Client.IsHost)
        {
            this.enabled = false;
            return;
        }
        continuousCoroutineStarted = false;
        waitForSeconds = new WaitForSeconds(TimeForLittleSquadSpawn);
        spawnPos = NearestSpawnPointOutsideView;
        coroutineOver = true;
    }
    private IEnumerator SpawnEnemyContinuous()
    {
        while (true)
        {
            yield return waitForSeconds;
            int length = Random.Range(MinLittleSquadSpawn, MaxLittleSquadSpawn);
            for (int i = 0; i < length; i++)
            {
                InstantiateEnemy(EnemyType.Normal, NearestSpawnPointOutsideView);
            }
        }
    }
    private IEnumerator SpawnEnemyWave()
    {
        coroutineOver = false;
        for (int i = 0; i < waveCount; i++)
        {
            yield return waitForFrame;
            InstantiateEnemy(EnemyType.Normal, spawnPos);
        }
        coroutineOver = true;
    }

    //sends to clients the command to instantiate an enemy in a given position, or it takes a random position from an array of randomic given positions if none is specified
    public void InstantiateEnemy(EnemyType type, Vector3 position)
    {
        if (CurrentEnemyCount >= MaxEnemyCount)
            return;

        int Id = IdDispenser.GetNewNetId();

        idAndPos.CurrentLength = 0;
        idAndPos.CurrentSeek = 0;

        idAndPos.Write((byte)type);
        idAndPos.Write(Id);
        idAndPos.Write(position.x);
        idAndPos.Write(position.y);
        idAndPos.Write(position.z);

        Client.SendPacketToInGameUsers(idAndPos.Data, 0, idAndPos.MaxCapacity, PacketType.EnemySpawn, Client.MyID, Steamworks.EP2PSend.k_EP2PSendReliable);
        //Debug.Log("sent: " + Id);
    }

    //this won't be in this cass, is just for testing
    //will be managed differently: client will send data when enemy is hit, host will decrease life and send datas back for clients to update enemies lives
    //TO DELETE
    bool ControlUsersStatus()
    {
        int inGameCount = 0;

        for (int i = 0; i < Client.Users.Count; i++)
        {
            string data = SteamMatchmaking.GetLobbyMemberData(Client.Lobby.LobbyID, Client.Users[i].SteamID, "InGame");
            if (data == "")
                continue;
            int index;
            if (int.TryParse(data, out index))
            {
                if (index == 1)
                    inGameCount++;
            }
        }

        if (inGameCount == Client.Users.Count)
            return true;

        return false;
    }
}
