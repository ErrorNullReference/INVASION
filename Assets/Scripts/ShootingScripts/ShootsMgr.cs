using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Steamworks;
using GENUtility;

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

        Client.AddCommand(PacketType.PlayerShoot, ReceiveShoot);
        //Client.AddCommand(PacketType.ShootServer, ReceiveShootServer);
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
            RayPlus ray = rays[i];
            float distance = ray.Speed * Time.deltaTime;
            distance = ray.Distance + distance > ray.MaxDistance ? ray.MaxDistance - ray.Distance : distance;
            ray.Distance += distance;

            //if (Application.isEditor)
            //  Debug.DrawRay(rays[i].Ray.origin, rays[i].Ray.direction * distance, Color.red, 0.5f);

            if (Physics.Raycast(ray.Ray, out raycastHit, distance))
            {
                //if (Application.isEditor)
                //  Debug.Log(raycastHit.collider);

                if (ray.ActivateCallbacks)
                {
                    GameNetworkObject obj = raycastHit.collider.gameObject.GetComponent<GameNetworkObject>();
                    if (obj)
                        SendHitToHost(obj.NetworkId, ray.Damage, ray.Shooter);
                }
                removeRays.Add(ray);
            }
            if (ray.Distance >= ray.MaxDistance)
            {
                removeRays.Add(ray);
            }

            ray.Ray.origin += ray.Ray.direction * distance;
        }
        for (int j = 0; j < removeRays.Count; j++)
        {
            rays.Remove(removeRays[j]);
        }
        removeRays.Clear();
    }

    void SendHitToHost(int id, float damage, ulong shooter)
    {
        //byte[] data = ArrayPool<byte>.Get(16);

        //ByteManipulator.Write(data, 0, id);
        //ByteManipulator.Write(data, 4, damage);
        //ByteManipulator.Write(data, 8, shooter);

        //Client.SendPacketToHost(data, 0, data.Length, PacketType.ShootHitServer, Steamworks.EP2PSend.k_EP2PSendReliable);

        //ArrayPool<byte>.Recycle(data);

        //Debug.Log("hit");
    }

    void ReceiveShoot(byte[] data, uint lenght, CSteamID sender)
    {
        if (sender != Client.MyID)
            PlayersMgr.Players[sender].Shoot(false);
    }

    //void ReceiveShootServer(byte[] data, uint lenght, CSteamID sender)
    //{
    //    Client.SendPacketToInGameUsers(data, 0, (int)lenght, PacketType.Shoot, sender, EP2PSend.k_EP2PSendReliable, false);
    //    if (sender != Client.MyID)
    //        PlayersMgr.Players[sender].Shoot(false);
    //}

    public static void AddRay(RayPlus ray)
    {
        //Debug.Log("Added ray to shootmanager");
        Instance.rays.Add(ray);
    }

    void OnDestroy()
    {
        Instance = null;
    }
}

public class RayPlus
{
    public RayPlus(Vector3 position, Vector3 direction, float distance, float damage, float speed, float maxDistance, ulong shooter, bool activateCallbacks = false)
    {
        Ray = new Ray(position, direction);
        Distance = distance;
        Damage = damage;
        Speed = speed;
        MaxDistance = maxDistance;
        ActivateCallbacks = activateCallbacks;
        Shooter = shooter;
    }

    public ulong Shooter;
    public float Distance;
    public Ray Ray;
    public float Damage;
    public float Speed;
    public float MaxDistance;
    public bool ActivateCallbacks;
}
