using UnityEngine;
[CreateAssetMenu(menuName = "Network/IdDipsenser")]
public class NetIdDispenser : ScriptableObject
{
    [SerializeField]
    protected int nextDispensedId = int.MinValue;
    public virtual int GetNewNetId()
    {
        int res = nextDispensedId;
        nextDispensedId = nextDispensedId == int.MaxValue ? int.MinValue : nextDispensedId + 1;
        return res;
    }
}