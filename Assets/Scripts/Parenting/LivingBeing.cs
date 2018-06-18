using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingBeing : MonoBehaviour
{
    public float Life { get { return life; } }
    public HeadsUpDisplay Stats;
    protected float life;
    public abstract void Die();
    public virtual void DecreaseLife(float decreaseAmount)
    {
        this.life -= decreaseAmount;
        if (life <= 0)
            Die();
    }
    public virtual void SetLife(float life)
    {
        float prev = this.life;
        this.life = life;

        if (prev > 0f && this.life <= 0f)
            Die();
    }
}
