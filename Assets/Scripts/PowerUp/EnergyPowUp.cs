using UnityEngine;
using SOPRO;
using System;
using GENUtility;
public class EnergyPowUp : PowerUp
{
    public SOVariableInt EnergyAmount;
    public Material PickableFromClient;
    public Material NonPickableFromClient;
    public Renderer Renderer;
    [NonSerialized]
    public ulong Owner;
    public override void ProcessAdditionalData(byte[] data, int startIndex, int length)
    {
        Owner = length > 0 ? ByteManipulator.ReadUInt64(data, startIndex) : 0;
        Renderer.material = Client.MyID.m_SteamID == Owner ? PickableFromClient : NonPickableFromClient;
    }
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