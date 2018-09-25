using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SOPRO;
using Steamworks;
using GENUtility;

public class BonusPointsSpawner : MonoBehaviour
{
    public SOListVector3Container SpawnPoints;
    public NetIdDispenser IdDispenser;
    public PowerUpsMgr Manager;
    public float SpawnTime = 10f;
	public int PointsToAdd = 100;

    //public float BonusTime = 10f;

    private float timer = 0f;
    //private float bonusTimer = 0f;
    //bool bonusActive;

    void Start()
    {
        Client.AddCommand(PacketType.BonusPoints, UpdateBonusPoints);
    }

    private void OnEnable()
    {
        if (!Client.IsHost)
            this.enabled = false;
        timer = 0f;
        //bonusTimer = 0;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > SpawnTime)
        {
            timer = 0f;

            Vector3 closest = SpawnPoints[UnityEngine.Random.Range(0, SpawnPoints.Elements.Count)];

            Manager.SendMsgSpawnPowerUp(PowerUpType.Points, IdDispenser.GetNewNetId(), closest, null, true);
        }

        /*if (ClientEnemyStatsMgr.BonuPointsMult != 1)
        {
            bonusTimer += Time.deltaTime;
            if (bonusTimer >= BonusTime)
            {
                ClientEnemyStatsMgr.ResetBonusPoints();
                bonusTimer = 0;
            }
        }
        else
            bonusTimer = 0;*/
    }

    void UpdateBonusPoints(byte[] data, uint dataLength, CSteamID sender)
    {
        //ClientEnemyStatsMgr.BonuPointsMult = (int)data[0];

		CSteamID id = (CSteamID)System.BitConverter.ToUInt64 (data, 0);
		if (PlayersMgr.Players.ContainsKey (id))
			PlayersMgr.Players [id].Player.TotalPoints += PointsToAdd;
    }
}