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
    private float HUDTimer, deadTimer;
    [SerializeField]
    private float HUDTimerShow;
    private GameNetworkObject networkId;
    HUDHealt hudManager;
    Image healthImage;
    public EnemyStats EnemyStats;
    bool dead;
    Animator animator;
    Brain brain;
    Collider collider;

    public Action OnDown, OnDeath;

    public GameNetworkObject NetObj
    {
        get
        {
            return networkId;
        }
    }

    private void Start()
    {
        if (hudManager == null)
            hudManager = GetComponentInChildren<HUDHealt>();
        healthImage = hudManager.GetComponent<Image>();
        healthImage.enabled = false;
        animator = GetComponent<Animator>();
        brain = GetComponent<Brain>();
    }

    private void Awake()
    {
        networkId = GetComponent<GameNetworkObject>();
    }

    public void Init(EnemyStats stats)
    {
        EnemyStats = stats;
        if (hudManager == null)
            hudManager = GetComponentInChildren<HUDHealt>();
        hudManager.InputAssetHUD = EnemyStats;
        life = EnemyStats.MaxHealth;
        dead = false;
        deadTimer = 0;
        if (collider == null)
            collider = GetComponent<Collider>();
        collider.enabled = true;
    }

    private void OnDisable()
    {
        networkId.ResetNetworkId();
    }

    public override void Die()
    {
        if (!Client.IsHost)
            return;

        byte[] d = ArrayPool<byte>.Get(sizeof(int));
        ByteManipulator.Write(d, 0, networkId.NetworkId);

        if (dead)
            Client.SendPacketToInGameUsers(d, 0, d.Length, PacketType.EnemyDeath, Steamworks.EP2PSend.k_EP2PSendReliable);
        else
            Client.SendPacketToInGameUsers(d, 0, d.Length, PacketType.EnemyDown, Steamworks.EP2PSend.k_EP2PSendReliable);

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

        if (dead && EnemyStats != null)
        {
            deadTimer += Time.deltaTime;
            if (deadTimer >= EnemyStats.DeathTime)
            {
                Die();
                if (OnDeath != null)
                    OnDeath.Invoke();
            }
        }
    }

    public void Down()
    {
        dead = true;
        deadTimer = 0;
        if (animator != null)
            animator.SetBool("Dead", dead);
        if (brain != null)
            brain.ShutDown();
        if (OnDown != null)
            OnDown.Invoke();
        collider.enabled = false;
    }
}
