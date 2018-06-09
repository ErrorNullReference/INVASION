using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SOPRO;
public enum ShootingType
{
    Single,
    Consecutive,
}


public class ShootSystem : MonoBehaviour
{
    /*
     
    Read documentation
         
    */
    /// <summary>
    /// Kind of shooting
    /// </summary>
    //0: shoot in one frame - 1: shoot in some time
    [Tooltip("0: shoot in one frame - 1: shoot in some time")]
    public ShootingType shootingType;
    private float recoilTime;
    public Gun gun;
    public Muzzle muzzle;
    private RaycastHit raycastHit;
    private LayerMaskHolder mask;

    private void Awake()
    {
        recoilTime = 0;
        raycastHit = new RaycastHit();
    }

    void CallShoot()
    {
        /*if (Selector == 0)
        {
            Shoot0();
        }
        else if (Selector == 1)
        {
            Shoot1();
            PerformShoot();
        }*/

        Shoot(true);
        SendShootToHost();
    }

    public void Shoot(bool activateCallbacks = false)
    {
        //rotate muzzle transform
        float r = gun.values.Range;
        muzzle.transform.localRotation = Quaternion.Euler(UnityEngine.Random.Range(-r, r), 0, UnityEngine.Random.Range(-r, r));
        //instanziate ray
        Ray ray = new Ray(muzzle.transform.position, muzzle.transform.forward);

        //method for start shooting
        float distance = shootingType == ShootingType.Single ? gun.values.MaxDistance : gun.values.Speed * Time.deltaTime;

        //if (Application.isEditor)
        //  Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, 0.5f);

        //if it shot someone
        if (Physics.Raycast(ray, out raycastHit, distance))
        {
            //this method went call when 
            if (activateCallbacks)
            {
                GameNetworkObject obj = raycastHit.collider.gameObject.GetComponent<GameNetworkObject>();
                if (obj != null)
                    SendHitToHost(obj.NetworkId);
            }
        }
        else if (shootingType == ShootingType.Consecutive) // else add the ray in a list
        {
            ShootsMgr.AddRay(new RayPlus(ray.origin + ray.direction * distance, ray.direction, distance, gun.values.Damage, gun.values.Speed, gun.values.MaxDistance, activateCallbacks));
        }
        //set rotation on identity
        muzzle.transform.localRotation = Quaternion.identity;

        gun.Shoot();
    }

    void SendShootToHost()
    {
        Client.SendPacketToHost(new byte[]{ }, PacketType.ShootServer, Steamworks.EP2PSend.k_EP2PSendReliable);
    }

    void SendHitToHost(int id)
    {
        byte[] data = new byte[]{ (byte)id };

        Client.SendPacketToHost(data, PacketType.ShootHitServer, Steamworks.EP2PSend.k_EP2PSendReliable);

        //Debug.Log("hit");
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
            default:
                break;
        }
    }

    //if you want change gun
    //use this for set time to 0
    /// <summary>
    /// if you want change gun
    /// use this for set time to 0
    /// </summary>
//    public void SetTimeTo0()
//    {
//        recoilTime = 0;
//    }
}