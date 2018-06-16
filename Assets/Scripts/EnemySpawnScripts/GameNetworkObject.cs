using UnityEngine;
using SOPRO;
public class GameNetworkObject : MonoBehaviour
{
    public int NetworkId { get { return this.netId; } }
    public bool Initialized { get; private set; }
    [SerializeField]
    private SODictionaryTransformContainer networkObjects;
    private int netId;
    public void SetNetworkId(int netId)
    {
        this.netId = netId;
        networkObjects.Elements.Add(this.netId, this.transform);
        Initialized = true;
    }
    public void ResetNetworkId()
    {
        networkObjects.Elements.Remove(this.netId);
        this.netId = -1;
        Initialized = false;
    }
}