

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using SOPRO;
[RequireComponent(typeof(NavMeshAgent))]
public class AIGoToTargetUntilOnSight : AIBehaviour
{
    [SerializeField]
    private LayerMaskHolder layerToLookInto;
    [SerializeField]
    private float maxViewDistance;
    [SerializeField][Range(1, 360)]
    private int fov;
    [SerializeField]
    private float cooldownBeforeRecalculation;
    private float currentCooldownLeftBeforeRecalculation;

    private GameObject target;
    public GameObject Target { get { return this.target; } private set { } }
    private NavMeshAgent agent;
    private AnimationControllerScript animController;

    public SOEvVoid OnTargetOnSight;

    public void Awake()
    {
        agent = this.GetComponent<NavMeshAgent>();
        animController = this.GetComponent<AnimationControllerScript>();
    }

    public override void AIUpdate()
    {
        currentCooldownLeftBeforeRecalculation -= Time.deltaTime;
        if (currentCooldownLeftBeforeRecalculation <= 0)
            this.agent.SetDestination(this.target.gameObject.transform.position);

        animController.Animation(this.agent.velocity.normalized.x, this.agent.velocity.normalized.z);

        CheckIfTargetInVision();
    }

    public override void OnStateEnter()
    {
        AIVision aIVision = owner.PreviousState as AIVision;
        if (aIVision == null)
        {
            OnTargetOnSight.Raise();
            return;
        }

        agent.isStopped = false;
        target = aIVision.CurrentTarget;
        agent.SetDestination(target.transform.position);
    }

    public override void OnStateExit()
    {
        agent.isStopped = true;
        animController.Animation(0, 0);

    }

    private void CheckIfTargetInVision()
    {
        float distanceToPlayer = Vector3.Distance(this.transform.position, target.gameObject.transform.position);
        if (distanceToPlayer > maxViewDistance)
            return;

        Vector3 directionToPlayer = target.gameObject.transform.position - this.transform.position;
        float angleToPlayer = Vector3.Angle(this.transform.forward, directionToPlayer);
        if (angleToPlayer > fov * 0.5f)
            return;

        RaycastHit hit;

        Vector3 raycastPositionStart = transform.position + new Vector3(0f, 0.5f, 0f);
        if (Physics.Raycast(raycastPositionStart, directionToPlayer, out hit, maxViewDistance, layerToLookInto))
        {
            if(hit.collider.gameObject == target)
            {
                Debug.Log("TARGET ON SIGHT!");
                OnTargetOnSight.Raise();
            }
        }
    }

}