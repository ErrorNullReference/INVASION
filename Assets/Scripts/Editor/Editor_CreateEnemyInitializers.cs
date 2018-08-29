using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Editor_CreateEnemyInitializers : MonoBehaviour
{
    [MenuItem("GameObject/Create Initializer")]
    public static void NewInitializer()
    {   
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject o = Selection.gameObjects[i];
            if (o.GetComponent<Animator>() == null)
                continue;

            EnemyInitializer init = ScriptableObject.CreateInstance<EnemyInitializer>();
            init.name = o.name;
            init.Body = o;
            string name = init.name + ".asset";
            string path = System.IO.Path.Combine("Assets", name);
            AssetDatabase.CreateAsset(init, path);
        }
    }
}
