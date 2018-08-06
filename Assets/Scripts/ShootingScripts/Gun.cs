using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;

public class Gun : MonoBehaviour
{
    public ParticleSystem[] PS;
    public SOPool PrefabPool;

    public GunScriptable values;
    public Transform Muzzle;

    public float AmmoReady;
    public float AmmoInMagazine;
    public float MaxAmmoStored;

    SoundEmitter emitter;

    public void Awake()
    {
        AmmoReady = values.CurrentAmmoInMag;
        AmmoInMagazine = values.MagCapacity;
        MaxAmmoStored = values.ComplessiveAmmoInMags;

        emitter = GetComponent<SoundEmitter>();
    }

    public void Shoot()
    {
        if (PS != null)
        {
            for (int i = 0; i < PS.Length; i++)
                PS[i].Play();
        }
        else
        {
            int nullObjsRemovedFromPool;
            PrefabPool.Get(null, Muzzle.position, Muzzle.rotation, out nullObjsRemovedFromPool);
        }

        if (emitter)
            emitter.EmitSound();
    }
}
