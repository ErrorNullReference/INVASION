using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_Stats", menuName = "User Interface/Enemy stats")]
public class EnemyStats : Stats
{
    public int Damage;
    public float DeathTime;
    public Color Color;
    public EnemyType Type;
}
