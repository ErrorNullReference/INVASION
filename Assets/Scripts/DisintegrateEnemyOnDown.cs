using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DisintegrateEnemyOnDown : MonoBehaviour
{
    public string PassName;
    public ParticleSystem PS;
    public Shader Shader;
    public float DurationMult;
    Enemy enemy;
    bool init, dead;
    List<Material> mInstances;
    Dictionary<Material, Shader> mInstancesNot;
    Action InitLate;
    float t, duration;
    int index;

    void Init()
    {
        enemy = GetComponent<Enemy>();
        if (enemy == null)
            return;

        mInstances = new List<Material>();
        mInstancesNot = new Dictionary<Material, Shader>();

        Renderer[] r = enemy.GetComponentsInChildren<Renderer>();
        PassName = PassName.ToUpper();
        for (int i = 0; i < r.Length; i++)
        {
            Material[] m = r[i].materials;
            for (int j = 0; j < m.Length; j++)
            {
                string s = m[j].GetPassName(0);
                if (m[j].GetPassName(0) == PassName)
                    mInstances.Add(m[j]);
                else
                    mInstancesNot.Add(m[j], m[j].shader);
            }
        }

        enemy.OnDown += ActivateDisintegration;
        //enemy.OnDeath += ActivatePS;
        
        Reset();

        init = true;
    }

    void OnEnable()
    {
        if (!init)
        {
            InitLate = Init;
            return;
        }

        Reset();
    }

    void Reset()
    {
        // Replace(null);
        dead = false;
        for (int i = 0; i < mInstances.Count; i++)
            mInstances[i].SetFloat("_T", 0); //2f
        t = 0;
    }

    void ActivateDisintegration()
    {
        duration = ((EnemyStats)enemy.Stats).DeathTime * DurationMult;
        for (int i = 0; i < mInstances.Count; i++)
            mInstances[i].SetFloat("_Duration", duration);
        dead = true;
        t = 0;
    }

    void MidDisintegration()
    {
        index = -1;
        Replace(Shader);
    }

    void ActivatePS()
    {
        ParticleSystem ps = Instantiate(PS);
        ps.transform.position = transform.position;
        ps.transform.rotation = Quaternion.Euler(90, 0, 0);
        ps.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (InitLate != null)
        {
            InitLate.Invoke();
            InitLate = null;
        }

        if (!dead)
            return;
        
        t += Time.deltaTime;
        for (int i = 0; i < mInstances.Count; i++)
            mInstances[i].SetFloat("_T", t);

        /*if (t >= duration / 2f && index == 1)
        {
            MidDisintegration();
            t = duration / 2f;
        }*/
    }

    void OnDestroy()
    {
        enemy.OnDown -= ActivateDisintegration;
        //enemy.OnDeath -= ActivatePS;
    }

    void Replace(Shader shader)
    {
        foreach (KeyValuePair<Material, Shader> KvP in mInstancesNot)
        {
            if (shader != null)
                KvP.Key.shader = shader;
            else
                KvP.Key.shader = KvP.Value;
        }
    }
}
