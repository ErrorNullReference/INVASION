using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
public class _AIShoot : MonoBehaviour
{
    public _WeaponScriptable Weapon;
    public Transform Pivot;
    public LayerMaskHolder layer;

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
        if (Physics.Raycast(ray, out hitInfo, Weapon.Range, layer))
        {
            if (Weapon.ActionTime < Weapon.CoolDown)
            {
                if (hitInfo.collider.GetComponent<Player>())
                {
                    Instantiate(Weapon.AiProjectilePrefab, Pivot.position, Pivot.transform.rotation, null);
                }
            }
        }
        else
        {
            Weapon.ActionTime = tempActionTime;
        }
        Debug.Log(Weapon.ActionTime);
    }

    void OnDisable()
    {
        Weapon.ActionTime = tempActionTime;
    }
}
