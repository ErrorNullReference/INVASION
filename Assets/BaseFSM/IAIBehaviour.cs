using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class AIBehaviour : MonoBehaviour
{
    private Brain owner;

    private bool initialized;

    public bool Initialized { get { return initialized; } }

    public void Init(Brain brain) { this.owner = brain; initialized = true; }
    
    public void Init() 
    {
        Brain[] brainCheck = this.gameObject.GetComponents<Brain>();
        if (brainCheck.Length > 1)
        {
            Debug.LogError("You need only one brain!");
        }
        else
        {
            owner = brainCheck[0];
            this.initialized = true;
        }
    }

    public abstract void OnStateEnter();
    public abstract void OnStateExit();
    public abstract void AIUpdate();
}
