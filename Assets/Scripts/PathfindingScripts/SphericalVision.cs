using UnityEngine;
using SOPRO;
public class SphericalVision : MonoBehaviour
{
    public SOEvGameObject OnPlayerSight;

    [SerializeField]
    private bool workAsClient;

    [SerializeField]
    private float maxViewDistance;

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
            OnPlayerSight.Raise(target);
    }

    private void CheckPlayerInVision()
    {
        Collider[] cols = Physics.OverlapSphere(this.gameObject.transform.position, maxViewDistance);
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].gameObject.GetComponentInParent<Player>() == null)
                continue;

            Vector3 direction = (cols[i].transform.position - (this.transform.position));
            Ray ray = new Ray(this.transform.position+new Vector3(0,0.5f,0), direction.normalized);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, int.MaxValue, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.GetComponentInParent<Player>() != null)
                    target = hit.collider.gameObject;
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