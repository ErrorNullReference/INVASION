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
    public SOListVector3Container SpawnPoints;
    public NetIdDispenser IdDispenser;
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

            Vector3 closest = SpawnPoints[UnityEngine.Random.Range(0, SpawnPoints.Elements.Count)];

            byte[] data = ArrayPool<byte>.Get(17);

            data[0] = (byte)PowerUpType.Health;

            ByteManipulator.Write(data, 1, IdDispenser.GetNewNetId());

            ByteManipulator.Write(data, 5, closest.x);
            ByteManipulator.Write(data, 9, closest.y);
            ByteManipulator.Write(data, 13, closest.z);

            Client.SendPacketToInGameUsers(data, 0, data.Length, PacketType.PowerUpSpawn, EP2PSend.k_EP2PSendReliable, true);

            ArrayPool<byte>.Recycle(data);
        }
    }
}