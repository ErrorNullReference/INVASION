using UnityEngine;

public class AIFindPlayer : AIBehaviour
{
    public string playerNameFound;

    public string toType;

    public AIBehaviour OnPlayerFoundNext;
    public AISwitchEvent OnPlayerFound;

    public override void AIUpdate()
    {
        Debug.Log("looking for player");
        if (Input.GetKeyDown(KeyCode.N))
        {
            
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