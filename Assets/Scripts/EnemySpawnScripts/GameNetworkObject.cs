using UnityEngine;
using SOPRO;
public class GameNetworkObject : MonoBehaviour
{
    public int NetworkId { get { return this.netId; } }
    [SerializeField]
    private SODictionaryTransformContainer networkObjects;
    private int netId;
    public void SetNetworkId(int netId)
    {
        this.netId = netId;
        networkObjects.Elements.Add(this.netId, this.transform);
    }
    public void ResetNetworkId()
    {
        networkObjects.Elements.Remove(this.netId);
        this.netId = -1;
    }
}