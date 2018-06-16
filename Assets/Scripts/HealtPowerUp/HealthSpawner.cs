using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SOPRO;
public class HealthSpawner : MonoBehaviour
{
    public SOPool HealthPool;
    public Transform PowerUpsParent;
    public SOListPlayerContainer Players;
    public SOListVector3Container SpawnPoints;
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
            float distance = float.MaxValue;

            for (int i = 0; i < length; i++)
            {
                Vector3 current = SpawnPoints[i];
                float currDistance = (current - Center).sqrMagnitude;
                if(currDistance < distance)
                {
                    distance = currDistance;
                    closest = current;
                }
            }

            int nullObjsRemoved;
            HealthPool.Get(PowerUpsParent, closest, Quaternion.identity, out nullObjsRemoved, true).GetComponent<PowerUp>().Pool = HealthPool;
        }
    }
}