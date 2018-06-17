using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingBeing : MonoBehaviour
{
    public float Life;
    public HeadsUpDisplay Stats;
    public abstract void Die();
}
