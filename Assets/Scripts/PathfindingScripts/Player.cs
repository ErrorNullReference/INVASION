using UnityEngine;
using Steamworks;
using SOPRO;
using GENUtility;
public class Player : LivingBeing
{
    private static readonly byte[] packet = new byte[12];
    public bool Dead; //TODO: da sostituire poi con animator bool property Dead
    [HideInInspector]
    public Collider PlayerCollider;
    [SerializeField]
    private SOListPlayerContainer players;
    [SerializeField]
    private SimpleAvatar avatar;
    [SerializeField]
    private MonoBehaviour[] onDeathDisable = new MonoBehaviour[0];

    private float prevLife;

    private void Start()
    {
        GetComponentInChildren<HUDManager>().InputAssetHUD = Stats;
        PlayerCollider = GetComponentInChildren<Collider>();
        Life = Stats.MaxHealth;
        prevLife = Life;
        Dead = Life <= 0f;
    }
    private void OnEnable()
    {
        players.Elements.Add(this);
    }
    private void OnDisable()
    {
        players.Elements.Remove(this);
    }
    protected void Update()
    {
        if (!Client.IsHost || Dead)
            return;

        if (!Mathf.Approximately(prevLife, Life))
        {
            Life = Mathf.Min(Life, Stats.MaxHealth);
            prevLife = Life;

            ByteManipulator.Write(packet, 0, (ulong)avatar.UserInfo.SteamID);
            ByteManipulator.Write(packet, 8, Life);

            if (Life > 0f)
            {
                Client.SendPacketToInGameUsers(packet, 0, packet.Length, PacketType.PlayerStatus, EP2PSend.k_EP2PSendUnreliable, false);
            }
            else
            {
                Client.SendTransformToInGameUsers(packet, this.transform, EP2PSend.k_EP2PSendUnreliable, false);
                Die();
            }
        }
    }
    public override void Die()
    {
        Dead = true;
        for (int i = 0; i < onDeathDisable.Length; i++)
        {
            onDeathDisable[i].enabled = false;
        }
    }
    public void Resurrect()
    {
        Dead = false;
        for (int i = 0; i < onDeathDisable.Length; i++)
        {
            onDeathDisable[i].enabled = true;
        }
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