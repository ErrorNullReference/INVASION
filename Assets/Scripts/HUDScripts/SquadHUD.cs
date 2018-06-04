using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquadHUD : MonoBehaviour
{
    //pressing "Z" or other set buttons, show the HUD with the status of the team mates.

    public KeyCode ShowSquadHUDKey;
    private bool SHUDShowing;
   // public int ClientID;

    [SerializeField]
    private HeadsUpDisplay DefHUDInfo;
    private HeadsUpDisplay[] HUDInfos;
    [SerializeField]
	/// <summary>
	/// The HUD prefab canvas: found in hudprefrabs/canvas HUD player (1). must be placed in the scene.
	/// </summary>
    private Canvas HUDPrefabCanvas;

    //public Mask mask;

	//dentro coroutine (ienumerator)
	/*while(true)
	 * 
	 * if( user list = null)
	 *  is null , no instantiation
	 * else
	 *  instantiate hud piece
	 * 
	 * */
    void Awake()
    {
		
        HUDInfos = new HeadsUpDisplay[4];
        for (int i = 0; i < 4; i++)
        {
            HUDInfos[i] = DefHUDInfo;
			HUDInfos[i].name= "Player "+ i+ " HUD data";
        }

		StartCoroutine (InitClientHUD());
    }

    void Update()
    {

        /*      //hideOnKey future option
                if(Input.GetKeyDown(ShowSquadHUDKey)){

                    SHUDShowing= !SHUDShowing;
                    if (!SHUDShowing) {
                        //show other player HUDs
                       
                    } else {
                        //hide

                    }
                }*/

		// HUD UPDATE 
		foreach (var player in Server.Users) {
			
		}

    }
	IEnumerator InitClientHUD(){
		while (true) {
			if (Server.Users != null) {

				for (int i = 0; i < Server.Users.Count; i++) {
					var newHUD = Instantiate (HUDPrefabCanvas, this.transform);	//create HUD part and set parent to main 2dHUD canvas
					foreach (HUDManager TwoDItem in newHUD.GetComponentsInChildren<HUDManager>()) {
						TwoDItem.InputAssetHUD = HUDInfos [i];
						TwoDItem.InputAssetHUD.ClientID = i;
						Debug.Log("Generated HUD piece for Player "+Server.Users[TwoDItem.InputAssetHUD.ClientID].SteamUsername+", client number: "+TwoDItem.InputAssetHUD.ClientID);
					}
				}

				yield break;
			} else {
				yield return null;
			}


		}
	}
}
