using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a Generic pool: can be used to recycle all the objects or components you need.
//considering we are working in unity the pool will be most likely of GameObjects.
//the pool's constructor takes as params a prefab that will be the prefab will all the components attached that you want to be instantiated and
//a function that will be the the one you need to instantiate the prefab (generically Instantiate );
//with AddToQueue will be allocated and instantiated n numbers of prefabs and then enqueued in the pool's Queue
//to AddToQueue Method can be passed a function as a param: you mught want to set the instantiated prefab as inactive until you get it for example -> i will then pass: (cb=>{cb.SetActive(false);})
//as the function method
//the Get function will Get the first element of the queue if there are any, otherwise it will instantiate a new prefab. On the item Will be called the function that is passed as parameter, if any is passed.
//It will then be returned the item on which the function has been called
//Recycle will take an object to be recycled and a function which we can pass or not.
//if the function is given (we might wanna pass a Reset() function to restore the initial values of the object) it will be called upon the item to be recycled
//then the item will be Enqueued again in the queue ready to be getted again by the Get function.



public class Pool<T>
{
    public Queue<T> pool;
    [SerializeField]
    private T[] prefabs;
    private GameObject parent;
    public delegate T NewFunc(T obj, GameObject parent);
    NewFunc newFunc;

    //Create a pool of items (most likely GameObjects in unity with components attached), takes a prefab with all the components that will be instantiated and a function to instantiate as Params.
    public Pool(T[] prefab, NewFunc func, GameObject parent)
    {
        newFunc = func;
        pool = new Queue<T>();
        this.prefabs = prefab;
        this.parent = parent;
    }

    //Gets the first elements of the queue if there are any otherwise it will invoke the instantiating function on the prefab. It will return the Item on which an action can be called.
    public T Get(Action<T> OnGet = null, int arrayIndex = -1)
    {
        T Item;
        if (pool.Count > 0 && arrayIndex < 0)
        {
            //if there are items in the queue one will be getted
            Item = pool.Dequeue();
        }
        else
        {
            //otherwise a new one will be created
            if (arrayIndex < 0)
            {
                int index = UnityEngine.Random.Range(0, prefabs.Length);
                Item = newFunc(prefabs[index], parent);
            }
            else
            {
                Item = newFunc(prefabs[arrayIndex], parent);
            }
        }
        if (OnGet != null)
        {
            //if a function is passed it will be called on the item before returning it
            OnGet.Invoke(Item);
        }
        return Item;
    }

    //Recycle and enqueue an item upon wich a function can be called
    public void Recycle(T Item, Action<T> OnRecycle = null)
    {
        if (OnRecycle != null)
        {
            OnRecycle.Invoke(Item);
        }
        pool.Enqueue(Item);
    }

    //Generates a first pool of object of numItems calling the instatiating function upn them, an other function func can be called on them if passed (such as setting the gamobject as inactive until it is getted)
    public void AddToQueue(int numItems, Action<T> func = null, int arrayIndex = -1)
    {
        for (int i = 0; i < numItems; i++)
        {
            T Item;
            if (arrayIndex < 0)
            {
                int index = UnityEngine.Random.Range(0, prefabs.Length);
                Item = newFunc(prefabs[index], parent);
            }
            else
            {
                Item = newFunc(prefabs[arrayIndex], parent);
            }
            if (func != null)
                func.Invoke(Item);
            pool.Enqueue(Item);
        }
    }
}