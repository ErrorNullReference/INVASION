using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class AIBehaviour : MonoBehaviour
{
    private Brain owner;

    public bool ActivateOnInitialize;

    private bool initialized;
    public bool Initialized { get { return initialized; } private set { initialized = value; } }

    public void Init(Brain brain) { this.owner = brain; this.initialized = true; }

    public abstract void OnStateEnter(AIBehaviour previous);
    public abstract void AIUpdate();
    public abstract void OnStateExit();
}
