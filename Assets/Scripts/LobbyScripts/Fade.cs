using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Fade : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Graphic GraphicComponent;
    [Range(0, 255)]
    public int MaxAlpha, MinAlpha;

    void OnEnable()
    {
        GraphicComponent = GetComponent<Graphic>();

        FadeDown();
    }

    public void FadeUp()
    {
        GraphicComponent.color = new Color(GraphicComponent.color.r, GraphicComponent.color.g, GraphicComponent.color.b, MaxAlpha / 255f);
    }

    public void FadeDown()
    {
        GraphicComponent.color = new Color(GraphicComponent.color.r, GraphicComponent.color.g, GraphicComponent.color.b, MinAlpha / 255f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        FadeUp();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        FadeDown();
    }
}
