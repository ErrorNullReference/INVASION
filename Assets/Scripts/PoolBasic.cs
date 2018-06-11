using System;
using System.Collections.Generic;
/// <summary>
/// Classic pool for general purposes
/// </summary>
/// <typeparam name="T">Type of object to handle</typeparam>
public static class PoolBasic<T> where T : class, new()
{
    /// <summary>
    /// Number of elements stored in the pool
    /// </summary>
    public static int ElementsStored { get { return elements.Count; } }
    private static Queue<T> elements;
    static PoolBasic()
    {
        elements = new Queue<T>();
    }
    /// <summary>
    /// Requests element from the pool
    /// </summary>
    /// <returns>object of the given type</returns>
    public static T Get()
    {
        return elements.Count != 0 ? elements.Dequeue() : new T();
    }
    /// <summary>
    /// Recycles the given instance in the pool
    /// </summary>
    /// <param name="toRecycle">instance to recycle</param>
    public static void Recycle(T toRecycle)
    {
        elements.Enqueue(toRecycle);
    }
    /// <summary>
    /// Clears the pool
    /// </summary>
    public static void Clear()
    {
        elements.Clear();
    }
    /// <summary>
    /// Clears the pool, invoking the given action on each element
    /// </summary>
    /// <param name="onDestroy">action invoked on each element in the pool</param>
    public static void Clear(Action<T> onDestroy)
    {
        while (elements.Count != 0)
            onDestroy(elements.Dequeue());
    }
}