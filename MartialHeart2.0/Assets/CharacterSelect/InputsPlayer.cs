using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class InputsPlayer : MonoBehaviour
{
    [SerializeField] InputActionAsset actions;

    public GameObject SelectorCharacterPrefab;

    [HideInInspector]
    public PlayerInput input;


    GameObject selectorGO;

    void Awake()
    {
        input = GetComponent<PlayerInput>();

        GameObject parent = GameObject.Find("CharacterSelectManager");

        if (parent != null)
        {
            selectorGO = Instantiate(SelectorCharacterPrefab, parent.transform);
            selectorGO.GetComponent<SelectorCharacter>().inputAction = gameObject;
            InputSystemUIInputModule inputUI = selectorGO.GetComponent<InputSystemUIInputModule>();
            input.uiInputModule = inputUI;
            //inputUI.actionsAsset = actions;

        }
    }

    /*
    menu.GetComponent<SelectorCharacter>().nPlayer = input.playerIndex + 1;

    if (menu.GetComponent<SelectorCharacter>().nPlayer == 2)
    {
        SelectorCharacter[] selectors = FindObjectsOfType<SelectorCharacter>();
        foreach (SelectorCharacter selector in selectors)
            selector.CancelCPU();
    }
    */
}
