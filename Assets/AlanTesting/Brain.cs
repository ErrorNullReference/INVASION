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
    private AIBehaviour activeState;
    private AIBehaviour precedentState;

    private void Awake()
    {
        AIBehaviour[] componentsToAdd = this.GetComponents<AIBehaviour>();
        for (int i = 0; i < componentsToAdd.Length; i++)
        {
            if (componentsToAdd[i].ActivateOnInitialize && activeState == null)
            {
                activeState = componentsToAdd[i];
                break;
            }
        }

        if (activeState != null)
            activeState.Init(this);
    }
	
	private void Update ()
    {
        if (activeState != null)
            activeState.AIUpdate();
	}

    public void SwitchState(AIBehaviour next)
    {
        precedentState = this.activeState;
        activeState = next;
        if (!activeState.Initialized)
            activeState.Init(this);
        activeState.OnStateEnter(precedentState);
    }
}
