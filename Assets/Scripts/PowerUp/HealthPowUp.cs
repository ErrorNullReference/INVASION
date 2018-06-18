using UnityEngine;
using SOPRO;
public class HealthPowUp : PowerUp
{
    public SOVariableFloat RecoverLifeAmount;
    protected override bool OnTriggerActive(Collider collision, Player collided)
    {
        collided.DecreaseLife(-RecoverLifeAmount);
        return true;
    }
}