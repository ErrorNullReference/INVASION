using UnityEngine;
using UnityEngine.Events;

public class AIAttack : AIBehaviour
{
    public string toType;

    public UnityEvent OnTargetDisappeared;

    public override void AIUpdate()
    {
        Debug.Log("attacking");
        if (Input.GetKeyDown(KeyCode.N))
            OnTargetDisappeared.Invoke();
    }

    public override void OnStateEnter(AIBehaviour previous)
    {
        string previousType = previous.GetType().ToString();
        string toTypeOnEnter = previousType + " was the previous type.";
        Debug.Log(toTypeOnEnter);
    }

    public override void OnStateExit()
    {
        Debug.Log("exiting AIAttack State");
    }
}