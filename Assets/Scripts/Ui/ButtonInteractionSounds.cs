using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(EventTrigger))]
public class ButtonInteractionSounds : MonoBehaviour
{
    public AudioClip OnOver, OnClick;
    AudioSource ASource;
    EventTrigger trigger;

    // Use this for initialization
    void Start()
    {
        ASource = GetComponent<AudioSource>();
        trigger = GetComponent<EventTrigger>();

        //SET ON ENTER
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) =>
            {
                Enter((PointerEventData)data);
            });
        trigger.triggers.Add(entry);

        //SET ON CLICK
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) =>
            {
                Click((PointerEventData)data);
            });
        trigger.triggers.Add(entry);
    }

    void SetClip(AudioClip clip)
    {
        ASource.clip = clip;
    }

    void Enter(PointerEventData data)
    {
        SetClip(OnOver);
        ASource.Play();
    }

    void Click(PointerEventData data)
    {
        SetClip(OnClick);
        ASource.Play();
    }
}
