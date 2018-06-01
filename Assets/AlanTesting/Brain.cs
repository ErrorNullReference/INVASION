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
    private Dictionary<System.Type, AIBehaviour> behaviours;
    
    private void Awake()
    {
        behaviours = new Dictionary<System.Type, AIBehaviour>();
    }

    private void Start ()
    {
        AIBehaviour[] componentsToAdd = this.GetComponents<AIBehaviour>();
        for (int i = 0; i < componentsToAdd.Length; i++)
        {
            if (!behaviours.ContainsKey(componentsToAdd[i].GetType()))
            {
                behaviours.Add(componentsToAdd[i].GetType(), componentsToAdd[i]);

                if (componentsToAdd[i].ActivateOnInitialize && activeState == null)
                    activeState = componentsToAdd[i];
            }
        }

        if (activeState != null)
            activeState.Init(this);
    }
	
	private void Update ()
    {
        activeState.AIUpdate();
	}

    public void SwitchState(AIBehaviour next)
    {
        precedentState = this.activeState;
        activeState = next;
        activeState.OnStateEnter(precedentState);
    }
}
