﻿using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class AIGoToDestination : AIBehaviour
{
    //[SerializeField]
    //private float cooldownBetweenRecalculations;
    //private float currentCooldownLeft;

    private Vector3 targetDestination;

    private NavMeshAgent agent;

    //private WaitForSeconds waitForSecond;

    public UnityEvent OnDestinationReached;

    public void Awake()
    {
        //waitForSecond = new WaitForSeconds(0.1f);
        agent = this.GetComponent<NavMeshAgent>();
    }

    public void OnEnable()
    {
        //currentCooldownLeft = 0;
    }

    public override void AIUpdate()
    {
        float distanceToDestination = (this.transform.position - targetDestination).magnitude;
        if(distanceToDestination <= agent.stoppingDistance)
        {
            OnDestinationReached.Invoke();
        }
    }

    public override void OnStateEnter()
    {
        AIVision aIVision = owner.PreviousState as AIVision;
        if (aIVision == null)
        {
            OnDestinationReached.Invoke();
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