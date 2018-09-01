using UnityEngine;
using SOPRO;

public class HealthPowUp : PowerUp
{
    public SOVariableFloat RecoverLifeAmount;

    public override void ProcessAdditionalData(byte[] data, int startIndex, int length)
    {
    }

    protected override bool OnTriggerActive(Collider collision, Player collided)
    {
        //collided.DecreaseLife(-RecoverLifeAmount);
        return true;
    }
}