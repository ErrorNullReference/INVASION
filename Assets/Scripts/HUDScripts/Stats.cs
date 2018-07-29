using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName = "User Interface/Stats")]
public class Stats : ScriptableObject
{
    [SerializeField]
    protected float maxHealth;

    public float MaxHealth { get { return maxHealth; } }

    [SerializeField]
    private float inverseMaxHealth;

    public float InverseMaxHealth { get { return inverseMaxHealth; } }

    public Gradient HealthBarGradient;

    void OnValidate()
    {
        inverseMaxHealth = Mathf.Approximately(0f, maxHealth) ? 0f : 1f / maxHealth;
    }
}
