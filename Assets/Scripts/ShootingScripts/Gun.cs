using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
public class Gun : MonoBehaviour
{
    public SOPool PrefabPool;

    public GunScriptable values;
    public Transform Muzzle;

    public float AmmoReady;
    public float AmmoInMagazine;
    public float MaxAmmoStored;

    public void Awake()
    {
        AmmoReady = values.CurrentAmmoInMag;
        AmmoInMagazine = values.MagCapacity;
        MaxAmmoStored = values.ComplessiveAmmoInMags;
    }

    public void Shoot()
    {
        PrefabPool.Get(null, Muzzle.position, Muzzle.rotation);
    }
}
