using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButtonsMgr : MonoBehaviour
{
    public Button[] Buttons;

    void OnEnable()
    {
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].onClick.AddListener(DisableButtons);
        }

        EnableButtons();
    }

    public void DisableButtons()
    {
        for (int i = 0; i < Buttons.Length; i++)
            Buttons[i].interactable = false;
    }

    public void EnableButtons()
    {
        for (int i = 0; i < Buttons.Length; i++)
            Buttons[i].interactable = true;
    }
}
