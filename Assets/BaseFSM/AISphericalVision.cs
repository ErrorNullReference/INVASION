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
        Collider[] cols = Physics.OverlapSphere(this.gameObject.transform.position, maxViewDistance, layerToLookInto);
        currentTarget = cols.Length == 0 ? null : cols[UnityEngine.Random.Range(0, cols.Length)].gameObject;
    }
}