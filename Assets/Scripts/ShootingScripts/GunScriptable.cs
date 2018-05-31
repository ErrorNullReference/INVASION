using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "GunValues", menuName = "GunAsset")]
public class GunScriptable : ScriptableObject
{
    //ammo
    //time to shoot
    public float Rateo;
    /// <summary>Number of ammo that the gun has left in the current magazine.
    /// </summary>
    [SerializeField]
    private float currentAmmoInMag;
    public float CurrentAmmoInMag { get { return currentAmmoInMag; } }
    /// <summary>The number of ammo that a Magazine can store.
    /// </summary>
    [SerializeField]
    private float magCapacity;
    public float MagCapacity { get { return magCapacity; } }
    /// <summary>All the ammo currently avaible to the gun.
    /// </summary>
    [SerializeField]
    private float complessiveAmmoInMags;
    public float ComplessiveAmmoInMags { get { return complessiveAmmoInMags; } }

    //range
    //muzzle error
    [SerializeField]
    private float range;
    public float Range { get { return range; } }
    //max projectile run
    [SerializeField]
    private float maxDistance;
    public float MaxDistance { get { return maxDistance; } }

    //speed
    //projectile speed
    [SerializeField]
    private float speed;

    public float Speed { get { return speed; } }

    //effect
    //shootEffect
    public ParticleSystem shootParticle;
    //wall effect
    public ParticleSystem wallParticle;
    //blood effect
    public ParticleSystem bloodParticles;

    //gun type
    [Range(0, 10)]
    [SerializeField]
    private int gunSystem;

    public int GunSystem { get { return gunSystem; } }

    //Damage
    [SerializeField]
    private float damage;

    public float Damage { get { return damage; } }
}
