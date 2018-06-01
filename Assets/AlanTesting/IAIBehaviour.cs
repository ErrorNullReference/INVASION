using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class AIBehaviour : MonoBehaviour
{
    private Brain owner;

    public bool ActivateOnInitialize;
    public void Init(Brain brain) { this.owner = brain; }

    public abstract void OnStateEnter(AIBehaviour previous);
    public abstract void AIUpdate();
    public abstract void OnStateExit();
}
