using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.IO;


public class HUDManager : MonoBehaviour
{
	//Scripts that can be attached on multiple components for HUD, will display one of the chosen data.

    public enum DisplayDataType
    {
        Health,
        Shield,
        Armor,
        LevelXP_amount,
        LevelXP_lvlNum,
        GunName,
        AmmoHeld,
        AmmoMag,
        LevelName,
        MissionName,
        ObjectiveDescription,
        PlayerImg,
        PlayerName
    }
    ;

    private Enemy enemy;
    public DisplayDataType dataType;
    public HeadsUpDisplay InputAssetHUD;
    public bool OneTimeUpdate;  //Set to true to disable component.

    void Awake()
    {
        if (OneTimeUpdate = true)
        {
            OneTimeUpdate = !OneTimeUpdate;
        }
        enemy=GetComponentInParent<Enemy>();

    }



    void OnValidate()
    {   //activates when the editor's inspector is modified. Future Event mngment feature

    }

    // Update is called once per frame
    void Update()
    {
        if (InputAssetHUD == null)
        {       //in case of missing HUD input, this throws an error and disables the component(to avoid warning spam)
            Debug.LogWarning(gameObject.name + "is missing a HUD Input asset !");
            this.gameObject.GetComponent<HUDManager>().enabled = false;
            return;
        }
        if (OneTimeUpdate)
        {
            this.gameObject.GetComponent<HUDManager>().enabled = false;     //disables the component on second update
            return;
        }

        Slider GOSlider = this.GetComponent<Slider>();
        Text GOText = this.GetComponent<Text>();
        Image GOImg = this.GetComponent<Image>();


        switch (dataType)
        {

            case DisplayDataType.Health:
                if (GOSlider != null)       //HUD 2D text subcase
                    GOSlider.value = enemy.Life / InputAssetHUD.MaxHealth;
                else if (GOImg != null)
                {   // sprite subcase
                    GOImg.color = InputAssetHUD.PlayerHealthBarGradient.Evaluate(enemy.Life / InputAssetHUD.MaxHealth);
                    GOImg.fillAmount = (enemy.Life / InputAssetHUD.MaxHealth);
                }
                else
                {   //Quad subcase 
                    gameObject.GetComponent<MeshRenderer>().material.color = InputAssetHUD.PlayerHealthBarGradient.Evaluate(enemy.Life / InputAssetHUD.MaxHealth);
                }

                break;

            case DisplayDataType.LevelXP_amount:
                GOSlider.value = InputAssetHUD.LevelXP % 1; //what's the max level? 100?
                break;

            case DisplayDataType.LevelXP_lvlNum:
                GOText.text = (InputAssetHUD.LevelXP - InputAssetHUD.LevelXP % 1).ToString();
                break;

            case DisplayDataType.GunName:
                GOText.text = InputAssetHUD.GunName;
                break;

            case DisplayDataType.AmmoHeld:
                if (GOText != null)
                    GOText.text = InputAssetHUD.AmmoHeld.ToString();
                else if (GOImg != null)
                {
                    GOImg.fillAmount = (float)((float)InputAssetHUD.AmmoHeld / (float)InputAssetHUD.MaxAmmo);
                }
                else
                {   //Quad case ( full circle case)
                    gameObject.GetComponent<MeshRenderer>().material.color =
                        InputAssetHUD.PlayerAmmoBarGradient.Evaluate(InputAssetHUD.AmmoHeld / InputAssetHUD.MaxAmmo);
                }
                break;

            case DisplayDataType.AmmoMag:
                GOText.text = InputAssetHUD.AmmoMag.ToString();
                break;

            /*		case DisplayDataType.LevelName:
                        GOText.text = InputAssetHUD.LevelName;
                        OneTimeUpdate = true;
                        break;

                    case DisplayDataType.MissionName:
                        GOText.text = InputAssetHUD.MissionName;
                        OneTimeUpdate = true;
                        break;

                    case DisplayDataType.ObjectiveDescription:
                        GOText.text = InputAssetHUD.ObjectiveDescription;
                        break;
                        */
            case DisplayDataType.PlayerName:
                if (Server.Users.Count <=0)
                {
                    GOText.text = "Player Name";
                }
                else
                    GOText.text = Server.Users[InputAssetHUD.ClientID].SteamUsername;        //get player current name from server
                OneTimeUpdate = true;
                break;
            case DisplayDataType.PlayerImg:
                if (Server.Users.Count > 0)
                    GOImg.material.SetTexture("PlayerAvatarImage", Server.Users[InputAssetHUD.ClientID].SteamAvatarImage);
                OneTimeUpdate = true;
                break;


            default:
                Debug.LogError("Unhandled Exception: DisplayDataType '" + dataType + "' not managed in " + gameObject.name);
                break;
        }


    }
}
