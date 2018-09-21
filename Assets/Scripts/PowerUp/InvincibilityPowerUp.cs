using UnityEngine;
using SOPRO;

public class InvincibilityPowerUp : PowerUp
{
    public override void ProcessAdditionalData(byte[] data, int startIndex, int length)
    {
    }

    protected override bool OnTriggerActive(Collider collision, Player collided)
    {
        Client.SendPacket(new byte[0], 0, 0, PacketType.Invincibility, Client.Host, collided.Avatar.UserInfo.SteamID, Steamworks.EP2PSend.k_EP2PSendReliable);
        return true;
    }
}