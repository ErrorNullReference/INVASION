using System;
using UnityEngine;
using System.Collections.Generic;
using SOPRO;

[Serializable]
public abstract class Factory<T> : ScriptableObject
{
    public Dictionary<T, SOPool> organizedPools { get; protected set; }

    [SerializeField]
    private SOPool[] pools = new SOPool[0];

    protected virtual void OnEnable()
    {
        int capacity;
        IEqualityComparer<T> comparer = GetComparer(out capacity);

        if (comparer == null)
            organizedPools = new Dictionary<T, SOPool>(capacity);
        else
            organizedPools = new Dictionary<T, SOPool>(capacity, comparer);

        int length = pools.Length;

        for (int i = 0; i < length; i++)
        {
            SOPool pool = pools[i];

            T identifier = ExtractIdentifier(pool.Prefab, i);

            if (organizedPools.ContainsKey(identifier))
                throw new ArgumentException("Impossible to initialize FactoryObj, 1 or more objects are classified by the same identifier (" + identifier + "). Conflict between " + pool.name + " and " + organizedPools[identifier].name, "pools");

            organizedPools.Add(identifier, pool);
        }
    }

    /// <summary>
    /// Called when filling up organizedPools.
    /// </summary>
    /// <param name="obj">obj from which to extract identifier</param>
    /// <returns>identifier</returns>
    protected abstract T ExtractIdentifier(GameObject obj, int i);

    /// <summary>
    /// Called on organizedPools initialization. Return null to not use custom comparer
    /// </summary>
    /// <param name="capacity">Capacity used</param>
    /// <returns>Comparer used. Return null to not use custom comparer</returns>
    protected virtual IEqualityComparer<T> GetComparer(out int capacity)
    {
        capacity = 0;
        return null;
    }
}