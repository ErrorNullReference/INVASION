using UnityEngine;
using SOPRO;

public abstract class AIVision : AIBehaviour
{
    protected Transform currentTarget;

    public Transform CurrentTarget { get { return currentTarget; } }
}

public class AISphericalVision : AIVision
{
    [SerializeField]
    protected float maxViewDistance;

    [SerializeField]
    protected SOListPlayerContainer players;

    [SerializeField]
    protected AIBehaviour next;

    private void Update()
    {
        currentTarget = null;

        LookForATarget();

        if (currentTarget != null)
            owner.SwitchState(next);
    }

    public override void OnStateEnter()
    {
        if (!Client.IsHost)
        {
            owner.SwitchState(next);
            return;
        }
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
                currentTarget = p.transform;
                break;
            }
        }
    }
}