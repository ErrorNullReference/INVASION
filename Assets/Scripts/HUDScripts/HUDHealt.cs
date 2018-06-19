using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDHealt : HUD
{
    private Slider GOSlider;
    private Image GOImg;
    private LivingBeing livingBeing;

    private void Start()
    {
        GOSlider = gameObject.GetComponent<Slider>();
        GOImg    = gameObject.GetComponent<Image>();
        livingBeing = gameObject.GetComponentInParent<LivingBeing>();

    }
    void Update ()
    {
        if (GOSlider != null)     
        {
            //HUD 2D text subcase
            GOSlider.value = livingBeing.Life * InputAssetHUD.InverseMaxHealth;
        }
        else if (GOImg != null)
        {
            // Sprite subcase
            GOImg.color = InputAssetHUD.PlayerHealthBarGradient.Evaluate(livingBeing.Life * InputAssetHUD.InverseMaxHealth);
            GOImg.fillAmount = (livingBeing.Life * InputAssetHUD.InverseMaxHealth);
        }
        else
        {
            //Quad subcase 
            gameObject.GetComponent<MeshRenderer>().material.color = InputAssetHUD.PlayerHealthBarGradient.Evaluate(livingBeing.Life * InputAssetHUD.InverseMaxHealth);
        }

    }
}
