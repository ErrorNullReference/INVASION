using UnityEngine;
using System.Collections;
using SOPRO;
public class Player : LivingBeing
{
    [HideInInspector]
    public Collider PlayerCollider;
    [SerializeField]
    private SOListPlayerContainer players;
    int dropLayer = 11;

    public float Energy;
    private void Start()
    {       
        GetComponentInChildren<HUDManager>().InputAssetHUD = Stats;
        PlayerCollider = GetComponentInChildren<Collider>();
        Life = Stats.MaxHealth;
    }
    private void OnEnable()
    {
        players.Elements.Add(this);
    }
    private void OnDisable()
    {
        players.Elements.Remove(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer==dropLayer)
        {
            if(collision.gameObject.GetComponent<EnergyDrop>()!=null)
            {
                Energy += collision.gameObject.GetComponent<EnergyDrop>().EnergyAmount;
                if (Energy > Stats.MaxEnergy)
                    Energy = Stats.MaxEnergy;
                Destroy(collision.gameObject);
            }
        }
    }
    //CHANGED, FOR NOW ONLY ONE CAMERA WILL BE USED.

    //public Camera[] cameras;
    //private int currentCameraIndex;

    //void Start()
    //{
    //    currentCameraIndex = 0;
    //    for (int i = 0; i < cameras.Length; i++)
    //    {
    //        cameras[i].gameObject.SetActive(false);
    //    }
    //    if (cameras.Length > 0)
    //    {
    //        cameras[0].gameObject.SetActive(true);
    //    }
    //}

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.R))
    //    {
    //        currentCameraIndex++;

    //        if (currentCameraIndex < cameras.Length)
    //        {
    //            cameras[currentCameraIndex - 1].gameObject.SetActive(false);
    //            cameras[currentCameraIndex].gameObject.SetActive(true);
    //        }
    //        else
    //        {
    //            cameras[currentCameraIndex - 1].gameObject.SetActive(false);
    //            currentCameraIndex = 0;
    //            cameras[currentCameraIndex].gameObject.SetActive(true);
    //        }
    //    }
    //}
}