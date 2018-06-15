using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Steamworks;
using GENUtility;

public class HostEnemySpawner : MonoBehaviour
{
    public static HostEnemySpawner Instance;
    int enemyId;
    public const int MAX_NUM_ENEMIES = 200;
    public NetIdDispenser IdDispenser;
    int firstWaveCount;
    public int enemiesCount;
    private Dictionary<int, Enemy> IdEnemies;
    WaitForEndOfFrame waitForFrame;
    bool coroutineStart;
    private static readonly BytePacket idAndPos = new BytePacket(16);
    private static readonly byte[] emptyArray = new byte[0];
    // Use this for initialization
    void Start()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        else
            Instance = this;
        if (!Client.IsHost)
        {
            this.enabled = false;
            return;
        }
        waitForFrame = new WaitForEndOfFrame();
        enemiesCount = 0;
        enemyId = 0;
        firstWaveCount = UnityEngine.Random.Range(1, 20);

        SteamCallbackReceiver.ChatUpdateEvent += InitCoroutine;
        SteamCallbackReceiver.LobbyDataUpdateEvent += InitCoroutine;

        //InitCoroutine(new LobbyDataUpdate_t());
    }

    void OnEnable()
    {
        coroutineStart = false;
    }

    private IEnumerator SpawnEnemiesAtStart()
    {
        for (int i = 0; i < firstWaveCount; i++)
        {
            InstantiateEnemy(EnemySpawnSystem.GetSpawnPoint());
            yield return waitForFrame;
        }
    }

    //sends to clients the command to instantiate an enemy in a given position, or it takes a random position from an array of randomic given positions if none is specified
    public void InstantiateEnemy(Vector3 position)
    {
        int Id = IdDispenser.GetNewNetId();

        if (position == Vector3.zero)
            position = EnemySpawnSystem.GetSpawnPoint();

        idAndPos.CurrentLength = 0;
        idAndPos.CurrentSeek = 0;

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

    void InitCoroutine(LobbyDataUpdate_t cb)
    {
        if (coroutineStart || !ControlUsersStatus())
            return;

        Server.Init();
        Client.LeaveCurrentLobby();
        Client.SendPacketToInGameUsers(emptyArray, 0, 0, PacketType.LeaveLobby, EP2PSend.k_EP2PSendReliable);
        StartCoroutine(SpawnEnemiesAtStart());
        coroutineStart = true;
    }

    void InitCoroutine(LobbyChatUpdate_t cb)
    {
        if (coroutineStart || !ControlUsersStatus())
            return;

        Server.Init();
        Client.LeaveCurrentLobby();
        Client.SendPacketToInGameUsers(emptyArray, 0, 0, PacketType.LeaveLobby, EP2PSend.k_EP2PSendReliable);
        StartCoroutine(SpawnEnemiesAtStart());
        coroutineStart = true;
    }

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
