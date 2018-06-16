using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SOPRO;
using Steamworks;
using GENUtility;
public enum PowerUpType : byte
{
    Health = 0,
    Energy = 1
}

public class Timer
{
    public bool IsActive;

    public float CurrentTime;
    public float TimeLimit;

    public Timer(float timeLimit)
    {
        this.TimeLimit = timeLimit;
    }

    public void Update(float deltaTime)
    {
        CurrentTime += deltaTime;
    }

    public bool IsOver { get { return CurrentTime > TimeLimit; } }
}
[RequireComponent(typeof(GameNetworkObject))]
public class PowerUp : MonoBehaviour
{
    public PowerUpType Type;
    public int Value;
    public float LifeTime;
    [NonSerialized]
    public SOPool Pool;

    [SerializeField]
    private Collider coll;
    private GameNetworkObject netObj;

    private Timer timer;

    private void OnEnable()
    {
        if (!Client.IsHost)
        {
            coll.enabled = false;
            this.enabled = false;
        }
        coll.isTrigger = true;
        coll.enabled = true;

        if (!netObj)
            netObj = GetComponent<GameNetworkObject>();

        if (timer == null)
            timer = new Timer(LifeTime);

        timer.TimeLimit = LifeTime;
        timer.CurrentTime = 0;
    }

    private void Update()
    {
        timer.Update(Time.deltaTime);
        if (timer.IsOver)
        {
            timer.CurrentTime = 0f;
            //Recycle
            Recycle(default(CSteamID), false);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!netObj.Initialized)
            return;

        Player p = collision.gameObject.GetComponent<Player>();
        if (!p)
            return;

        if (Type == PowerUpType.Health)
        {
            Debug.Log("Before: " + p.Life);
            p.Life += Value;
        }
        Debug.Log("After : " + p.Life);

        //Manager call the recycle of this istance maybe
        Recycle(p.Avatar.UserInfo.SteamID, true);
    }
    public void Recycle()
    {
        Pool.Recycle(this.gameObject);
        netObj.ResetNetworkId();
    }
    private void Recycle(CSteamID picker, bool picked)
    {
        byte[] data = ArrayPool<byte>.Get(picked ? 12 : 4);
        ByteManipulator.Write(data, 0, netObj.NetworkId);
        if(picked)
            ByteManipulator.Write(data, 4, picker.m_SteamID);

        Client.SendPacketToInGameUsers(data, 0, data.Length, PacketType.PowerUpDespawn, EP2PSend.k_EP2PSendUnreliable, true);

        ArrayPool<byte>.Recycle(data);
    }
}
