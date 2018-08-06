using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMgr : MonoBehaviour
{
    public Texture2D MouseTexture;

    // Use this for initialization
    void Start()
    {
        //MenuEvents.OnMenuOpen += ShowMouse;
        //MenuEvents.OnMenuClose += HideMouse;

        //HideMouse();

        Cursor.lockState = CursorLockMode.Confined;

        Cursor.SetCursor(MouseTexture, new Vector2(MouseTexture.width / 2f, MouseTexture.height / 2f), CursorMode.Auto);
    }

    void OnDestroy()
    {
        ShowMouse();

        //MenuEvents.OnMenuOpen -= ShowMouse;
        //MenuEvents.OnMenuClose -= HideMouse;
    }

    void ShowMouse()
    {
        Cursor.visible = true;
    }

    void HideMouse()
    {
        Cursor.visible = false;
    }
}
