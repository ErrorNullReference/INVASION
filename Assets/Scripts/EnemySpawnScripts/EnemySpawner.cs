using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using Steamworks;
using UnityEngine.AI;
using GENUtility;
using SOPRO;
[CreateAssetMenu(menuName = "Network/EnemySpawner")]
public class EnemySpawner : ScriptableObject
{
    public SODictionaryTransformContainer netEntities;
    public SOPool[] EnemyPools;
    public GameObject PoolRoot;

    public BaseSOEvGameNetworkObject OnEnemyAddEvent;
    public BaseSOEvGameNetworkObject OnEnemyRemoveEvent;

    public SOVariableInt EnemiesCount;

    public NavMeshAreaMaskHolder Mask;

    private Transform poolRoot;

    public void Init()
    {
        Client.AddCommand(PacketType.EnemyDeath, OnEnemyDeath);
        Client.AddCommand(PacketType.EnemySpawn, InstantiateEnemy);
        EnemiesCount.Value = 0;
    }

    //InstantiateEnemy will be called when command EnemySpawn is received from host
    private void InstantiateEnemy(byte[] data, uint length, CSteamID senderId)
    {
        if (!poolRoot && PoolRoot)
        {
            poolRoot = GameObject.Instantiate(PoolRoot).transform;
            poolRoot.name = "Enemies Root";
        }

        int id = ByteManipulator.ReadInt32(data, 0);

        float positionX = ByteManipulator.ReadSingle(data, 4);
        float positionY = ByteManipulator.ReadSingle(data, 8);
        float positionZ = ByteManipulator.ReadSingle(data, 12);

        Vector3 position = new Vector3(positionX, positionY, positionZ);
        //TODO: spawn system similar to power up where each pool is identified by a byte-int which indicates that specific enemy. This is to avoid spawning different enemies on cllients
        SOPool pool = EnemyPools[UnityEngine.Random.Range(0, EnemyPools.Length)];

        bool parented;
        int nullObjsRemovedFromPool;
        GameObject go = pool.DirectGet(poolRoot, out nullObjsRemovedFromPool, out parented);

        Enemy enemy = go.GetComponent<Enemy>();
        enemy.Pool = pool;
        enemy.NetworkId.SetNetworkId(id);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, 1f, Mask))
            go.GetComponent<NavMeshAgent>().Warp(hit.position);
        else
            Debug.LogWarning("NavMesh point for enemy spawn not found , souce pos = " + position);

        go.SetActive(true);

        EnemiesCount.Value++;

        OnEnemyAddEvent.Raise(enemy.NetworkId);  
    }

    //OnEnemyDeath will be called when command EnemyDeath is received from host
    private void OnEnemyDeath(byte[] data, uint length, CSteamID senderId)
    {
        int Id = ByteManipulator.ReadInt32(data, 0);
        Enemy obj = netEntities[Id].GetComponent<Enemy>();

        if (!obj)
            throw new NullReferenceException("NetId does not correspond to an enemy");

        obj.Reset();
        obj.Pool.Recycle(obj.gameObject);

        EnemiesCount.Value--;

        OnEnemyRemoveEvent.Raise(obj.NetworkId);
    }
}