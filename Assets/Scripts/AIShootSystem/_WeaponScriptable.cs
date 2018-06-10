using UnityEngine;
using SOPRO;
[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class _WeaponScriptable : ScriptableObject
{
    public float Range;
    public int Damage;
    [Tooltip("It decreases with time.fixedDeltaTime")]
    public float ActionTime;
    [Tooltip("Var used for check the ActionTime")]
    public float CoolDown;
    public SOPool AiProjectilePool;
}
