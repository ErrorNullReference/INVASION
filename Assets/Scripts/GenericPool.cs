using System;
using System.Collections.Generic;
/// <summary>
/// Pool which works with prototype instance to create new instances
/// </summary>
/// <typeparam name="T">Type of the objects to handle</typeparam>
public class GenericPool<T> where T : class
{
    /// <summary>
    /// Number of elements stored in the pool
    /// </summary>
    public int ElementsStored { get { return elements.Count; } }
    private Queue<T> elements;
    private Func<T, T> allocator;
    /// <summary>
    /// Allocates a new pool instance given the allocator
    /// </summary>
    /// <param name="allocator">allocator that given an original instance returns a new object of the same type</param>
    public GenericPool(Func<T, T> allocator)
    {
        elements = new Queue<T>();
        this.allocator = allocator;
    }
    /// <summary>
    /// Allocates a new pool instance given the allocator. Allows preallocation
    /// </summary>
    /// <param name="allocator">allocator that given an original instance returns a new object of the same type</param>
    /// <param name="original">original instance to use in the allocator</param>
    /// <param name="preallocation">number of elements the pool must start from</param>
    /// <param name="onPreallocation">action called on each element instanciated during preallocation logic</param>
    public GenericPool(Func<T, T> allocator, T original, int preallocation, Action<T> onPreallocation) : this(allocator)
    {
        for (int i = 0; i < preallocation; i++)
        {
            T allocated = allocator(original);
            onPreallocation(allocated);
            elements.Enqueue(allocated);
        }
    }
    /// <summary>
    /// Recycles the given instance
    /// </summary>
    /// <param name="toRecycle">object to recycle</param>
    public void Recycle(T toRecycle)
    {
        elements.Enqueue(toRecycle);
    }
    /// <summary>
    /// Requests an element from the pool.
    /// </summary>
    /// <param name="original">original instance to use in the allocation of a new instance</param>
    /// <returns>the requested element instance</returns>
    public T Get(T original)
    {
        return elements.Count == 0 ? allocator(original) : elements.Dequeue();
    }
    /// <summary>
    /// Clears the pool invoking an action on each element
    /// </summary>
    /// <param name="onDestroy">action invoked on each element in the pool</param>
    public void Clear(Action<T> onDestroy)
    {
        while (elements.Count != 0)
        {
            T obj = elements.Dequeue();
            if (obj != null)
                onDestroy(obj);
        }
    }
}