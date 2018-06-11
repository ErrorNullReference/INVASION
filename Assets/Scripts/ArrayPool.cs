using System.Collections.Generic;
/// <summary>
/// Very basic generic pool for arrays
/// </summary>
/// <typeparam name="T">underlying array type</typeparam>
public static class ArrayPool<T>
{
    /// <summary>
    /// Number of different array sizes currently stored
    /// </summary>
    public static int ElementsCount { get { return elements.Count; } }

    private static Dictionary<int, Queue<T[]>> elements;

    static ArrayPool()
    {
        elements = new Dictionary<int, Queue<T[]>>();
    }
    /// <summary>
    /// Get an array instance
    /// </summary>
    /// <param name="sizeNeeded">array size needed</param>
    /// <returns>array instance</returns>
    public static T[] Get(int sizeNeeded)
    {
        if (!elements.ContainsKey(sizeNeeded))
            elements.Add(sizeNeeded, new Queue<T[]>());

        Queue<T[]> q = elements[sizeNeeded];

        return q.Count == 0 ? new T[sizeNeeded] : q.Dequeue();
    }
    /// <summary>
    /// Recycle an array
    /// </summary>
    /// <param name="toRecycle">array to recycle</param>
    public static void Recycle(T[] toRecycle)
    {
        int length = toRecycle.Length;

        if (!elements.ContainsKey(length))
            elements.Add(length, new Queue<T[]>());

        elements[length].Enqueue(toRecycle);
    }
    /// <summary>
    /// Clears the given size pool
    /// </summary>
    /// <param name="size">size of arrays in the pool to clear</param>
    public static void Clear(int size)
    {
        if (elements.ContainsKey(size))
            elements[size].Clear();
    }
    /// <summary>
    /// Clear all pools
    /// </summary>
    public static void ClearAll()
    {
        elements.Clear();
    }
    /// <summary>
    /// Resizes the given size pool to reach a tot number of elements in it
    /// </summary>
    /// <param name="arraySize">size of arrays in pool to resize</param>
    /// <param name="elementCount">target count of element</param>
    public static void Resize(int arraySize, int elementCount)
    {
        if (!elements.ContainsKey(arraySize))
            elements.Add(arraySize, new Queue<T[]>());

        Queue<T[]> queue = elements[arraySize];

        while (queue.Count < elementCount)
        {
            queue.Enqueue(new T[arraySize]);
        }
        while (queue.Count > elementCount)
        {
            queue.Dequeue();
        }
    }
    /// <summary>
    /// Resizes all pools to reach a tot number of elements in them. Alloc for iteration
    /// </summary>
    /// <param name="elementCount">target count of element</param>
    public static void ResizeAll(int elementCount)
    {
        foreach (KeyValuePair<int, Queue<T[]>> item in elements)
        {
            while (item.Value.Count < elementCount)
            {
                item.Value.Enqueue(new T[item.Key]);
            }
            while (item.Value.Count > elementCount)
            {
                item.Value.Dequeue();
            }
        }
    }
}