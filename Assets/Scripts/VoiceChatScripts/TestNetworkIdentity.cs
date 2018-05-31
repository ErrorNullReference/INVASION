using UnityEngine;
public class TestNetworkIdentity : MonoBehaviour, INetworkIdentity
{
    [SerializeField]
    private ulong networkId;
    
    public ulong NetworkId { get { return networkId; } set { networkId = value; } }
    public bool IsLocalPlayer { get; set; }
}