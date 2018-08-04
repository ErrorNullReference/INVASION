using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMgr : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        MenuEvents.OnMenuOpen += ShowMouse;
        MenuEvents.OnMenuClose += HideMouse;

        HideMouse();

        Cursor.lockState = CursorLockMode.Confined;
    }

    void OnDestroy()
    {
        ShowMouse();

        MenuEvents.OnMenuOpen -= ShowMouse;
        MenuEvents.OnMenuClose -= HideMouse;
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
