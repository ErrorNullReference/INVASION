using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.IO;
using SOPRO;

//Scripts that can be attached on multiple components for HUD, will display one of the chosen data.
public enum DisplayDataType
{
    Health,
    Score,
    AmmoHeld,
    AmmoMag,
    PlayerImg,
    PlayerName
    //  Shield,
    //    Armor,
    //    LevelXP_amount,
    //   LevelXP_lvlNum,
    //   GunName,
    //   LevelName,
    //   MissionName,
    //   ObjectiveDescription,
}
    
public class HUDManager : MonoBehaviour
{
    public SOListPlayerContainer DataContainer;
    public HeadsUpDisplay InputAssetHUD;

    [SerializeField]
    private DisplayDataType dataType;

    private LivingBeing livingBeing;
    private Slider GOSlider;
    private Image GOImg;
    private Text GOText;

    void Awake()
    {
        livingBeing=gameObject.GetComponentInParent<LivingBeing>();
        GOText = gameObject.GetComponent<Text>();
        GOSlider = this.GetComponent<Slider>();
        GOImg = this.GetComponent<Image>();
    }

    void Update()
    {
        switch (dataType)
        {
            case DisplayDataType.Health:

                if (GOSlider != null)       //HUD 2D text subcase
                {
                    GOSlider.value = livingBeing.Life / InputAssetHUD.MaxHealth;
                }
                else if (GOImg != null)
                {  
                    // sprite subcase
                    GOImg.color      = InputAssetHUD.PlayerHealthBarGradient.Evaluate(livingBeing.Life / InputAssetHUD.MaxHealth);
                    GOImg.fillAmount = (livingBeing.Life / InputAssetHUD.MaxHealth);
                }
                else
                {   
                    //Quad subcase 
                    gameObject.GetComponent<MeshRenderer>().material.color = InputAssetHUD.PlayerHealthBarGradient.Evaluate(livingBeing.Life / InputAssetHUD.MaxHealth);
                }

                break;

            /*     case DisplayDataType.LevelXP_amount:
                     GOSlider.value = InputAssetHUD.LevelXP % 1; //what's the max level? 100?
                     break;

                 case DisplayDataType.LevelXP_lvlNum:
                     GOText.text = (InputAssetHUD.LevelXP - InputAssetHUD.LevelXP % 1).ToString();
                     break;

                 case DisplayDataType.GunName:
                     GOText.text = InputAssetHUD.GunName;
                     break;
                 */

            case DisplayDataType.AmmoHeld:

                if (GOText != null)
                {
                    GOText.text = InputAssetHUD.AmmoHeld.ToString();
                }
                else if (GOImg != null)
                {
                    GOImg.fillAmount = (float)((float)InputAssetHUD.AmmoHeld / (float)InputAssetHUD.MaxAmmo);
                }
                else
                {
                    //Quad case ( full circle case)
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
                if (Server.Users.Count <= 0)
                    GOText.text = "Player Name";
                else
                    GOText.text = Server.Users[InputAssetHUD.ClientID].SteamUsername;        //get player current name from server
                break;

            case DisplayDataType.PlayerImg:

                if (Server.Users.Count > 0)
                    GOImg.material.SetTexture("PlayerAvatarImage", Server.Users[InputAssetHUD.ClientID].SteamAvatarImage);
                break;

            case DisplayDataType.Score:

                for (int i = 0; i < DataContainer.Elements.Count; i++)
                {
                    if (DataContainer.Elements[i].Avatar.UserInfo.SteamID == Client.MyID)
                    {
                        GOText.text = DataContainer.Elements[i].TotalPoints.ToString();
                    }
                }
                break;

            default:                
                break;
        }


    }
}
