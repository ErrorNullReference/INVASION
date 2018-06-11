using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Steamworks;

public class HostEnemySpawner : MonoBehaviour
{
    public static HostEnemySpawner Instance;
    int enemyId;
    public const int MAX_NUM_ENEMIES = 200;
    public Transform[] spawnPoints;
    public Transform firstWavePosition;
    int firstWaveCount;
    public int enemiesCount;
    private Dictionary<int, Enemy> IdEnemies;
    WaitForEndOfFrame waitForFrame;
    bool coroutineStart;

    // Use this for initialization
    void Start()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        else Instance = this;
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

        InitCoroutine(new LobbyDataUpdate_t());
    }

    void OnEnable()
    {
        coroutineStart = false;
    }

    private IEnumerator SpawnEnemiesAtStart()
    {
        for (int i = 0; i < firstWaveCount; i++)
        {
            InstantiateEnemy(-1, firstWavePosition.position.x, firstWavePosition.position.y, firstWavePosition.position.z);
            yield return waitForFrame;
        }
    }

    //sends to clients the command to instantiate an enemy in a given position, or it takes a random position from an array of randomic given positions if none is specified
    public void InstantiateEnemy(int EnemyId, float x = 0, float y = 0, float z = 0)
    {
        int Id = 0;
        if (EnemyId == -1)
        {
            Id = enemyId;
            enemyId++;
        }
        else
            Id = EnemyId;
        int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        Vector3 position = new Vector3(x, y, z);
        if (position == Vector3.zero)
        {
            position = spawnPoints[randomIndex].position;
        }
        byte[] positionData = GetBytePosition(position);
        byte[] data = MergeIdAndPosition(positionData, Id);
        Client.SendPacketToInGameUsers(data, PacketType.EnemySpawn, Client.MyID, Steamworks.EP2PSend.k_EP2PSendReliable);
        //Debug.Log("sent: " + Id);
    }

    //Creates the byte[] given the position
    private byte[] GetBytePosition(Vector3 position)
    {
        byte[] positionX = BitConverter.GetBytes(position.x);
        byte[] positionY = BitConverter.GetBytes(position.y);
        byte[] positionZ = BitConverter.GetBytes(position.z);
        byte[] positionData = new byte[positionX.Length + positionY.Length + positionZ.Length];
        Array.Copy(positionX, 0, positionData, 0, positionX.Length);
        Array.Copy(positionY, 0, positionData, positionX.Length, positionY.Length);
        Array.Copy(positionZ, 0, positionData, positionX.Length + positionY.Length, positionZ.Length);
        return positionData;
    }

    //creates a byte[] with Id and position
    private byte[] MergeIdAndPosition(byte[] positionData, int Id)
    {
        byte[] id = new byte[] { (byte)Id };
        byte[] data = new byte[positionData.Length + id.Length];
        Array.Copy(id, 0, data, 0, id.Length);
        Array.Copy(positionData, 0, data, id.Length, positionData.Length);
        return data;
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
        Client.SendPacketToInGameUsers(new byte[]{ }, PacketType.LeaveLobby, EP2PSend.k_EP2PSendReliable);
        StartCoroutine(SpawnEnemiesAtStart());  
        coroutineStart = true;
    }

    void InitCoroutine(LobbyChatUpdate_t cb)
    {
        if (coroutineStart || !ControlUsersStatus())
            return;

        Server.Init();
        Client.LeaveCurrentLobby();
        Client.SendPacketToInGameUsers(new byte[]{ }, PacketType.LeaveLobby, EP2PSend.k_EP2PSendReliable);
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
