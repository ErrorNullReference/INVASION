using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
[RequireComponent(typeof(Collider))]
public class CustomRigidBody : MonoBehaviour
{
    public bool UseGravity;
    public int MaxRepositionAttempts = 5;
    public LayerMaskHolder CustomMask;
    Collider[] results;
    Vector3[] contactPoints;
    Collider coll;
    Vector3 newDir, midPoint;
    float dist1, dist2;
    int attempts;

    void Start()
    {
        coll = GetComponent<Collider>();
        results = new Collider[10];
        contactPoints = new Vector3[results.Length];
    }

    void Update()
    {
        if (UseGravity)
            ApplyGravity();
    }

    void ApplyGravity()
    {
        Move(Physics.gravity.normalized, Physics.gravity.magnitude, Time.deltaTime, false);
    }

    /// <summary>
    /// Move the transform if no obstacle is detected.
    /// </summary>
    /// <param name="direction">Direction.</param>
    /// <param name="speed">Speed.</param>
    /// <param name="time">Time.</param>
    /// <param name="advanced">if false the transform will not move if a collision is detected. If true the transform will try to slide over the bounding box. Note that the advanced mode is less performant.</param>
    public void Move(Vector3 direction, float speed, float time, bool advanced)
    {
        Vector3 position = coll.bounds.center + direction * speed * time;

        if (advanced)
        {
            attempts = 0;
            transform.position = TryMove(position, direction, time) + (transform.position - coll.bounds.center);
        }
        else
            transform.position = TryMove(position);
    }

    Vector3 TryMove(Vector3 position)
    {
        if (Physics.OverlapSphereNonAlloc(position, GetRightExtent(position), results, CustomMask) >= 1)
            return transform.position;
        return position + (transform.position - coll.bounds.center);
    }

    Vector3 TryMove(Vector3 position, Vector3 direction, float time)
    {
        int num = Physics.OverlapSphereNonAlloc(position, GetRightExtent(position), results, CustomMask);
        if (num >= 1)
        {
            attempts++;
            if (attempts >= MaxRepositionAttempts)
                return position;

            newDir = Vector3.zero;
            midPoint = Vector3.zero;

            for (int i = 0; i < num; i++)
            {
                contactPoints[i] = results[i].ClosestPoint(position);
                dist1 = (coll.bounds.center - position).sqrMagnitude;
                dist2 = (coll.bounds.center - contactPoints[i]).sqrMagnitude;
                Vector3 d = dist1 < dist2 ? (position - contactPoints[i]).normalized : (contactPoints[i] - position).normalized;

                newDir += d;
                midPoint += contactPoints[i] + d * GetRightExtent(contactPoints[i]);
            }
            newDir = (direction + newDir.normalized).normalized;
            midPoint /= (float)num;

            return TryMove(newDir != Vector3.zero ? midPoint + newDir * time : coll.bounds.center, newDir, time);
        }
        return position;
    }

    float GetRightExtent(Vector3 position)
    {
        Vector3 dir = (position - coll.bounds.center).normalized;
        dir = new Vector3(Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z));
        return dir.x > dir.y ? (dir.x > dir.z ? coll.bounds.extents.x : coll.bounds.extents.z) : (dir.y > dir.z ? coll.bounds.extents.y : coll.bounds.extents.z);

        /*if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y) && Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            return collider.bounds.extents.x + collider.bounds.center.x;
        else if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x) && Mathf.Abs(direction.y) > Mathf.Abs(direction.z))
            return collider.bounds.extents.y + collider.bounds.center.y;
        else
            return collider.bounds.extents.z + collider.bounds.center.z;*/
    }

    //void OnTriggerEnter(Collider c)
    //{
    //}
}
