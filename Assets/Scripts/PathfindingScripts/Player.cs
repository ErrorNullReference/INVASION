using UnityEngine;
using Steamworks;
using SOPRO;
using GENUtility;

public class Player : LivingBeing
{
    private static readonly byte[] packet = new byte[12];

    public SimpleAvatar Avatar { get { return avatar; } }

    public int TotalPoints = 0;
    public int Energy;

    public bool Dead;

    public float MaxRessTime = 120f;
    public float MaxRessDistance = 10f;
    public float RessTimeMultiplier = 0.2f;
    public float RessHealthPercentage = 0.5f;

    public SOEvBool PlayerAliveStatusChanged;

    public Collider PlayerCollider { get { return playerCollider; } }

    [SerializeField]
    private Collider playerCollider;
    [SerializeField]
    private SOListPlayerContainer playersAlive;
    [SerializeField]
    private SOListPlayerContainer allPlayers;
    [SerializeField]
    private SimpleAvatar avatar;
    [SerializeField]
    private PlayerAnimatorController controller;

    private float timer;

    private float prevLife;

    private void Awake()
    {
        for (int i = playersAlive.Elements.Count - 1; i >= 0; i--)
        {
            if (!playersAlive[i])
                playersAlive.Elements.RemoveAt(i);
        }
        for (int i = allPlayers.Elements.Count - 1; i >= 0; i--)
        {
            if (!allPlayers[i])
                allPlayers.Elements.RemoveAt(i);
        }
    }

    private void Start()
    {
        GetComponentInChildren<HUDHealt>().InputAssetHUD = Stats;
        life = Stats.MaxHealth;
        prevLife = life;
        Dead = life <= 0f;
        Energy = 0;

        if (!Dead)
            playersAlive.Elements.Add(this);
    }

    private void OnEnable()
    {
        allPlayers.Elements.Add(this);
    }

    private void OnDisable()
    {
        allPlayers.Elements.Remove(this);
    }

    protected void LateUpdate()
    {
        if (!Client.IsHost)
            return;

        //Death logic
        if (Dead)
        {
            int length = playersAlive.Elements.Count;
            Transform transf = transform;
            bool near = false;

            for (int i = 0; i < length; i++)
            {
                Player other = playersAlive[i];
                if (other != this && (transf.position - other.transform.position).sqrMagnitude < MaxRessDistance * MaxRessDistance)
                {
                    this.life += RessTimeMultiplier * Stats.MaxHealth * Time.deltaTime;
                    if (this.life >= Stats.MaxHealth)
                        this.Resurrect(Stats.MaxHealth * RessHealthPercentage);
                    near = true;
                    break;
                }
            }

            //if no player is near the dead player the death timer will proceed
            if (!near)
            {
                timer += Time.deltaTime;
                if (timer > MaxRessTime)
                {
                    byte[] data = ArrayPool<byte>.Get(8);

                    ByteManipulator.Write(data, 0, (ulong)avatar.UserInfo.SteamID);

                    Client.SendPacketToInGameUsers(data, 0, data.Length, PacketType.PlayerDeath, EP2PSend.k_EP2PSendReliable, true);

                    ArrayPool<byte>.Recycle(data);
                }
            }
        }

        //Alive logic
        Energy = Mathf.Min(Energy, Stats.MaxEnergy);

        if (!Mathf.Approximately(prevLife, life))
        {
            life = Mathf.Min(life, Stats.MaxHealth);
            prevLife = life;

            ByteManipulator.Write(packet, 0, (ulong)avatar.UserInfo.SteamID);
            ByteManipulator.Write(packet, 8, life);

            Client.SendPacketToInGameUsers(packet, 0, packet.Length, PacketType.PlayerStatus, EP2PSend.k_EP2PSendReliable, false);
            if (life <= 0f && !Dead)
            {
                Die();
                Client.SendTransformToInGameUsers(packet, this.transform, EP2PSend.k_EP2PSendReliable, false);
            }
        }
    }

    public override void Die()
    {
        Dead = true;

        controller.Die(true);

        life = 0f;
        timer = 0f;

        playersAlive.Elements.Remove(this);

        if (avatar.UserInfo.SteamID == Client.MyID)
            PlayerAliveStatusChanged.Raise(false);

        if (!Client.IsHost)
            return;
        ByteManipulator.Write(packet, 0, (ulong)avatar.UserInfo.SteamID);
        ByteManipulator.Write(packet, 8, life);
        Client.SendPacketToInGameUsers(packet, 0, packet.Length, PacketType.PlayerStatus, EP2PSend.k_EP2PSendReliable, false);
    }

    public void Resurrect(float newLife)
    {
        life = Mathf.Min(newLife, Stats.MaxHealth);
        if (life <= 0f)
            return;

        Dead = false;

        controller.Die(false);

        playersAlive.Elements.Add(this);

        RessHealthPercentage *= 0.5f;
        if (RessHealthPercentage < 0.1f)
            RessHealthPercentage = 0.1f;

        if (avatar.UserInfo.SteamID == Client.MyID)
            PlayerAliveStatusChanged.Raise(true);
    }
}