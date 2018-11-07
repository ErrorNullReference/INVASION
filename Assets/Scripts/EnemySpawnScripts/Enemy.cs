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
    public EnemyInitializer Initializer;
    public Transform BodyRoot;
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
    public GameObject RadarIndicator;
    bool dead;

    public Animator animator { get; set; }

    public AnimationControllerScript animController { get; set; }

    Brain brain;
    Collider collider;
    Rigidbody body;
    bool init;
    int bodyIndex;
    Gun gun;

    public SoundEmitter DeathSound;

    public Action OnDown, OnDeath;

    public bool Active { get; private set; }

    public GameNetworkObject NetObj
    {
        get
        {
            return networkId;
        }
    }

    [SerializeField]
    bool CallDown, ForceInit;
    NetObjTransformSync trSync;

    void OnValidate()
    {
        if (CallDown)
        {
            if (OnDown != null)
                OnDown.Invoke();
            CallDown = false;
        }

        if (ForceInit)
        {
            ForceInit = false;
            if (BodyRoot.childCount > 0)
                Destroy(BodyRoot.GetChild(0).gameObject);
            if (Initializer != null)
                Initializer.Init(this, BodyRoot, ref bodyIndex);
        }
    }

    public void Shoot(uint id)
    {
        gun.Shoot(id);
    }

    public void StartInit()
    {
        hudManager = GetComponentInChildren<HUDHealt>();
        healthImage = hudManager.GetComponent<Image>();
        healthImage.enabled = false;
        brain = GetComponent<Brain>();
        body = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        networkId = GetComponent<GameNetworkObject>();
        Active = false;
        brain.Active = false;
        gun = GetComponentInChildren<Gun>();
        trSync = GetComponent<NetObjTransformSync>();
    }

    private void Awake()
    {
        //networkId = GetComponent<GameNetworkObject> ();
    }

    public void Init(EnemyStats stats)
    {
        if (Initializer != null)
            Initializer.Init(this, BodyRoot, ref bodyIndex);

        EnemyStats = stats;
        //if (hudManager == null)
        //    hudManager = GetComponentInChildren<HUDHealt>();
        hudManager.InputAssetHUD = EnemyStats;
        life = EnemyStats.MaxHealth;
        dead = false;

        if (animator != null)
            animator.applyRootMotion = false;

        deadTimer = 0;
        //if (collider == null)
        //	collider = GetComponent<Collider> ();
        collider.enabled = true;

        if (RadarIndicator != null)
            RadarIndicator.SetActive(true);

        brain.Active = true;
        if (gun != null)
            gun.Enable();
        if (trSync != null)
            trSync.Enable();
    }

    private void OnDisable()
    {
        if (Initializer != null)
            Initializer.Destroy(bodyIndex);
        networkId.ResetNetworkId();
        if (gun != null)
            gun.Disable();
        if (trSync != null)
            trSync.Disable();
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
        if (!Active)
            return;

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
        {
            animator.SetTrigger("Die");
            animator.SetFloat("Death", UnityEngine.Random.Range(1, 4));
            animator.applyRootMotion = true;
        }
        if (brain != null)
            brain.ShutDown();
        if (OnDown != null)
            OnDown.Invoke();
        collider.enabled = false;

        if (RadarIndicator != null)
            RadarIndicator.SetActive(false);

        if (DeathSound != null)
            DeathSound.EmitSound();
    }

    void FixedUpdate()
    {
        body.velocity = Vector3.zero;
        body.angularDrag = 0;
        body.angularVelocity = Vector3.zero;
    }

    public bool IsActive()
    {
        return Active;
    }

    public void Activate()
    {
        Active = true;
        brain.OnEnable();
    }

    public void Deactivate()
    {
        Active = false;
        OnDisable();
    }
}
