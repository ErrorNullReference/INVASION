using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SetButtonOnFriendList : MonoBehaviour
{
    Button button;
    MenuMgr mgr;

    // Use this for initialization
    void Start()
    {
        button = GetComponent<Button>();
        mgr = FindObjectOfType<MenuMgr>();

        button.onClick.AddListener(() =>
            {
                if (mgr != null)
                {
                    mgr.OpenFriendList();
                }
            });
    }
}
