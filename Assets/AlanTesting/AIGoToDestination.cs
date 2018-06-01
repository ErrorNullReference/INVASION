using UnityEngine;

internal class AIGoToDestination : AIBehaviour
{
    public string toType;

    public override void AIUpdate()
    {
        Debug.Log("going to destination");
    }

    public override void OnStateEnter(AIBehaviour previous)
    {
        string previousType = previous.GetType().ToString();

        if(previous.GetType() == typeof(AIFindPlayer))
        {
            AIFindPlayer playerFound = (AIFindPlayer)previous;
            previousType = "The player found is: " + playerFound.playerNameFound + ". " + previous.GetType().ToString();
        }

        string toTypeOnEnter = previousType + " was the previous type.";
        Debug.Log(toTypeOnEnter);
    }

    public override void OnStateExit()
    {
        Debug.Log("exiting AIGoToDestination State");
    }
}