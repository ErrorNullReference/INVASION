using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class EnemyInitializer : ScriptableObject
{
    public static int StartInstancesAmount = 3;
    [SerializeField]
    public GameObject Body;
    [HideInInspector]
    public List<BodyInstanced> BodyInstance;

    public struct BodyInstanced
    {
        public GameObject Instance;
        public bool Used;
    }

    public void InitInstances()
    {
        if (BodyInstance == null)
            BodyInstance = new List<BodyInstanced>();

        if (BodyInstance.Count != 0)
            return;

        for (int i = 0; i < StartInstancesAmount; i++)
        {
            BodyInstanced b2 = new BodyInstanced();
            b2.Instance = Instantiate(Body);
            b2.Instance.hideFlags = HideFlags.HideInHierarchy;
            b2.Used = false;
            b2.Instance.SetActive(false);
            BodyInstance.Add(b2);
        }
    }

    public void Init(Enemy enemy, Transform root, ref int index)
    {
        if (BodyInstance == null)
            BodyInstance = new List<BodyInstanced>();

        GameObject o = GetBody(ref index);

        o.SetActive(true);
        o.transform.SetParent(root);
        o.transform.localPosition = Vector3.zero;
        o.transform.localRotation = Quaternion.identity;

        enemy.animator = o.GetComponent<Animator>();
        enemy.animController = o.GetComponent<AnimationControllerScript>();
    }

    public void Destroy(int index)
    {
        for (int i = 0; i < BodyInstance.Count; i++)
        {
            if (i == index)
            {
                BodyInstanced b = BodyInstance[i];
                b.Used = false;
                BodyInstance[i] = b;
                BodyInstance[i].Instance.transform.SetParent(null);
                BodyInstance[i].Instance.SetActive(false);

                return;
            }
        }
    }

    GameObject GetBody(ref int index)
    {
        for (int i = 0; i < BodyInstance.Count; i++)
        {
            if (!BodyInstance[i].Used)
            {
                BodyInstanced b = BodyInstance[i];
                b.Used = true;
                BodyInstance[i] = b;
                index = i;
                return BodyInstance[i].Instance;
            }
        }

        BodyInstanced b2 = new BodyInstanced();
        b2.Instance = Instantiate(Body);
        b2.Instance.hideFlags = HideFlags.HideInHierarchy;
        b2.Used = true;
        BodyInstance.Add(b2);
        index = BodyInstance.Count - 1;
        return b2.Instance;
    }

    void OnDestroy()
    {
        for (int i = 0; i < BodyInstance.Count; i++)
            Destroy(BodyInstance[i].Instance);
        BodyInstance.Clear();
    }
}
