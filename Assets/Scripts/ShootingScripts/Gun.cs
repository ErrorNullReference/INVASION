using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;

public class Gun : MonoBehaviour
{
    public ParticleSystem PSMuzzle;
    public Vector3 OffsetMuzzle;
    public ParticleSystem PSProjectile;
    public Vector3 OffsetProjectile;
    public bool InstantiatePS = true;
    public SOPool PrefabPool;

    public GunScriptable values;
    public Transform Muzzle, Projectile;

    public float AmmoReady;
    public float AmmoInMagazine;
    public float MaxAmmoStored;

    SoundEmitter emitter;
    Root root;

    public void Awake()
    {
        AmmoReady = values.CurrentAmmoInMag;
        AmmoInMagazine = values.MagCapacity;
        MaxAmmoStored = values.ComplessiveAmmoInMags;

        emitter = GetComponent<SoundEmitter>();

        if (GetComponentInParent<Enemy>() != null)
            root = this.gameObject.GetComponentInParent<Enemy>().gameObject.AddComponent<Root>();

        if (Muzzle == null)
            Muzzle = root.GetComponentInChildren<Muzzle>() != null ? root.GetComponentInChildren<Muzzle>().transform : null;
        if (Projectile == null)
            Projectile = GetComponentInChildren<MuzzleProjectile>() != null ? GetComponentInChildren<MuzzleProjectile>().transform : null;

        if (InstantiatePS)
        {
            if (PSMuzzle != null)
                PSMuzzle = Instantiate(PSMuzzle, Muzzle);
            if (PSProjectile != null)
                PSProjectile = Instantiate(PSProjectile, Projectile);
        }
    }

    public void Shoot(uint shootType)
    {
        if (PSMuzzle != null)
            PSMuzzle.Play();
        if (PSProjectile != null)
            PSProjectile.Play();

        if (emitter)
            emitter.EmitSound();
    }
}
