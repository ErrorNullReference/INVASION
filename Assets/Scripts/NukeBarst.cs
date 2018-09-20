using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class NukeBarst : MonoBehaviour
{

    private PostProcessingBehaviour post;
    public bool Barst;
    public BloomModel.Settings UpdatedSetting;
    public AnimationCurve IntesnityCurve;
    public AnimationCurve DeltaTimeCurve;

    private float AnimationTime;
    // Use this for initialization
    void Start()
    {
        post = GetComponent<PostProcessingBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Barst)
        {
            if (AnimationTime <= 1)
            {
                AnimationTime += Time.deltaTime;
            }
            if (AnimationTime >= 1)
            {
                Barst = false;
            }
        }
        Time.timeScale = DeltaTimeCurve.Evaluate(AnimationTime);
        if (!Barst)
        {
            if (AnimationTime >= 0)
            {
                AnimationTime -= Time.deltaTime;
            }
        }
        UpdatedSetting.bloom.intensity = IntesnityCurve.Evaluate(AnimationTime);
        UpdatedSetting.bloom.antiFlicker = true;
        post.profile.bloom.settings = UpdatedSetting;

    }
}
