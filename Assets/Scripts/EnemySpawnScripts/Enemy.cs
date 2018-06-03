using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : LivingBeing
{    
    public bool Destroy;
    public bool Recycling;
    public float randomSpawnTimer;   
    private float HUDTimer;
    [SerializeField]
    private float HUDTimerShow;
    [SerializeField]
    private int networkId;
    HUDManager hudManager;
    Image healthImage;


    public int NetworkId
    {
        get
        {
            return networkId;
        }        
    }

    private void Start()
    {
        hudManager = GetComponentInChildren<HUDManager>();
        healthImage = hudManager.GetComponent<Image>();
        healthImage.enabled = false;
        hudManager.InputAssetHUD = Stats;
    }  
    
    private void OnEnable()
    {
        networkId = GetComponent<GameNetworkObject>().NetworkId;
        randomSpawnTimer = Random.Range(0f, 5.0f);
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

    public void DestroyAndRecycle()
    {
        Destroy = true;
        Recycling = true;
    }

    public void DecreaseLife()
    {
        healthImage.enabled = true;
        HUDTimer = HUDTimerShow;
        Life--;
    }

    private void Update()
    {
        if(HUDTimer>0)
        {
            HUDTimer -= Time.deltaTime;
            if(HUDTimer<=0)
            {
                healthImage.enabled = false;
            }
        }
    }



}
