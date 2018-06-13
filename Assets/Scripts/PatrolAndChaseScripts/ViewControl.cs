using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
public class ViewControl : MonoBehaviour //TODO: what is this class??
{
    private Transform root;
    //public EnemyController enemy;
    public Transform transf;
    public bool Overlap;
    public float distance;
    public LayerMaskHolder RayMask;
    public LayerHolder PlayerLayer;

    private void Awake()
    {
        root = transform.root;
       // enemy = root.GetComponent<EnemyController>();
    }
    private void Update()
    {
        if (Overlap)
            if (Application.isEditor)
            {
                transform.localScale = new Vector3(distance * 0.5f, distance * 0.5f, distance * 0.5f);

            }
        {
            OverlapMethod();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (!Overlap)
        {
            Check(other);
        }

    }
    private void Check(Collider other)
    {
        if (other.gameObject.layer == PlayerLayer)
        {
            Vector3 direction = (other.transform.position - (transf.position + new Vector3(0, 0, 0)));
            Debug.DrawRay(transf.position, Vector3.up * 800, Color.red);
            Ray ray = new Ray(transf.position, direction.normalized);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, RayMask, QueryTriggerInteraction.Ignore))
            {

                if (hit.collider.gameObject.layer == PlayerLayer)
                {
                    //enemy.Chase = true;
                    gameObject.SetActive(false);
                }
                else
                {
                }
            }

        }
    }

    private void OverlapMethod()
    {
        Collider[] cols = Physics.OverlapSphere(root.position, distance);
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].gameObject.layer == PlayerLayer)
            {
                Vector3 direction = (cols[i].transform.position - (transf.position + new Vector3(0, 0, 0)));
                Debug.DrawRay(transf.position, Vector3.up * 800, Color.red);
                Ray ray = new Ray(transf.position, direction.normalized);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, RayMask, QueryTriggerInteraction.Ignore))
                {

                    if (hit.collider.gameObject.layer == PlayerLayer)
                    {
                        //enemy.Chase = true;
                        gameObject.SetActive(false);
                    }
                    else
                    {
                    }
                }

            }
        }
    }
}

