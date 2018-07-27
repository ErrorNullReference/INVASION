using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingBeing : MonoBehaviour
{
    public float Life { get { return life; } }

    public float MaxLife { get { return Stats.MaxHealth; } }

    public HeadsUpDisplay Stats;
    protected float life;

    public abstract void Die();

    public virtual void DecreaseLife(float decreaseAmount)
    {
        if (life <= 0)
            return;
        
        float prev = this.life;
        this.life -= decreaseAmount;
        if (prev > 0f && life <= 0)
        {
            life = 0;
            Die();
        }
    }

    public virtual void SetLife(float life)
    {
        float prev = this.life;
        this.life = life;

        if (prev > 0f && this.life <= 0f)
            Die();
    }
}
