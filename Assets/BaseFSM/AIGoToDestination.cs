using System;
using UnityEngine;
using UnityEngine.AI;
using SOPRO;
[RequireComponent(typeof(NavMeshAgent))]
public class AIGoToDestination : AIBehaviour
{
    //[SerializeField]
    //private float cooldownBetweenRecalculations;
    //private float currentCooldownLeft;

    private Vector3 targetDestination;

    private NavMeshAgent agent;

    //private WaitForSeconds waitForSecond;

    public BaseSOEvVoid OnDestinationReached;

    protected override void Awake()
    {
        base.Awake();
        //waitForSecond = new WaitForSeconds(0.1f);
        agent = this.GetComponent<NavMeshAgent>();
    }

    protected void OnEnable()
    {
        //currentCooldownLeft = 0;
    }

    private void Update()
    {
        float distanceToDestination = (this.transform.position - targetDestination).magnitude;
        if(distanceToDestination <= agent.stoppingDistance)
        {
            OnDestinationReached.Raise();
        }
    }

    public override void OnStateEnter()
    {
        AIVision aIVision = owner.PreviousState as AIVision;
        if (aIVision == null)
        {
            OnDestinationReached.Raise();
            return;
        }

        agent.isStopped = false;
        targetDestination = aIVision.CurrentTarget.transform.position;
        agent.SetDestination(targetDestination);
        return;
    }

    public override void OnStateExit()
    {
        agent.isStopped = true;
    }
}