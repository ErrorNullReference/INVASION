using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using Steamworks;
using UnityEngine.AI;
using GENUtility;
public class EnemySpawner : MonoBehaviour
{

    public Pool<GameObject> enemies;
    public GameObject[] prefab;
    public GameObject poolRoot;
    public static EnemySpawner Instance;

    public delegate void OnEnemyOperation(GameNetworkObject gameNetworkObject);

    public static event OnEnemyOperation OnEnemyAddEvent;
    public static event OnEnemyOperation OnEnemyRemoveEvent;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        enemies = new Pool<GameObject>(prefab, EnemyCreation, poolRoot);

        //activeEnemyList = new List<Enemy>();
        enemies.AddToQueue(20, cb =>
            {
                cb.GetComponent<GameNetworkObject>().NetworkId = -1;
                cb.SetActive(false);
            });       
        Client.AddCommand(PacketType.EnemyDeath, OnEnemyDeath);
        Client.AddCommand(PacketType.EnemySpawn, InstantiateEnemy);
    }

    public GameObject EnemyCreation(GameObject obj, GameObject parent)
    {
        GameObject o = Instantiate(obj, parent.transform);
        //if (OnEnemyAddEvent != null)
        //    OnEnemyAddEvent.Invoke(o.GetComponent<GameNetworkObject>());
        return o;
    }
   
    //InstantiateEnemy will be called when command EnemySpawn is received from host
    private void InstantiateEnemy(byte[] data, uint length, CSteamID senderId)
    {       
        int Id = data[0];
        //Debug.Log("received: " + Id);
        float positionX = ByteManipulator.ReadSingle(data, 1);
        float positionY = ByteManipulator.ReadSingle(data, 5);
        float positionZ = ByteManipulator.ReadSingle(data, 9);
        Vector3 position = new Vector3(positionX, positionY, positionZ);
        GameObject enemy = enemies.Get((cb) =>
            {
                //function called on the instance of the object that will be returned with the pool.Get() method  
                GameNetworkObject NObj = cb.GetComponent<GameNetworkObject>();
                NObj.NetworkId = Id;
                //cb.transform.position = position;
                cb.GetComponent<NavMeshAgent>().Warp(position);
                cb.SetActive(true);
                cb.GetComponent<NavMeshAgent>().enabled = true;
                OnEnemyAddEvent.Invoke(NObj);
            });
        //if (!activeEnemyList.Contains(enemy.GetComponent<Enemy>()))
        //    enemiesToAdd.Add(enemy.GetComponent<Enemy>());        
    }

    //OnEnemyDeath will be called when command EnemyDeath is received from host
    private void OnEnemyDeath(byte[] data, uint length, CSteamID senderId)
    {
        int Id = data[0];
        //for (int i = 0; i < activeEnemyList.Count; i++)
        //{
        //    if (activeEnemyList[i].NetworkId == Id)
        //    {
        GameObject obj = ClientTransformManager.IdEnemies[Id].gameObject;                
        enemies.Recycle(obj, (cb) =>
            {
                //cb.GetComponent<GameNetworkObject>().NetworkId = -1;
                //cb.transform.position = Vector3.zero;
                //cb.GetComponent<NavMeshAgent>().Warp(Vector3.zero);
                cb.GetComponent<Enemy>().Reset();
                cb.GetComponent<NavMeshAgent>().enabled = false;
                cb.SetActive(false);
                OnEnemyRemoveEvent.Invoke(cb.GetComponent<GameNetworkObject>());
            });               
        //  }
        // }
    }
}


