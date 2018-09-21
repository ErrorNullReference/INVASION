using UnityEngine;
using SOPRO;

public class BonusPointsPowerUp : PowerUp
{
    public override void ProcessAdditionalData(byte[] data, int startIndex, int length)
    {
    }

    protected override bool OnTriggerActive(Collider collision, Player collided)
    {
        Client.SendPacketToInGameUsers(new byte[]{ (byte)2 }, 0, 1, PacketType.BonusPoints, Steamworks.EP2PSend.k_EP2PSendReliable);
        return true;
    }
}