using GENUtility;
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
        byte[] d = ArrayPool<byte>.Get(sizeof(int));

        ByteManipulator.Write(d, 0, gnOnject.NetworkId);

        Client.SendPacketToInGameUsers(d, 0, d.Length, PacketType.ShootCall, Client.MyID, Steamworks.EP2PSend.k_EP2PSendUnreliable, false);

        ArrayPool<byte>.Recycle(d);
    }

    public void ReceiveShotCall() //USED BY SHOTMANAGER?
    {
        // Debug.Log("Enemy: " + this.GetComponent<GameNetworkObject>().NetworkId + "just Shot");
    }
}