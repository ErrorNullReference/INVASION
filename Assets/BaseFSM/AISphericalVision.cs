using UnityEngine;
using SOPRO;
public abstract class AIVision : AIBehaviour
{
    protected GameObject currentTarget;
    public GameObject CurrentTarget { get { return currentTarget; } private set { } }

    public SOEvVoid OnSpottedTarget;

    public override void AIUpdate()
    {
        throw new System.NotImplementedException();
    }

    public override void OnStateEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void OnStateExit()
    {
    }
}

public class AISphericalVision : AIVision
{
    [SerializeField]
    protected LayerMaskHolder layerToLookInto;

    [SerializeField]
    protected float maxViewDistance;

    [SerializeField]
    protected SOListPlayerContainer players;

    public void Awake()
    {
        if (!Client.IsHost)
        {
            this.enabled = false;
            return;
        }
    }

    public override void AIUpdate()
    {
        currentTarget = null;

        LookForATarget();

        if (currentTarget != null)
            OnSpottedTarget.Raise();
    }

    public override void OnStateEnter()
    {
        currentTarget = null;
    }

    public override void OnStateExit()
    {
    }

    private void LookForATarget()
    {
        int length = players.Elements.Count;
        currentTarget = null;
        Vector3 pos = transform.position;
        for (int i = 0; i < length; i++)
        {
            Player p = players[i];
            if (Vector3.Distance(pos, p.transform.position) < maxViewDistance)
            {
                currentTarget = p.gameObject;
                break;
            }
        }
    }
}