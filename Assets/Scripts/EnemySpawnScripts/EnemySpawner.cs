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

    public SOVariableInt EnemiesCount;

    public NavMeshAreaMaskHolder Mask;

    public SOListGenericContainer EnemyStatsPool;

    public EnemyInitializer[] EnemyInitializers;

    private Transform poolRoot;

    public void Init()
    {
        Client.AddCommand(PacketType.EnemyDeath, OnEnemyDeath);
        Client.AddCommand(PacketType.EnemyDown, OnEnemyDown);
        Client.AddCommand(PacketType.EnemySpawn, InstantiateEnemy);
        EnemiesCount.Value = 0;

        for (int i = 0; i < EnemyInitializers.Length; i++)
            EnemyInitializers[i].InitInstances();
    }

    protected override byte ExtractIdentifier(GameObject obj, int i)
    {
        return (byte)i;
        //return (byte)obj.GetComponent<Enemy>().Type.Value;
    }

    //InstantiateEnemy will be called when command EnemySpawn is received from host
    private void InstantiateEnemy(byte[] data, uint length, CSteamID senderId)
    {
        if (!poolRoot && PoolRoot)
        {
            poolRoot = GameObject.Instantiate(PoolRoot).transform;
            poolRoot.name = "Enemies Root";
            poolRoot.gameObject.AddComponent<ObjectsRegister>().Obj = this;
        }

        byte type = data[0];
        int id = ByteManipulator.ReadInt32(data, 1);

        float positionX = ByteManipulator.ReadSingle(data, 5);
        float positionY = ByteManipulator.ReadSingle(data, 9);
        float positionZ = ByteManipulator.ReadSingle(data, 13);

        Vector3 position = new Vector3(positionX, positionY, positionZ);

        SOPool pool = organizedPools[(int)EnemyType.Normal];
        //SOPool pool = organizedPools[type];

        bool parented;
        int nullObjsRemovedFromPool;
        GameObject go = pool.DirectGet(poolRoot, position, Quaternion.identity, out nullObjsRemovedFromPool, out parented);

        Enemy enemy = go.GetComponent<Enemy>();
        enemy.Pool = pool;
        enemy.NetObj.SetNetworkId(id);
        enemy.Initializer = EnemyInitializers[(int)type];
        enemy.Init((EnemyStats)EnemyStatsPool.Elements[0]); //[type]

        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, 1f, Mask))
            go.GetComponent<NavMeshAgent>().Warp(hit.position);

        go.SetActive(true);

        EnemiesCount.Value++;
    }

    //OnEnemyDeath will be called when command EnemyDeath is received from host
    private void OnEnemyDeath(byte[] data, uint length, CSteamID senderId)
    {
        int Id = ByteManipulator.ReadInt32(data, 0);

        if (Id >= netEntities.Elements.Count)
            return;

        if (!netEntities.Elements.ContainsKey(Id))
            return;

        Enemy obj = netEntities[Id].GetComponent<Enemy>();

        if (!obj)
            throw new NullReferenceException("NetId does not correspond to an enemy");

        obj.Pool.Recycle(obj.gameObject);

        EnemiesCount.Value--;
    }

    private void OnEnemyDown(byte[] data, uint length, CSteamID senderId)
    {
        int Id = ByteManipulator.ReadInt32(data, 0);

        if (Id >= netEntities.Elements.Count)
            return;

        if (!netEntities.Elements.ContainsKey(Id))
            return;

        Enemy obj = netEntities[Id].GetComponent<Enemy>();

        if (!obj)
            throw new NullReferenceException("NetId does not correspond to an enemy");

        obj.Down();
    }
}