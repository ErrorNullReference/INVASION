using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadFade : MonoBehaviour
{
    [SerializeField]
    private float Duration;

    private Texture2D texture;
    Rect rect;
    bool drawTexture;
    Color color;
    Color[] colors;

    // Use this for initialization
    void Start()
    {
        rect = new Rect(0, 0, Screen.width, Screen.height);

        texture = new Texture2D(250, 250, TextureFormat.RGBAFloat, false);

        color = Color.black;
        colors = new Color[texture.width * texture.height];
        SetPixels();

        drawTexture = true;
    }

    void Update()
    {
        if (!drawTexture)
            Destroy(this);
        
        color.a -= Time.deltaTime / Duration;
        if (color.a <= 0)
        {
            color.a = 1;
            drawTexture = false;
        }
        SetPixels();
    }

    void OnGUI()
    {
        if (drawTexture)
            GUI.DrawTexture(rect, texture);
    }

    void SetPixels()
    {
        for (int i = 0; i < colors.Length; i++)
            colors[i] = color;
        texture.SetPixels(colors);
        texture.Apply();   
    }
}
