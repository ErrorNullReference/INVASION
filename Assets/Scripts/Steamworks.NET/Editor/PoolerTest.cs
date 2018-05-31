using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class PoolerTest {

    [UnityTest]
    public IEnumerator AddToQueueTest()
    {
        GameObject[] obj = new GameObject[] { new GameObject() };
        GameObject obj1 = new GameObject();
        Pool<GameObject> objs = new Pool<GameObject>(obj, EnemySpawner.Instance.EnemyCreation, obj1);
        objs.AddToQueue(20, cb => { cb.SetActive(false); });

        bool AllInactive = true;
        foreach(GameObject o in objs.pool)
        {
            if(o.activeInHierarchy)
            {
                AllInactive = false;
                break;
            }
            else
            {
                AllInactive = true;
            }            
        }
        Assert.That(AllInactive, Is.EqualTo(true));
        yield return null;
    }

    [UnityTest]
    public IEnumerator AddToQueueTestQuantity()
    {
        GameObject[] obj = new GameObject[] { new GameObject() };
        GameObject obj1 = new GameObject();
        Pool<GameObject> objs = new Pool<GameObject>(obj, EnemySpawner.Instance.EnemyCreation, obj1);
        objs.AddToQueue(20, cb => { cb.SetActive(false); });

        Assert.That(objs.pool.Count, Is.EqualTo(20));
        yield return null;
    }

    [UnityTest]
    public IEnumerator PoolGetTest()
    {
        GameObject[] obj = new GameObject[] { new GameObject() };
        GameObject obj1 = new GameObject();
        Pool<GameObject> objs = new Pool<GameObject>(obj, EnemySpawner.Instance.EnemyCreation, obj1);
        objs.AddToQueue(20, cb => { cb.SetActive(false); });
        GameObject o = objs.Get(cb => { cb.SetActive(true); });

        Assert.That(o.activeInHierarchy, Is.EqualTo(true));
        yield return null;
    }

    [UnityTest]
    public IEnumerator PoolGetTest2()
    {
        GameObject[] obj = new GameObject[] { new GameObject() };
        GameObject obj1 = new GameObject();
        Pool<GameObject> objs = new Pool<GameObject>(obj, EnemySpawner.Instance.EnemyCreation, obj1);
        objs.AddToQueue(20, cb => { cb.SetActive(false); });
        GameObject o = objs.Get(cb => { cb.SetActive(true); });

        Assert.That(!objs.pool.Contains(o));
        yield return null;
    }
    [UnityTest]
    public IEnumerator PoolGetTest3()
    {
        GameObject[] obj = new GameObject[] { new GameObject() };
        GameObject obj1 = new GameObject();
        Pool<GameObject> objs = new Pool<GameObject>(obj, EnemySpawner.Instance.EnemyCreation, obj1);
        GameObject o = objs.Get(cb => { cb.SetActive(true); });

        Assert.That(o.activeInHierarchy, Is.EqualTo(true));
        yield return null;
    }

    [UnityTest]
    public IEnumerator RecycleTest()
    {
        GameObject[] obj = new GameObject[] { new GameObject() };
        GameObject obj1 = new GameObject();
        Pool<GameObject> objs = new Pool<GameObject>(obj, EnemySpawner.Instance.EnemyCreation, obj1);
        GameObject o = objs.Get(cb => { cb.SetActive(true); });
        objs.Recycle(o, cb => { cb.SetActive(false); });
        Assert.That(o.activeInHierarchy, Is.EqualTo(false));
        yield return null;
    }

    [UnityTest]
    public IEnumerator RecycleTest2()
    {
        GameObject[] obj = new GameObject[] { new GameObject() };
        GameObject obj1 = new GameObject();
        Pool<GameObject> objs = new Pool<GameObject>(obj, EnemySpawner.Instance.EnemyCreation, obj1);
        GameObject o = objs.Get(cb => { cb.SetActive(true); });
        objs.Recycle(o, cb => { cb.SetActive(false); });
        Assert.That(objs.pool.Contains(o));
        yield return null;
    }
}

	

