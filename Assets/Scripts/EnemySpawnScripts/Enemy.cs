using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public HeadsUpDisplay Stats;
    public bool Destroy;
    public bool Recycling;
    public float randomSpawnTimer;
    [SerializeField]
    private int networkId;

    public int NetworkId
    {
        get
        {
            return networkId;
        }
        
    }

    private void Start()
    {
        GetComponentInChildren<HUDManager>().InputAssetHUD = Stats;
    }
    //network id of object, needed to get object from list and update its transform
    public float Life;
    public Vector3 position;
//variable to save position for pathfinding. it does not affect gameobject position!

    private void OnEnable()
    {
        networkId = GetComponent<GameNetworkObject>().NetworkId;
        //Debug.Log("Spawn: " + networkId);
        randomSpawnTimer = Random.Range(0f, 5.0f);
        Stats.MaxHealth = 5;
        Life = Stats.MaxHealth;
        Destroy = false;
        Recycling = false;
    }

    private void Awake()
    {       
        randomSpawnTimer = Random.Range(0f, 5.0f);
        Destroy = false;
        Recycling = false;        
    }

    public void Reset()
    {
        Life = Stats.MaxHealth;
    }

    //this will be managed by the host to send enemies datas to players
    public void DestroyAndRecycle()
    {
        Destroy = true;
        Recycling = true;
    }

    //TO DELETE
    public void DecreaseLife()
    {
        Life--;
    }

   

}
