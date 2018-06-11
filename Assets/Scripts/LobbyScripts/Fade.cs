using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    //public Graphic GraphicComponent;
    public int MaxAlpha, MinAlpha;

    void OnEnable()
    {
        FadeDown();
    }

    public void FadeUp()
    {
        int num = transform.childCount;
        for (int i = 0; i < num; i++)
        {
            Graphic g = transform.GetChild(i).GetComponent<Graphic>();
            if (g != null)
                g.color = new Color(g.color.r, g.color.g, g.color.b, MaxAlpha / 255f);
        }
        //GraphicComponent.color = new Color(GraphicComponent.color.r, GraphicComponent.color.g, GraphicComponent.color.b, MaxAlpha / 255f);
    }

    public void FadeDown()
    {
        int num = transform.childCount;
        for (int i = 0; i < num; i++)
        {
            Graphic g = transform.GetChild(i).GetComponent<Graphic>();
            if (g != null)
                g.color = new Color(g.color.r, g.color.g, g.color.b, MinAlpha / 255f);
        }
        //GraphicComponent.color = new Color(GraphicComponent.color.r, GraphicComponent.color.g, GraphicComponent.color.b, MinAlpha / 255f);
    }
}
