using UnityEngine;

public class Brain : MonoBehaviour
{
    [SerializeField]
    private bool disableIfClient;

    public AIBehaviour CurrentState { get { return currentState; } }

    public AIBehaviour PreviousState { get { return previousState; } }

    [SerializeField]
    private AIBehaviour currentState;
    private AIBehaviour previousState;

    [HideInInspector]
    public Enemy enemy;

    private void Start()
    {
        if (disableIfClient && !Client.IsHost)
        {
            this.enabled = false;
            return;
        }

        enemy = GetComponent<Enemy>();

        if (currentState == null)
            currentState = GetComponent<AIAggroPlayers>();

        if (currentState != null)
        {
            currentState.enabled = true;
            currentState.OnStateEnter();
        }
    }

    void OnEnable()
    {
        if (disableIfClient && !Client.IsHost)
            return;

        if (currentState == null && previousState != null)
        {
            currentState = previousState;
            currentState.enabled = true;
            currentState.OnStateEnter();
        }
    }

    public void SwitchState(AIBehaviour next)
    {
        if (disableIfClient && !Client.IsHost)
            return;

        previousState = this.currentState;

        if (previousState)
        {
            previousState.OnStateExit();
            previousState.enabled = false;
        }

        currentState = next;

        currentState.enabled = true;

        currentState.OnStateEnter();
    }

    public void ShutDown()
    {
        if (disableIfClient && !Client.IsHost)
            return;

        previousState = this.currentState;

        if (previousState)
        {
            previousState.OnStateExit();
            previousState.enabled = false;
        }

        currentState = null;
    }
}