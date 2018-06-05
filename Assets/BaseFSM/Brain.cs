using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AISwitchEvent : UnityEvent<AIBehaviour> { }

public class Brain : MonoBehaviour
{
    public AIBehaviour CurrentState { get { return currentState; } }
    public AIBehaviour PreviousState
    {
        get
        {
            if (previousState != null)
                return previousState;
            else
                return currentState;
        }
    }

    [SerializeField]
    private AIBehaviour currentState;
    private AIBehaviour previousState;

    private void Start()
    {
        if (currentState != null)
        {
            currentState.Init(this);
            currentState.OnStateEnter();
        }
    }

    private void Update()
    {
        if (currentState != null)
            currentState.AIUpdate();
    }

    public void SwitchState(AIBehaviour next)
    {
        currentState.OnStateExit();

        previousState = this.currentState;
        currentState = next;
        if (!currentState.Initialized)
            currentState.Init(this);

        currentState.OnStateEnter();
    }
}
