using UnityEngine;

[RequireComponent(typeof(GameNetworkObject))]
public class EnemyShootSync : MonoBehaviour
{
    GameNetworkObject gnOnject;
    private void Start()
    {
        gnOnject = GetComponent<GameNetworkObject>();
    }
    public void SendShotCall()
    {
        byte[] d = ArrayPool<byte>.Get(1);

        d[0] = (byte)gnOnject.NetworkId;

        Client.SendPacketToInGameUsers(d, 0, 1, PacketType.ShootCall, Client.MyID, Steamworks.EP2PSend.k_EP2PSendUnreliable, false);

        ArrayPool<byte>.Recycle(d);
    }

    public void ReceiveShotCall() //USED BY SHOTMANAGER?
    {
        // Debug.Log("Enemy: " + this.GetComponent<GameNetworkObject>().NetworkId + "just Shot");
    }
}