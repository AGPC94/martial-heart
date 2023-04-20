using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
{
    public Stage stage;

    [SerializeField] Text txtStage;
    [SerializeField] Image imgStage;

    PlayerInputActions playerInputActions;
    [SerializeField] PlayerConfiguration playerConfiguration;


    void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.SelectCharacter.Disable();
        playerInputActions.SelectStage.Enable();
    }
    void Start()
    {
        foreach (PlayerConfiguration item in SelectorCharacterSpawnManager.instance.GetPlayerConfigs().ToArray())
            if (item.PlayerIndex == 0)
                playerConfiguration = item;

        //Asignar propiedades del jugador desde su configuración

        playerConfiguration.Input.onActionTriggered += Input_onActionTriggered;

        AudioManagerBrackeys.instance.PlayMusic("MainMenu");

        Debug.Log(playerConfiguration.Input.playerIndex);
    }

    
    void Input_onActionTriggered(InputAction.CallbackContext context)
    {
        if (context.action.name == playerInputActions.SelectStage.Cancel.name)
            Cancel(context);
    }
    

    // Update is called once per frame
    void Update()
    {
        if (stage != null)
            ShowStage(stage);
    }

    public void ShowStage(Stage stage)
    {
        txtStage.text = stage.name;
        imgStage.sprite = stage.preview;
    }

    public void SelectStage(StageButton sb)
    {
        stage = sb.stage;
        GameManager.instance.SetStage(stage);
    }

    public void LoadStage()
    {
        playerConfiguration.Input.onActionTriggered -= Input_onActionTriggered;
        playerInputActions.SelectStage.Disable();
        LevelLoader.instance.LoadLevel("Arena");
    }
    public void BackToCharacterSelect()
    {
        playerConfiguration.Input.onActionTriggered -= Input_onActionTriggered;
        Destroy(GameObject.Find("PlayerInputManager"));
        AudioManagerBrackeys.instance.Play("Back");
        LevelLoader.instance.LoadLevel("CharacterSelect");
    }

    public void Cancel(InputAction.CallbackContext context)
    {
        BackToCharacterSelect();
    }
}
