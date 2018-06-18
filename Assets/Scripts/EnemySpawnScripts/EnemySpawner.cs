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

    public NavMeshAreaMaskHolder Mask;

    private Transform poolRoot;

    public void Init()
    {
        Client.AddCommand(PacketType.EnemyDeath, OnEnemyDeath);
        Client.AddCommand(PacketType.EnemySpawn, InstantiateEnemy);
    }

    //public GameObject EnemyCreation(GameObject obj, GameObject parent) //TODO: is this needed ? no one calls it
    //{
    //    GameObject o = Instantiate(obj, parent.transform);
    //    //if (OnEnemyAddEvent != null)
    //    //    OnEnemyAddEvent.Invoke(o.GetComponent<GameNetworkObject>());
    //    return o;
    //}

    //InstantiateEnemy will be called when command EnemySpawn is received from host
    private void InstantiateEnemy(byte[] data, uint length, CSteamID senderId)
    {
        if (!poolRoot && PoolRoot)
        {
            poolRoot = GameObject.Instantiate(PoolRoot).transform;
            poolRoot.name = "Enemies Root";
        }

        int Id = ByteManipulator.ReadInt32(data, 0);
        //Debug.Log("received: " + Id);
        float positionX = ByteManipulator.ReadSingle(data, 4);
        float positionY = ByteManipulator.ReadSingle(data, 8);
        float positionZ = ByteManipulator.ReadSingle(data, 12);
        Vector3 position = new Vector3(positionX, positionY, positionZ);

        SOPool pool = EnemyPools[UnityEngine.Random.Range(0, EnemyPools.Length)];

        bool parented;
        int nullObjsRemovedFromPool;
        GameObject enemy = pool.DirectGet(poolRoot, out nullObjsRemovedFromPool, out parented);

        enemy.GetComponent<Enemy>().Pool = pool;
        GameNetworkObject NObj = enemy.GetComponent<GameNetworkObject>();
        NObj.SetNetworkId(Id);
        //cb.transform.position = position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, 1f, Mask))
            enemy.GetComponent<NavMeshAgent>().Warp(hit.position);
        else
            Debug.LogWarning("NavMesh point for enemy spawn not found , souce pos = " + position);

        enemy.SetActive(true);
        OnEnemyAddEvent.Raise(NObj);
        //if (!activeEnemyList.Contains(enemy.GetComponent<Enemy>()))
        //    enemiesToAdd.Add(enemy.GetComponent<Enemy>());        
    }

    //OnEnemyDeath will be called when command EnemyDeath is received from host
    private void OnEnemyDeath(byte[] data, uint length, CSteamID senderId)
    {
        int Id = ByteManipulator.ReadInt32(data, 0);
        ulong killerId = (ulong)ByteManipulator.ReadInt64(data, 4);
        Enemy obj = netEntities[Id].GetComponent<Enemy>();
        if(killerId==(ulong)Client.MyID)
        {
            obj.DropEnergy();
        }
        if (!obj)
            throw new NullReferenceException("NetId does not correspond to an enemy");

        obj.Reset();
        obj.Pool.Recycle(obj.gameObject);
        OnEnemyRemoveEvent.Raise(obj.GetComponent<GameNetworkObject>());
    }

    
}