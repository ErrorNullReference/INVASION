using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingBeing : MonoBehaviour {

    [SerializeField]
    private float life;
    public HeadsUpDisplay Stats;
    public float Life
    {
        get
        {
            return life;
        }
        set
        {
            life = value;
        }
    }	
}
