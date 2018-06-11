using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
public class AIAggroPlayers : AIVision
{
    [SerializeField]
    private SOListPlayerContainer possibleTargets;
    [SerializeField]
    private AIBehaviour next;

    private void Update()
    {
        this.currentTarget = possibleTargets.Elements.Count == 0 ? null : possibleTargets[Random.Range(0, possibleTargets.Elements.Count)].transform;
        if (currentTarget)
            owner.SwitchState(next);
    }

    public override void OnStateEnter()
    {
    }

    public override void OnStateExit()
    {
    }
}
