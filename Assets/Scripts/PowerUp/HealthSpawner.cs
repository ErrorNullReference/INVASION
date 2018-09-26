using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SOPRO;
using Steamworks;
using GENUtility;
public class HealthSpawner : MonoBehaviour
{
    public NetIdDispenser IdDispenser;
    public PowerUpsMgr Manager;
    public float SpawnTime = 10f;

    private float timer = 0f;
    private void OnEnable()
    {
        if (!Client.IsHost)
            this.enabled = false;
        timer = 0f;
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > SpawnTime)
        {
            timer = 0f;

			Vector3 closest = PowerUpSpawnMgr.GetPosition ();

            Manager.SendMsgSpawnPowerUp(PowerUpType.Health, IdDispenser.GetNewNetId(), closest, null, true);
        }
    }
}