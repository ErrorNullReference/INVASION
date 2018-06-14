using UnityEngine;
public class SimpleMover : MonoBehaviour
{
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private Transform myTransform;
    void Update()
    {
        this.myTransform.position += this.myTransform.forward * speed * Time.deltaTime;
    }
}