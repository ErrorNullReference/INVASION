using UnityEngine;
using SOPRO;

public class NukePowerUp : PowerUp
{
    public override void ProcessAdditionalData(byte[] data, int startIndex, int length)
    {
    }

    protected override bool OnTriggerActive(Collider collision, Player collided)
    {
		byte[] steamId = System.BitConverter.GetBytes ((ulong)collided.Avatar.UserInfo.SteamID);
		Client.SendPacketToInGameUsers(steamId, 0, 0, PacketType.Nuke, Steamworks.EP2PSend.k_EP2PSendReliable);
        return true;
    }
}