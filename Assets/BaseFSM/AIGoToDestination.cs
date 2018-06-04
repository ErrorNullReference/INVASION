using UnityEngine;
using UnityEngine.Events;

internal class AIGoToDestination : AIBehaviour
{
    public string toType;

    public UnityEvent OnDestinationReached;

    public override void AIUpdate()
    {
        Debug.Log("going to destination");
        if (Input.GetKeyDown(KeyCode.N))
            OnDestinationReached.Invoke();
    }

    public override void OnStateEnter( )
    {
        string toTypeOnEnter = "ON STATE ENTER : AI go to destination ";
        Debug.Log(toTypeOnEnter);
    }

    public override void OnStateExit()
    {
        Debug.Log("ON STATE EXIT : AI go to destination ");
    }
}