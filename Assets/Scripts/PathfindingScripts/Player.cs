using UnityEngine;
using Steamworks;
using SOPRO;
using GENUtility;
public class Player : LivingBeing
{
    private static readonly byte[] packet = new byte[12];
    public SimpleAvatar Avatar { get { return avatar; } }

    public int TotalPoints = 0;

    public bool Dead;

    public float MaxRessTime = 120f;
    public float MaxRessDistance = 10f;
    public float RessTimeMultiplier = 0.2f;
    public float RessHealthPercentage = 0.5f;

    public SOEvBool PlayerAliveStatusChanged;
    [HideInInspector]
    public Collider PlayerCollider;
    [SerializeField]
    private SOListPlayerContainer players;
    [SerializeField]
    private SimpleAvatar avatar;
    [SerializeField]
    private PlayerAnimatorController controller;

    private float timer;

    private float prevLife;

    private void Start()
    {
        GetComponentInChildren<HUDHealt>().InputAssetHUD = Stats;
        PlayerCollider = GetComponentInChildren<Collider>();
        life = Stats.MaxHealth;
        prevLife = life;
        Dead = life <= 0f;
    }
    private void OnEnable()
    {
        players.Elements.Add(this);
    }
    private void OnDisable()
    {
        players.Elements.Remove(this);
    }
    protected void LateUpdate()
    {
        if (!Client.IsHost)
            return;

        if (Dead)
        {
            int length = players.Elements.Count;
            for (int i = 0; i < length; i++)
            {
                Player other = players[i];
                if (other != this && (this.transform.position - other.transform.position).sqrMagnitude < MaxRessDistance * MaxRessDistance)
                {
                    this.life += RessTimeMultiplier * Time.deltaTime;
                    if (this.life >= 1f)
                        this.Resurrect(Stats.MaxHealth * RessHealthPercentage);
                    return;
                }
            }

            //if no player is near the dead player the death timer will proceed
            timer += Time.deltaTime;
            if (timer > MaxRessTime)
            {
                byte[] data = ArrayPool<byte>.Get(8);

                ByteManipulator.Write(data, 0, (ulong)avatar.UserInfo.SteamID);

                Client.SendPacketToInGameUsers(data, 0, data.Length, PacketType.PlayerDeath, EP2PSend.k_EP2PSendReliable, true);

                ArrayPool<byte>.Recycle(data);
            }

            return;
        }

        if (!Mathf.Approximately(prevLife, life))
        {
            life = Mathf.Min(life, Stats.MaxHealth);
            prevLife = life;

            ByteManipulator.Write(packet, 0, (ulong)avatar.UserInfo.SteamID);
            ByteManipulator.Write(packet, 8, life);

            if (life > 0f)
            {
                Client.SendPacketToInGameUsers(packet, 0, packet.Length, PacketType.PlayerStatus, EP2PSend.k_EP2PSendReliable, false);
            }
            else
            {
                Client.SendTransformToInGameUsers(packet, this.transform, EP2PSend.k_EP2PSendReliable, false);
                Die();
            }
        }
    }
    public override void Die()
    {
        Dead = true;

        controller.Die(true);

        life = 0f;
        timer = 0f;

        if (avatar.UserInfo.SteamID == Client.MyID)
            PlayerAliveStatusChanged.Raise(false);
    }
    /// <summary>
    /// Requires Life value to be already setted
    /// </summary>
    public void Resurrect(float newLife)
    {
        life = Mathf.Min(newLife, Stats.MaxHealth);
        if (life <= 0f)
            return;

        Dead = false;

        controller.Die(false);

        if (avatar.UserInfo.SteamID == Client.MyID)
            PlayerAliveStatusChanged.Raise(true);
    }


    //CHANGED, FOR NOW ONLY ONE CAMERA WILL BE USED.

    //public Camera[] cameras;
    //private int currentCameraIndex;

    //void Start()
    //{
    //    currentCameraIndex = 0;
    //    for (int i = 0; i < cameras.Length; i++)
    //    {
    //        cameras[i].gameObject.SetActive(false);
    //    }
    //    if (cameras.Length > 0)
    //    {
    //        cameras[0].gameObject.SetActive(true);
    //    }
    //}

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.R))
    //    {
    //        currentCameraIndex++;

    //        if (currentCameraIndex < cameras.Length)
    //        {
    //            cameras[currentCameraIndex - 1].gameObject.SetActive(false);
    //            cameras[currentCameraIndex].gameObject.SetActive(true);
    //        }
    //        else
    //        {
    //            cameras[currentCameraIndex - 1].gameObject.SetActive(false);
    //            currentCameraIndex = 0;
    //            cameras[currentCameraIndex].gameObject.SetActive(true);
    //        }
    //    }
    //}
}