using UnityEngine;
using UnityEngine.Events;

public class AIFindPlayer : AIBehaviour
{
    public string playerNameFound;
    public string toType;

    public UnityEvent OnPlayerFound;
    public UnityEvent OnPlayerFoundInRange;

    public override void AIUpdate()
    {
        Debug.Log("looking for player");
        if (Input.GetKeyDown(KeyCode.N))
        {
            OnPlayerFound.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            OnPlayerFoundInRange.Invoke();
        }
    }

    public override void OnStateEnter(AIBehaviour previous)
    {
        string previousType = previous.GetType().ToString();
        string toTypeOnEnter = previousType + " was the previous type.";
        Debug.Log(toTypeOnEnter);
    }

    public override void OnStateExit()
    {
        Debug.Log("exiting AIFindPlayer State");
    }
}