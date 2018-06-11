using UnityEngine;
using SOPRO;
public class SphericalVision : MonoBehaviour
{
    [SerializeField]
    private bool workAsClient;

    [SerializeField]
    private float maxViewDistance;

    [SerializeField]
    private MovementManager manager;

    [SerializeField]
    private SOListPlayerContainer players;

    private GameObject target;

    private bool isLooking;

    private void Awake()
    {
        if (!workAsClient && !Client.IsHost)
            this.enabled = false;
    }

    private void OnEnable()
    {
        isLooking = true;
    }

    private void Update()
    {
        target = null;

        if (isLooking)
            CheckPlayerInVision();

        if (target != null)
            manager.SetDestination(target);
    }

    private void CheckPlayerInVision()
    {
        Vector3 pos = transform.position;
        int length = players.Elements.Count;
        for (int i = 0; i < length; i++)
        {
            Player p = players[i];
            Vector3 direction = (p.transform.position - pos);
            Ray ray = new Ray(pos + new Vector3(0, 0.5f, 0), direction.normalized);
            RaycastHit hit;
            if (p.PlayerCollider.Raycast(ray, out hit, 100))
            {
                target = p.gameObject;
                break;
            }
        }
    }

    public void StopLooking()
    {
        isLooking = false;
    }

    public void StartLooking()
    {
        isLooking = true;
    }
}