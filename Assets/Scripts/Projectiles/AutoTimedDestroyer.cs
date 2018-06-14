using UnityEngine;
public class AutoTimedDestroyer : MonoBehaviour
{
    [SerializeField]
    private float timeLimit = 2f;

    private float currentTime;
    void OnEnable()
    {
        currentTime = 0f;
    }
    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= timeLimit)
            Destroy(this.gameObject);
    }
}