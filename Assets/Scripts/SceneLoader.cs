using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static UnityAction<Scene, LoadSceneMode> OnLoad;
    public int SceneID;
    public bool Asyinc, LoadOnStart;

    // Use this for initialization
    void Start()
    {
        if (LoadOnStart)
            StartCoroutine(Loading());
    }

    IEnumerator Loading()
    {
        yield return null;
        Load();
    }

    public void Load()
    {
        SceneManager.sceneLoaded += CallOnLoad;

        if (Asyinc)
            SceneManager.LoadSceneAsync(SceneID);
        else
            SceneManager.LoadScene(SceneID);
    }

    void CallOnLoad(Scene s, LoadSceneMode ls)
    {
        if (OnLoad != null)
            OnLoad.Invoke(s, ls);

        SceneManager.sceneLoaded -= CallOnLoad;
    }
}
