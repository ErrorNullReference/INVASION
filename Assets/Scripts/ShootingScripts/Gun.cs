using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;

public class Gun : MonoBehaviour
{
    public ParticleSystem PSMuzzle;
    public Vector3 OffsetMuzzle;
    public ParticleSystem[] PSProjectile;
    public Vector3 OffsetProjectile;
    public bool InstantiatePS = true;
    public SOPool PrefabPool;

    public GunScriptable values;
    public Transform Muzzle, Projectile;

    public float AmmoReady;
    public float AmmoInMagazine;
    public float MaxAmmoStored;
    [SerializeField]
    SoundEmitter[] emitters;
    Root root;

    public void Awake()
    {
        AmmoReady = values.CurrentAmmoInMag;
        AmmoInMagazine = values.MagCapacity;
        MaxAmmoStored = values.ComplessiveAmmoInMags;

        if (GetComponentInParent<Enemy>() != null)
            root = this.gameObject.GetComponentInParent<Enemy>().gameObject.AddComponent<Root>();

        //if (Muzzle == null)
        //	Muzzle = root.GetComponentInChildren<Muzzle> () != null ? root.GetComponentInChildren<Muzzle> ().transform : null;
        if (Projectile == null)
            Projectile = GetComponentInChildren<MuzzleProjectile>() != null ? GetComponentInChildren<MuzzleProjectile>().transform : null;

        if (InstantiatePS)
        {
            if (PSMuzzle != null)
            {
                PSMuzzle = Instantiate(PSMuzzle, Muzzle);
                PSMuzzle.gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
            if (PSProjectile != null)
            {
                for (int i = 0; i < PSProjectile.Length; i++)
                    PSProjectile[i] = Instantiate(PSProjectile[i], Projectile);
            }
        }
    }

    public void Enable()
    {
        Muzzle = root.GetComponentInChildren<Muzzle>().transform;
        PSMuzzle.transform.SetParent(Muzzle);
        PSMuzzle.transform.localPosition = new Vector3(OffsetMuzzle.x * Muzzle.transform.forward.x, OffsetMuzzle.y * Muzzle.transform.forward.y, OffsetMuzzle.z * Muzzle.transform.forward.z);
        PSMuzzle.transform.localRotation = Quaternion.identity;
    }

    public void Disable()
    {
        if (PSMuzzle == null)
            return;
        PSMuzzle.transform.SetParent(null);
        PSMuzzle.transform.position = new Vector3(-1000, 0, 0);
    }

    public void Shoot(uint shootType)
    {
        if (PSMuzzle != null)
            PSMuzzle.Play();
        if (PSProjectile != null && shootType < PSProjectile.Length && PSProjectile[shootType] != null)
            PSProjectile[shootType].Play();

        if (emitters != null && shootType < emitters.Length && emitters[shootType] != null)
            emitters[shootType].EmitSound();
    }
}
