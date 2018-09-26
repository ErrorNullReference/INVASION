using UnityEngine;
using System;
using SOPRO;
using Steamworks;
using GENUtility;

[RequireComponent(typeof(GameNetworkObject))]
public abstract class PowerUp : MonoBehaviour
{
    public SOVariablePowerUpType Type;

    public SOVariableFloat LifeTime;

    public LayerHolder SelfLayer;

    [NonSerialized]
    public SOPool Pool;

    [SerializeField]
    private Collider coll;
    private GameNetworkObject netObj;

    private float timer;

    public void Recycle()
    {
        Pool.Recycle(this.gameObject);
        netObj.ResetNetworkId();
    }

    public abstract void ProcessAdditionalData(byte[] data, int startIndex, int length);

    protected virtual void OnEnable()
    {
        if (!netObj)
            netObj = GetComponent<GameNetworkObject>();

        if (gameObject.layer != SelfLayer)
            gameObject.layer = SelfLayer;

        if (!Client.IsHost)
        {
            coll.enabled = false;
            this.enabled = false;
            return;
        }

        coll.isTrigger = true;
        coll.enabled = true;

        timer = 0f;
    }

    protected virtual void Update()
    {
        timer += Time.deltaTime;
        if (timer > LifeTime)
        {
            timer = 0f;
            //Recycle
            Recycle(default(CSteamID), false);
        }
    }

    /// <summary>
    /// Method called when there have been a valid collision
    /// </summary>
    /// <param name="collision">collision collider</param>
    /// <param name="collided">player collided</param>
    /// <returns>true to despawn power up, false to not despawn power up</returns>
    protected abstract bool OnTriggerActive(Collider collision, Player collided);

    protected void Recycle(CSteamID picker, bool picked)
    {
        byte[] data = ArrayPool<byte>.Get(picked ? 12 : 4);
        ByteManipulator.Write(data, 0, netObj.NetworkId);
        if (picked)
            ByteManipulator.Write(data, 4, picker.m_SteamID);

        Client.SendPacketToInGameUsers(data, 0, data.Length, PacketType.PowerUpDespawn, EP2PSend.k_EP2PSendReliable, true);

        ArrayPool<byte>.Recycle(data);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!netObj.Initialized)
            return;

        Player p = collision.gameObject.GetComponent<Player>();
        if (!p)
            return;

        if (OnTriggerActive(collision, p))
            Recycle(p.Avatar.UserInfo.SteamID, true);

		PowerUpSpawnMgr.FreePosition (transform.position);
    }
}