using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEnemy : MonoBehaviour
{
	public int DamageId;
    ShootSystem SS;

    void Start()
    {
        SS = GetComponentInParent<ShootSystem>();
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.GetComponent<Enemy>() != null)
			SS.SendHitMessage(other.GetComponent<GameNetworkObject>().NetworkId, SS.gun.values.Damage[DamageId]);
    }
}
