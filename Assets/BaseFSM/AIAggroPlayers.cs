using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAggroPlayers : AIVision
{
    private Player[] possibleTargets;

    public override void AIUpdate()
    {
        this.currentTarget = possibleTargets[Random.Range(0, possibleTargets.Length)].gameObject;
        OnSpottedTarget.Raise();
    }

    public override void OnStateEnter()
    {
        currentTarget = null;
        possibleTargets = FindObjectsOfType<Player>();
    }

    public override void OnStateExit()
    {
        possibleTargets = null;
    }
}
