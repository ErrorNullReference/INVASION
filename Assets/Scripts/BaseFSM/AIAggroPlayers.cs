using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
using Steamworks;

public class AIAggroPlayers : AIVision
{
    [SerializeField]
    private SOListPlayerContainer possibleTargets;
    [SerializeField]
    private AIBehaviour aggroActivated;

    protected override void Awake()
    {
        base.Awake();
        if (aggroActivated == null)
            aggroActivated = GetComponent<AIGoToTargetUntilOnSight>();
    }

    private void Update()
    {
        Player p = possibleTargets.Elements.Count == 0 ? null : possibleTargets[Random.Range(0, possibleTargets.Elements.Count)];
        if (p != null)
            this.currentTarget = p.transform;
        if (currentTarget)
            owner.SwitchState(aggroActivated);
    }

    public override void OnStateEnter()
    {
    }

    public override void OnStateExit()
    {
    }
}
