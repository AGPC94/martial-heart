using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AddEventTriggerToButton : MonoBehaviour
{
    enum Type { MoveAndSubmit, Move  }
    [SerializeField] Type type;

    EventTrigger eventTrigger;

    void Awake()
    {
        AddEventTriggerByScript();
        switch (type)
        {
            case Type.MoveAndSubmit:
                AddMoveSound();
                AddSubmitSound();
                break;
            case Type.Move:
                AddMoveSound();
                break;
        }
    }

    void AddEventTriggerByScript()
    {
        //Component
        if (GetComponent<EventTrigger>() == null)
            gameObject.AddComponent<EventTrigger>();
        eventTrigger = GetComponent<EventTrigger>();
    }

    void AddMoveSound()
    {
        //Move
        EventTrigger.Entry entryMove = new EventTrigger.Entry();

        entryMove.eventID = EventTriggerType.Move;

        entryMove.callback.AddListener((functionIWant) => { PlaySound("Move"); });

        eventTrigger.triggers.Add(entryMove);
    }
    void AddSubmitSound()
    {
        //Submit
        EventTrigger.Entry entrySubmit = new EventTrigger.Entry();

        entrySubmit.eventID = EventTriggerType.Submit;

        entrySubmit.callback.AddListener((functionIWant2) => { PlaySound("Submit"); });

        eventTrigger.triggers.Add(entrySubmit);
    }

    void PlaySound(string sound)
    {
        AudioManagerBrackeys.instance.Play(sound);
    }
}
