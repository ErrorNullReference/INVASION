using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingBeing : MonoBehaviour
{
    public float Life;
    public HeadsUpDisplay Stats;
    public abstract void Die();
    public virtual void DecreaseLife(float damage = 1)
    {
        this.Life -= Mathf.Abs(damage);
        if (Life <= 0)
            Die();
    }
}
