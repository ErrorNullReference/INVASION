using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "HUD_Data", menuName = "User Interface/Hud data Input")]
public class HeadsUpDisplay : ScriptableObject
{

    //public bool SerializeMe;
    [SerializeField]
    private float maxHealth;
    [SerializeField]
    private float inverseMaxHealth;
    public float MaxHealth { get { return maxHealth; } }
    public float InverseMaxHealth { get { return inverseMaxHealth; } }

    //[Range 0-99]
    /// <summary>
    /// The experience levels  points of the client (i.e lvl 1 = 1.00 , lvl 3 and half = 3.5,  lvl 4 with 76.5 % xp = 4.765)
    /// </summary>
    //public float LevelXP;
    /// <summary>
    /// The name of the gun.
    /// </summary>
    //public string GunName;
    /// <summary>
    /// The ammo held.
    /// </summary>
    [SerializeField]
    private int maxEnergy;
    [SerializeField]
    private float inverseMaxEnergy;
    public int MaxEnergy { get { return maxEnergy; } }
    public float InverseMaxEnergy { get { return inverseMaxEnergy; } }
    /// <summary>
    /// The ammo in gun magazine
    /// </summary>
    //public int AmmoMag;

    //	public string LevelName;
    //	public string MissionName;
    //	public string ObjectiveDescription;
    public Gradient PlayerHealthBarGradient;
    public Gradient PlayerAmmoBarGradient;

    void OnValidate()
    {
        inverseMaxEnergy = Mathf.Approximately(0f, maxEnergy) ? 0f : 1f / maxEnergy;
        inverseMaxHealth = Mathf.Approximately(0f, maxHealth) ? 0f : 1f / maxHealth;
    }
    //[SerializeField]
    //private string filename = "Client Game Info";//name of the ScriptableObj that saves client info (energy,ammo,etc). Initialized to avoid bugs on serialization.

    //   private void OnEnable ()	//serialization
    //{
    //	if (!SerializeMe)
    //		return;
    //	string filePath = Path.Combine (Application.streamingAssetsPath, filename);

    //	if (File.Exists (filePath)) {
    //		string json = File.ReadAllText (filePath);
    //		JsonUtility.FromJsonOverwrite (json, this);
    //	} else {
    //		string json = JsonUtility.ToJson (this);
    //		File.WriteAllText (filePath, json); 	//creates new file if doesn't exist
    //	}
    //}

    //public static void LoadFromHUDData(HeadsUpDisplay PrimitiveData,HeadsUpDisplay newHUDData){
    //	JsonUtility.FromJsonOverwrite (JsonUtility.ToJson (PrimitiveData), newHUDData);
    //}

}
