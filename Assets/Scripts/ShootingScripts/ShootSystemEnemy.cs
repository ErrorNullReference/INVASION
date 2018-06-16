using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
using GENUtility;
public class ShootSystemEnemy : MonoBehaviour
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
    private static readonly byte[] emptyArray = new byte[0];

    private void Awake()
    {
        recoilTime = 0;
        raycastHit = new RaycastHit();
    }

    public void CallShoot(Vector3 objective)
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

        Shoot(objective, true);
        SendShootToHost();
    }

    public void Shoot(Vector3 objective, bool activateCallbacks = false)
    {
        //rotate muzzle transform
        float r = gun.values.Range;

        Vector3 dir = (objective - muzzle.transform.position).normalized;
        muzzle.transform.forward = dir;
        muzzle.transform.localRotation = Quaternion.Euler(UnityEngine.Random.Range(-r, r), 0, UnityEngine.Random.Range(-r, r));
        //instanziate ray
        Ray ray = new Ray(muzzle.transform.position, muzzle.transform.forward);

        //method for start shooting
        float distance = gun.values.MaxDistance;

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
        //set rotation on identity
        muzzle.transform.localRotation = Quaternion.identity;

        gun.Shoot();
    }

    void SendShootToHost()
    {
        Client.SendPacketToHost(emptyArray, 0, 0, PacketType.ShootServer, Steamworks.EP2PSend.k_EP2PSendReliable);
    }

    void SendHitToHost(int id)
    {
        byte[] data = ArrayPool<byte>.Get(sizeof(int));
        ByteManipulator.Write(data, 0, id);

        Client.SendPacketToHost(data, 0, data.Length, PacketType.ShootServer, Steamworks.EP2PSend.k_EP2PSendReliable);

        ArrayPool<byte>.Recycle(data);
        //Debug.Log("hit");
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
