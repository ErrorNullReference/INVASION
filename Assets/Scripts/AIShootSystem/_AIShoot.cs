using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
public class _AIShoot : MonoBehaviour
{
    public _WeaponScriptable Weapon;
    public Transform Pivot;
    public SOListPlayerContainer players;

    private float tempActionTime;
    void Awake()
    {
        tempActionTime = Weapon.ActionTime;
    }

    void FixedUpdate()
    {
        ShotWithRaycast();
    }

    void ShotWithRaycast()
    {
        Ray ray = new Ray(Pivot.position, Pivot.forward);
        RaycastHit hitInfo;

        Weapon.ActionTime -= Time.fixedDeltaTime;
        Debug.DrawRay(ray.origin, ray.direction);

        int length = players.Elements.Count;
        bool bulletShot = false;
        for (int i = 0; i < length; i++)
        {
            Player p = players[i];
            if (p.PlayerCollider.Raycast(ray, out hitInfo, Weapon.Range))
            {
                if (Weapon.ActionTime < Weapon.CoolDown)
                {
                    int nullObjsRemovedFromPool;
                    Weapon.AiProjectilePool.Get(null, ray.origin, Pivot.rotation, out nullObjsRemovedFromPool);
                    bulletShot = true;
                    break;
                }
            }
        }
        if (bulletShot)
        {
            Weapon.ActionTime = tempActionTime;
        }
        //Debug.Log(Weapon.ActionTime);
    }

    void OnDisable()
    {
        Weapon.ActionTime = tempActionTime;
    }
}
