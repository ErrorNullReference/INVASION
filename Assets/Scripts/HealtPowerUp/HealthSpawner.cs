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
    public PowerUpsMgr PowUpManager;
    public SOListPlayerContainer Players;
    public SOListVector3Container SpawnPoints;
    public NetIdDispenser IdDispenser;
    public float IdealDistanceSpawn = 10f;
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

            int length = Players.Elements.Count;
            Vector3 Center = default(Vector3);
            for (int i = 0; i < length; i++)
            {
                Center += Players[i].transform.position;
            }
            Center /= length;


            length = SpawnPoints.Elements.Count;

            Vector3 closest = default(Vector3);
            float distanceToIdeal = float.MaxValue;

            for (int i = 0; i < length; i++)
            {
                Vector3 current = SpawnPoints[i];
                float currDistance = Mathf.Abs((current - Center).sqrMagnitude - IdealDistanceSpawn);
                if (currDistance < distanceToIdeal)
                {
                    distanceToIdeal = currDistance;
                    closest = current;
                }
            }

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