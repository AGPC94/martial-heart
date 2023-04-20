using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectorMultiplayer : MonoBehaviour
{
    Image imgSelector;
    EventTrigger eventTrigger;

    void Awake()
    {
        imgSelector = transform.Find("Selector").GetComponent<Image>();

        if (GetComponent<EventTrigger>() == null)
            gameObject.AddComponent<EventTrigger>();
        eventTrigger = GetComponent<EventTrigger>();

        EventTrigger.Entry entryMove = new EventTrigger.Entry();

        entryMove.eventID = EventTriggerType.Deselect;

        entryMove.callback.AddListener((functionIWant) => { NoneImg(); });

        eventTrigger.triggers.Add(entryMove);
    }

    void Start()
    {
        NoneImg();
    }

    public void Select()
    {
        imgSelector.gameObject.SetActive(true);
    }

    public void NoneImg()
    {
        imgSelector.gameObject.SetActive(false);
    }
}
