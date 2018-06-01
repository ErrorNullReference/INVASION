using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AISwitchEvent : UnityEvent<PassingAIBehaviour> { }

public class Brain : MonoBehaviour
{
    private List<AIBehaviour> activeBehaviours;
    private Dictionary<System.Type, AIBehaviour> behaviours;

    private void Awake()
    {
        activeBehaviours = new List<AIBehaviour>();
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
                if(componentsToAdd[i].ActivateOnInitialize)
                    activeBehaviours.Add(componentsToAdd[i]);
            }
        }

        for (int i = 0; i < activeBehaviours.Count; i++)
        {
            activeBehaviours[i].Init(this);
        }
    }
	
	private void Update ()
    {
        for (int i = 0; i < activeBehaviours.Count; i++)
        {
            activeBehaviours[i].AIUpdate();
        }
	}

    public void SwitchState(PassingAIBehaviour passingAIBehaviour)
    {
        passingAIBehaviour.previous.OnStateExit();
        AIBehaviour nextBehaviour;
        if (behaviours.ContainsKey(passingAIBehaviour.next.GetType()))
        {
            nextBehaviour = behaviours[passingAIBehaviour.next.GetType()];
            if (!activeBehaviours.Contains(nextBehaviour))
            {
                nextBehaviour.OnStateEnter(passingAIBehaviour.previous);
                activeBehaviours.Add(nextBehaviour);
            }
        }
    }

    public void DeactivateState(PassingAIBehaviour passingAIBehaviour)
    {
        activeBehaviours.Remove(passingAIBehaviour.previous);
    }
}
