using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Steamworks;

public class ShootsMgr : MonoBehaviour
{
    [SerializeField]
    private List<RayPlus> rays;
    private List<RayPlus> removeRays;
    private RaycastHit raycastHit;

    static ShootsMgr Instance;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        rays = new List<RayPlus>();
        removeRays = new List<RayPlus>();

        Client.AddCommand(PacketType.Shoot, ReceiveShoot);
        Client.AddCommand(PacketType.ShootServer, ReceiveShootServer);
    }

    void Update()
    {
        PerformShoot();
    }

    public void PerformShoot()
    {
        UnityEngine.Random.InitState(Time.frameCount);
        for (int i = 0; i < rays.Count; i++)
        {
            float distance = rays[i].Speed * Time.deltaTime;
            distance = rays[i].Distance + distance > rays[i].MaxDistance ? rays[i].MaxDistance - rays[i].Distance : distance;
            rays[i].Distance += distance;

            //if (Application.isEditor)
            //  Debug.DrawRay(rays[i].Ray.origin, rays[i].Ray.direction * distance, Color.red, 0.5f);

            if (Physics.Raycast(rays[i].Ray, out raycastHit, distance))
            {
                //if (Application.isEditor)
                //  Debug.Log(raycastHit.collider);
                
                if (rays[i].ActivateCallbacks)
                {
                    GameNetworkObject obj = raycastHit.collider.gameObject.GetComponent<GameNetworkObject>();
                    if (obj != null)
                        SendHitToHost(obj.NetworkId);
                }
                removeRays.Add(rays[i]);
            }
            if (rays[i].Distance >= rays[i].MaxDistance)
            {
                removeRays.Add(rays[i]);
            }

            rays[i].Ray.origin += rays[i].Ray.direction * distance;
        }
        for (int j = 0; j < removeRays.Count; j++)
        {
            rays.Remove(removeRays[j]);
        }
        removeRays.Clear();
    }

    void SendHitToHost(int id)
    {
        byte[] data = new byte[]{ (byte)id };

        Client.SendPacketToHost(data, PacketType.ShootHitServer, Steamworks.EP2PSend.k_EP2PSendReliable);

        Debug.Log("hit");
    }

    void ReceiveShoot(byte[] data, uint lenght, CSteamID sender)
    {
        if (sender != Client.MyID)
            PlayersMgr.Players[sender].Shoot(false);
    }

    void ReceiveShootServer(byte[] data, uint lenght, CSteamID sender)
    {
        Client.SendPacketToInGameUsers(data, PacketType.Shoot, sender, EP2PSend.k_EP2PSendReliable, false);
        if (sender != Client.MyID)
            PlayersMgr.Players[sender].Shoot(false);
    }

    public static void AddRay(RayPlus ray)
    {
        Instance.rays.Add(ray);
    }
}

public class RayPlus
{
    public RayPlus(Vector3 position, Vector3 direction, float distance, float damage, float speed, float maxDistance, bool activateCallbacks = false)
    {
        Ray = new Ray(position, direction);
        Distance = distance;
        Damage = damage;
        Speed = speed;
        MaxDistance = maxDistance;
        ActivateCallbacks = activateCallbacks;
    }

    public float Distance;
    public Ray Ray;
    public float Damage;
    public float Speed;
    public float MaxDistance;
    public bool ActivateCallbacks;
}
