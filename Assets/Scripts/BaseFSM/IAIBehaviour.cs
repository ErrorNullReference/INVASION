using UnityEngine;

public abstract class AIBehaviour : MonoBehaviour
{
    [SerializeField]
    protected Brain owner;

    protected virtual void Awake()
    {
        this.enabled = false;

        if (owner == null)
            owner = GetComponent<Brain>();
    }

    public abstract void OnStateEnter();

    public abstract void OnStateExit();
}