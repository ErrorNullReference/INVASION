using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Steamworks;

public class ClientTransformManager : MonoBehaviour
{
    //public static List<GameNetworkObject> enemiesInScene;
    public static ClientTransformManager Instance;
    public static Dictionary<int, GameNetworkObject> IdEnemies;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        else Instance = this;
        //enemiesInScene = new List<GameNetworkObject>();
        IdEnemies = new Dictionary<int, GameNetworkObject>();
        EnemySpawner.OnEnemyAddEvent += AddEnemyInScene;
        EnemySpawner.OnEnemyRemoveEvent += RemoveEnemyFromScene;
        RegisterTransformCommand();

    }

    public void AddEnemyInScene(GameNetworkObject toAdd)
    {
        //enemiesInScene.Add(toAdd);
        IdEnemies.Add(toAdd.NetworkId, toAdd);
    }

    public void RemoveEnemyFromScene(GameNetworkObject toRemove)
    {
        IdEnemies.Remove(toRemove.NetworkId);
    }

    private void RegisterTransformCommand()
    {
        Client.AddCommand(PacketType.EnemyTransform, EnemyTransformReceive);
    }

    private void EnemyTransformReceive(byte[] data, uint dataLength, CSteamID sender)
    {
        int index = 0;

        int id = data[index++];

        float x = BitConverter.ToSingle(data, index);
        index += sizeof(float);
        float y = BitConverter.ToSingle(data, index);
        index += sizeof(float);
        float z = BitConverter.ToSingle(data, index);
        index += sizeof(float);

        Vector3 position = new Vector3(x, y, z);

        x = BitConverter.ToSingle(data, index);
        index += sizeof(float);
        y = BitConverter.ToSingle(data, index);
        index += sizeof(float);
        z = BitConverter.ToSingle(data, index);
        index += sizeof(float);
        float w = BitConverter.ToSingle(data, index);

        Quaternion rotation = new Quaternion(x, y, z, w);

        //for (int i = 0; i < enemiesInScene.Count; i++)
        //{
        //    if (id == enemiesInScene[i].NetworkId && enemiesInScene[i].gameObject.activeInHierarchy)
        //    {
        //        enemiesInScene[i].gameObject.GetComponent<MovementManager>().ReceiveTransform(position, rotation);
        //    }
        //}
        if (IdEnemies.ContainsKey(id))
            IdEnemies[id].GetComponent<EnemyTransformSync>().ReceiveTransform(position, rotation);
    }

    private void OnDestroy()
    {
        EnemySpawner.OnEnemyAddEvent -= AddEnemyInScene;
        EnemySpawner.OnEnemyRemoveEvent -= RemoveEnemyFromScene;
    }
}
