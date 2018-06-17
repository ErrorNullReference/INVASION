using UnityEngine;
using SOPRO;
public class HealthPowUp : PowerUp
{
    public SOVariableFloat RecoverLifeAmount;
    protected override bool OnTriggerActive(Collider collision, Player collided)
    {
        collided.Life += RecoverLifeAmount;
        return true;
    }
}