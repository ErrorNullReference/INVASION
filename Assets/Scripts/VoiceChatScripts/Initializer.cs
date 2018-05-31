using UnityEngine;
[RequireComponent(typeof(IAudioTransportLayer))]
public class Initializer : MonoBehaviour
{
    [SerializeField]
    private VoiceDataWorkflow manager;
    void Update()
    {
        if (SteamManager.Initialized)
        {
            manager.Init(new SteamVoiceDataManipulator(), GetComponent<IAudioTransportLayer>());
            Destroy(this);
        }
    }
}