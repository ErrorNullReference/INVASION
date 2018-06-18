using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SOPRO;
using GENUtility;
using System;
[RequireComponent(typeof(GameNetworkObject))]
public class Enemy : LivingBeing
{
    public SOVariableEnemyType Type;
    [NonSerialized]
    public SOPool Pool;
    private float HUDTimer;
    [SerializeField]
    private float HUDTimerShow;
    private GameNetworkObject networkId;
    HUDManager hudManager;
    Image healthImage;

    public GameNetworkObject NetObj
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
    private void Awake()
    {
        networkId = GetComponent<GameNetworkObject>();
    }
    private void OnEnable()
    {
        Life = Stats.MaxHealth;
    }

    private void OnDisable()
    {
        networkId.ResetNetworkId();
    }

    public void Reset()
    {
        Life = Stats.MaxHealth;
    }

    public override void Die()
    {
        if (!Client.IsHost)
            return;

        byte[] d = ArrayPool<byte>.Get(sizeof(int));
        ByteManipulator.Write(d, 0, networkId.NetworkId);

        Client.SendPacketToInGameUsers(d, 0, d.Length, PacketType.EnemyDeath, Steamworks.EP2PSend.k_EP2PSendReliable);

        ArrayPool<byte>.Recycle(d);
    }

    public override void DecreaseLife(float decrease)
    {
        healthImage.enabled = true;
        HUDTimer = HUDTimerShow;

        base.DecreaseLife(decrease);
    }

    private void Update()
    {
        if (HUDTimer > 0)
        {
            HUDTimer -= Time.deltaTime;
            if (HUDTimer <= 0)
            {
                healthImage.enabled = false;
            }
        }
    }
}