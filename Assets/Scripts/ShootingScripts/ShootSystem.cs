using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SOPRO;
using GENUtility;

public class ShootSystem : MonoBehaviour
{
    public Gun gun;
    public Muzzle muzzle;
    [SerializeField]
    private LayerMaskHolder mask;
    private RaycastHit raycastHit;
    private Ray ray;
    private float recoilTime;
    private static readonly byte[] emptyArray = new byte[0];

    SimpleAvatar avatar;

    private void Awake()
    {
        recoilTime = 0;
        raycastHit = new RaycastHit();
        avatar = GetComponent<SimpleAvatar>();
        ray = new Ray();
    }

    void CallShoot()
    {
        Shoot();
        SendShootToAll();
    }

    public void Shoot()
    {
        /*if (activateCallbacks)
        {
            //instanziate ray
            ray.origin = muzzle.transform.position;
            ray.direction = muzzle.transform.forward;

            //if it shot someone
            if (Physics.Raycast(ray, out raycastHit, gun.values.MaxDistance, mask))
            {
                GameNetworkObject obj = raycastHit.collider.gameObject.GetComponent<GameNetworkObject>();
                if (obj != null)
                    SendHitMessage(obj.NetworkId, gun.values.Damage);
            }
        }*/

        gun.Shoot();
    }

    void SendShootToAll()
    {
        if (Client.IsHost)
            Client.SendPacketToInGameUsers(emptyArray, 0, 0, PacketType.PlayerShoot, Steamworks.EP2PSend.k_EP2PSendReliable, false);
        else
            Client.SendPacketToHost(emptyArray, 0, 0, PacketType.PlayerShootServer, Steamworks.EP2PSend.k_EP2PSendReliable);
    }

    public void SendHitMessage(int id, float damage)
    {
        byte[] data = ArrayPool<byte>.Get(16);
        ByteManipulator.Write(data, 0, id);
        ByteManipulator.Write(data, 4, damage);
        ByteManipulator.Write(data, 8, (ulong)avatar.UserInfo.SteamID);

        if (Client.IsHost)
            Client.SendPacketToInGameUsers(data, 0, data.Length, PacketType.ShootHit, Steamworks.EP2PSend.k_EP2PSendReliable, true);
        else
            Client.SendPacketToHost(data, 0, data.Length, PacketType.ShootHitServer, Steamworks.EP2PSend.k_EP2PSendReliable);

        ArrayPool<byte>.Recycle(data);
    }

    void Update()
    {
        //different beheaviour with number
        recoilTime -= Time.deltaTime;
        switch (gun.values.GunSystem)
        {
        //single shoot
            case 0:
                if (Input.GetButtonDown("Fire1") && recoilTime <= 0)
                {
                    CallShoot();
                    recoilTime = gun.values.Rateo;
                }
                break;
        //multi shoot
            case 1:
                if (Input.GetButton("Fire1") && recoilTime <= 0)
                {
                    CallShoot();
                    recoilTime = gun.values.Rateo;
                }
                break;
        }
    }
}