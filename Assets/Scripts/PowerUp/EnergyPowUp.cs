using UnityEngine;
using SOPRO;
using System;
public class EnergyPowUp : PowerUp
{
    public SOVariableInt EnergyAmount;
    [NonSerialized]
    public ulong Owner;
    protected override bool OnTriggerActive(Collider collision, Player collided)
    {
        if(collided.Avatar.UserInfo.SteamID.m_SteamID == Owner)
        {
            collided.Energy += EnergyAmount;
            return true;
        }
        return false;
    }
}