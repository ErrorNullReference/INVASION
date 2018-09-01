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
    public float DisableShadowFrac = 0.7f;
    Enemy enemy;
    bool dead, shadowsDisabled;
    List<Material> mInstances;
    Dictionary<Material, Shader> mInstancesNot;
    Action InitLate;
    float t, duration;
    DisableShadows[] DisShadows;

    public void Init()
    {
        if (enemy == null)
            enemy = GetComponent<Enemy>();
        if (enemy == null)
            return;

        if (mInstances == null)
            mInstances = new List<Material>();
        else
            mInstances.Clear();
        if (mInstancesNot == null)
            mInstancesNot = new Dictionary<Material, Shader>();
        else
            mInstancesNot.Clear();

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
        if (DisShadows == null)
            DisShadows = GetComponentsInChildren<DisableShadows>();
        
        Reset();
    }

    void Reset()
    {
        shadowsDisabled = false;
        for (int i = 0; i < DisShadows.Length; i++)
            DisShadows[i].Enable();

        dead = false;
        for (int i = 0; i < mInstances.Count; i++)
            mInstances[i].SetFloat("_T", 0);
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

    void DisableShadows()
    {
        shadowsDisabled = true;
        for (int i = 0; i < DisShadows.Length; i++)
            DisShadows[i].Disable();
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

        if (t >= duration * DisableShadowFrac && !shadowsDisabled)
            DisableShadows();
    }

    void OnDestroy()
    {
        enemy.OnDown -= ActivateDisintegration;
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
