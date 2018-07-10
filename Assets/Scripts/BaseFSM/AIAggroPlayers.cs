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

    private void Update()
    {
        this.currentTarget = possibleTargets.Elements.Count == 0 ? null : possibleTargets[Random.Range(0, possibleTargets.Elements.Count)].transform;
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
