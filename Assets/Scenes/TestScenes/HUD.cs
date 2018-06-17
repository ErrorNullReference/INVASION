using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SOPRO;

public abstract class HUD : MonoBehaviour
{
    public SOListPlayerContainer DataContainer;
    public HeadsUpDisplay InputAssetHUD;

    protected Text textComponent;

	void Start ()
    {
        try
        {
            textComponent = gameObject.GetComponent<Text>();
        }
        catch
        {
        }
	}
	
}
