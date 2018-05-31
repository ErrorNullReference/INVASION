using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentFitter : MonoBehaviour
{
    public RectTransform Viewport;
    RectTransform rect;

    void OnEnable()
    {
        if (rect == null)
            rect = GetComponent<RectTransform>();
    }

    public void Init()
    {
        float height = 0;
        int num = transform.childCount;
        for (int i = 0; i < num; i++)
        {   
            Transform t = transform.GetChild(i);
            if (!t.gameObject.activeSelf)
                continue;
            RectTransform r = t.GetComponent<RectTransform>();
            if (r != null)
                height += r.rect.height;
        }
        if (height < Viewport.rect.height)
            height = Viewport.rect.height;
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -rect.rect.height / 2f);
    }
}
