using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using SOPRO;

[RequireComponent(typeof(NavMeshAgent))]
public class AIGoToTargetUntilOnSight : AIBehaviour
{
    [SerializeField]
    private LayerMaskHolder targetLayerMask;
    [SerializeField]
    private float maxViewDistance;
    [SerializeField]
    [Range(1, 360)]
    private int fov;
    [SerializeField]
    private float cooldownBeforeRecalculation;
    private float currentCooldownLeftBeforeRecalculation;

    private Transform target;

    public Transform Target { get { return this.target; } }

    private NavMeshAgent agent;
    private AnimationControllerScript animController;

    [SerializeField]
    private AIBehaviour targetVisible;
    [SerializeField]
    private AIBehaviour targetLost;

    protected override void Awake()
    {
        base.Awake();
        agent = this.GetComponent<NavMeshAgent>();
        animController = this.GetComponent<AnimationControllerScript>();

        Client.OnUserDisconnected += (id) =>
        {
            target = null;
        };
    }

    private void Update()
    {
        currentCooldownLeftBeforeRecalculation -= Time.deltaTime;
        if (target != null && currentCooldownLeftBeforeRecalculation <= 0)
            this.agent.SetDestination(target.position);

        Vector3 vel = this.agent.velocity.normalized;

        animController.Animation(vel.x, vel.z);

        CheckIfTargetInVision();
    }

    public override void OnStateEnter()
    {
        AIVision aIVision = owner.PreviousState as AIVision;
        if (aIVision == null || !aIVision.CurrentTarget)
        {
            owner.SwitchState(targetLost);
            return;
        }

        agent.isStopped = false;
        target = aIVision.CurrentTarget;
        agent.SetDestination(target.position);
    }

    public override void OnStateExit()
    {
        agent.isStopped = true;
        animController.Animation(0, 0);

    }

    private void CheckIfTargetInVision()
    {
        if (target == null)
        {
            owner.SwitchState(targetLost);
            return;
        }

        Vector3 targetPos = target.position;
        Vector3 pos = transform.position;

        float distanceToPlayer = Vector3.Distance(pos, targetPos);
        if (distanceToPlayer > maxViewDistance)
            return;

        Vector3 directionToPlayer = (targetPos - pos).normalized;
        float angleToPlayer = Vector3.Angle(this.transform.forward, directionToPlayer);
        if (angleToPlayer > fov * 0.5f)
            return;

        RaycastHit hit;

        Vector3 raycastPositionStart = pos + new Vector3(0f, 0.5f, 0f);

        if (!target || !target.gameObject.activeSelf)
            owner.SwitchState(targetLost);
        else if (Physics.Raycast(raycastPositionStart, directionToPlayer, out hit, maxViewDistance, targetLayerMask) && hit.collider.transform.root == target)
            owner.SwitchState(targetVisible);
    }
}