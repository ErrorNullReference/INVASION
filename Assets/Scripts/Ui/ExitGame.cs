using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ExitGame : MonoBehaviour {

    public void Exit()
    {
        Debug.Log("exit");

        UnityEditor.EditorApplication.isPlaying = false;

        Application.Quit();
    }
}
