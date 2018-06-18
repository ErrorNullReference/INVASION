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
public class EnemySpawner : Factory<byte>
{
    public SODictionaryTransformContainer netEntities;
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
    protected override byte ExtractIdentifier(GameObject obj)
    {
        return (byte)obj.GetComponent<Enemy>().Type.Value;
    }
    //InstantiateEnemy will be called when command EnemySpawn is received from host
    private void InstantiateEnemy(byte[] data, uint length, CSteamID senderId)
    {
        if (!poolRoot && PoolRoot)
        {
            poolRoot = GameObject.Instantiate(PoolRoot).transform;
            poolRoot.name = "Enemies Root";
        }

        byte type = data[0];
        int id = ByteManipulator.ReadInt32(data, 1);

        float positionX = ByteManipulator.ReadSingle(data, 5);
        float positionY = ByteManipulator.ReadSingle(data, 9);
        float positionZ = ByteManipulator.ReadSingle(data, 13);

        Vector3 position = new Vector3(positionX, positionY, positionZ);

        SOPool pool = organizedPools[type];

        bool parented;
        int nullObjsRemovedFromPool;
        GameObject go = pool.DirectGet(poolRoot, out nullObjsRemovedFromPool, out parented);

        Enemy enemy = go.GetComponent<Enemy>();
        enemy.Pool = pool;
        enemy.NetObj.SetNetworkId(id);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, 1f, Mask))
            go.GetComponent<NavMeshAgent>().Warp(hit.position);
        else
            Debug.LogWarning("NavMesh point for enemy spawn not found , souce pos = " + position);

        go.SetActive(true);

        EnemiesCount.Value++;

        OnEnemyAddEvent.Raise(enemy.NetObj);  
    }

    //OnEnemyDeath will be called when command EnemyDeath is received from host
    private void OnEnemyDeath(byte[] data, uint length, CSteamID senderId)
    {
        int Id = ByteManipulator.ReadInt32(data, 0);

        Enemy obj = netEntities[Id].GetComponent<Enemy>();

        if (!obj)
            throw new NullReferenceException("NetId does not correspond to an enemy");

        obj.Pool.Recycle(obj.gameObject);

        EnemiesCount.Value--;

        OnEnemyRemoveEvent.Raise(obj.NetObj);
    }
}