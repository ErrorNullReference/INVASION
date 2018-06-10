using UnityEngine;
public abstract class AIBehaviour : MonoBehaviour
{
    [SerializeField]
    protected Brain owner;

    protected virtual void Awake()
    {
        this.enabled = false;
    }

    public abstract void OnStateEnter();
    public abstract void OnStateExit();
}