using UnityEngine;

[RequireComponent(typeof(GameNetworkObject))]
public class EnemyShootSync : MonoBehaviour
{
    public void SendShotCall()
    {
        byte[] payload = new byte[] { (byte)this.GetComponent<GameNetworkObject>().NetworkId };
        Client.SendPacketToInGameUsers(payload, PacketType.ShootCall, Client.MyID, Steamworks.EP2PSend.k_EP2PSendUnreliable, false);
    }

    public void ReceiveShotCall() //USED BY SHOTMANAGER?
    {
        Debug.Log("Enemy: " + this.GetComponent<GameNetworkObject>().NetworkId + "just Shot");
    }
}