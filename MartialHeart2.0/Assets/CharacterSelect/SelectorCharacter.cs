using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SelectorCharacter : MonoBehaviour
{
    public PlayerCharacter playerCharacter;

    public int nPlayer;

    public GameObject inputAction;

    GameObject selectedGO;

    EventSystem eventSystem;
    PlayerInputActions playerInputActions;
    PlayerConfiguration playerConfiguration;

    void Awake()
    {
        nPlayer = SelectorCharacterSpawnManager.instance.inputManager.playerCount;


        PlayerCharacter[] playerCharacters = FindObjectsOfType<PlayerCharacter>();
        foreach (PlayerCharacter item in playerCharacters)
            if (item.nPlayer == nPlayer)
                playerCharacter = item;

        playerCharacter.state = SelectState.PLAYER_CHARACTER;

        playerInputActions = new PlayerInputActions();
        playerInputActions.SelectCharacter.Enable();

        eventSystem = GetComponent<EventSystem>();

        eventSystem.firstSelectedGameObject = playerCharacter.lastBtnSelected.gameObject;


    }

    void Start()
    {
        foreach (PlayerConfiguration item in SelectorCharacterSpawnManager.instance.GetPlayerConfigs().ToArray())
            if (item.PlayerIndex == nPlayer -1)
                playerConfiguration = item;

        //Asignar propiedades del jugador desde su configuración

        playerConfiguration.Input.onActionTriggered += Input_onActionTriggered;



    }

    void Input_onActionTriggered(InputAction.CallbackContext context)
    {
        if (context.action.name == playerInputActions.SelectCharacter.Move.name)
        {
            Move(context);
        }
        if (context.action.name == playerInputActions.SelectCharacter.Submit.name)
        {
            Submit(context);
        }
        if (context.action.name == playerInputActions.SelectCharacter.Cancel.name)
        {
            Cancel(context);
        }
        if (context.action.name == playerInputActions.SelectCharacter.Pause.name)
        {
            Pause(context);
        }
    }

    // Update is called once per frame
    void Update()
    {
        SelectButton();
    }

    void SelectButton()
    {
        selectedGO = eventSystem.currentSelectedGameObject;

        if (selectedGO.TryGetComponent(out CharacterButton characterButton))
        {
            Button btn = selectedGO.GetComponent<Button>();
            playerCharacter.lastBtnSelected = btn;
            playerCharacter.character = characterButton.character;
            
            playerCharacter.AvoidRepeatSkin();
            characterButton.Select(this, playerCharacter);
        }

        if (selectedGO.TryGetComponent(out SelectorMultiplayer selectorMultiplayer))
        {
            Button btn = selectedGO.GetComponent<Button>();
            playerCharacter.lastBtnSelected = btn;
            selectorMultiplayer.Select( );
        }
    }

    void SelectChar(Character character)
    {
        //currentBtn.onClick.RemoveAllListeners();
        //Debug.Log("Ha pulsado el botón " + name);

        if (playerCharacter.state == SelectState.PLAYER_CHARACTER)
        {
            SelectorCharacterSpawnManager.instance.SetPlayerCharacter(nPlayer - 1, character);
            playerCharacter.state = SelectState.PLAYER_COLOR;
            eventSystem.SetSelectedGameObject(playerCharacter.sldColor.gameObject);
        }

        
    }

    public void CancelCPU()
    {
        //if (nPlayer == 1 && SelectorCharacterSpawnManager.instance.inputManager.playerCount == 1)

        PlayerCharacter[] playerCharacters = FindObjectsOfType<PlayerCharacter>();

        foreach (PlayerCharacter item in playerCharacters)
            if (item.nPlayer == nPlayer)
                playerCharacter = item;

        playerCharacter.state = SelectState.PLAYER_CHARACTER;
        eventSystem.SetSelectedGameObject(playerCharacter.lastBtnSelected.gameObject);
    }

    #region Inputs
    public void Move(InputAction.CallbackContext context)
    {
        Vector2 VectorMove = context.ReadValue<Vector2>();

        if (context.started)
        {
            if (playerCharacter.state == SelectState.PLAYER_COLOR)
            {
                if (VectorMove.x > 0)
                    playerCharacter.NextSkin();
                if (VectorMove.x < 0)
                    playerCharacter.PreviousSkin();
            }
        }
    }

    public void Submit(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            switch (playerCharacter.state)
            {
                case SelectState.PLAYER_CHARACTER:
                    if (selectedGO.TryGetComponent(out CharacterButton characterButton))
                    {
                        SelectorCharacterSpawnManager.instance.SetPlayerCharacter(playerCharacter.nPlayer - 1, playerCharacter.character);

                        playerCharacter.state = SelectState.PLAYER_COLOR;
                        eventSystem.SetSelectedGameObject(playerCharacter.sldColor.gameObject);
                        
                        AudioManagerBrackeys.instance.Play("Submit");
                    }
                    break;

                case SelectState.PLAYER_COLOR:

                    //SelectorCharacterSpawnManager.instance.SetPlayerColor(nPlayer - 1, playerCharacter.skinIndex);

                    if (SelectorCharacterSpawnManager.instance.inputManager.playerCount == 1)
                    {
                        if (nPlayer == 1 && playerCharacter.nPlayer == 2)
                        {
                            //Ambos jugadores pasan a seleccionar el stage
                            Debug.Log("Pasar a la stage select desde el character select");

                            SelectorCharacter[] playerSelectors = FindObjectsOfType<SelectorCharacter>();
                            foreach (SelectorCharacter playerSelector in playerSelectors)
                                playerSelector.DisableInputs();

                            SelectorCharacterSpawnManager.instance.SetPlayerColor(playerCharacter.nPlayer - 1, playerCharacter.skinIndex);
                            LevelLoader.instance.LoadLevel("StageSelect");

                            AudioManagerBrackeys.instance.Play("Submit");
                        }
                        else
                        {
                            //Pasa a alegir la CPU
                            PlayerCharacter[] playerCharacters = FindObjectsOfType<PlayerCharacter>();

                            foreach (PlayerCharacter item in playerCharacters)
                                if (item.nPlayer != nPlayer)
                                    playerCharacter = item;

                            SelectorCharacterSpawnManager.instance.GetPlayerConfigs().Add(new PlayerConfiguration(1));

                            playerCharacter.state = SelectState.PLAYER_CHARACTER;
                            eventSystem.SetSelectedGameObject(playerCharacter.lastBtnSelected.gameObject);
                            
                            AudioManagerBrackeys.instance.Play("Submit");
                        }
                    }

                    if (SelectorCharacterSpawnManager.instance.inputManager.playerCount == 2)
                    {
                        SelectorCharacter[] playerSelectors = FindObjectsOfType<SelectorCharacter>();

                        foreach (SelectorCharacter playerSelector in playerSelectors)
                            if (playerSelector.playerCharacter.state != SelectState.PLAYER_COLOR)
                                return;

                        Debug.Log("Pasar a la stage select desde el character select");

                        playerInputActions.SelectCharacter.Disable();

                        foreach (SelectorCharacter playerSelector in playerSelectors)
                            playerSelector.DisableInputs();
                        
                        LevelLoader.instance.LoadLevel("StageSelect");

                        AudioManagerBrackeys.instance.Play("Submit");
                    }
                    break;
            }
        }
    }

    public void Cancel(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            switch (playerCharacter.state)
            {
                case SelectState.PLAYER_CHARACTER:
                    if (nPlayer == 1)
                    {
                        if (playerCharacter.nPlayer == 1)
                        {
                            CharacterSelectManager.instance.BackToMainMenu();
                            
                            AudioManagerBrackeys.instance.Play("Back");
                        }

                        //Cambiar de elegir CP1 a elegir jugador 1 (Player Character 1)
                        if (playerCharacter.nPlayer == 2)
                        {
                            SelectorCharacterSpawnManager.instance.GetPlayerConfigs().RemoveAt(1);
                            PlayerCharacter[] playerCharacters = FindObjectsOfType<PlayerCharacter>();

                            foreach (PlayerCharacter item in playerCharacters)
                                if (item.nPlayer == nPlayer)
                                    playerCharacter = item;

                            playerCharacter.state = SelectState.PLAYER_COLOR;
                            eventSystem.SetSelectedGameObject(playerCharacter.sldColor.gameObject);

                            AudioManagerBrackeys.instance.Play("Back");
                        }
                    }

                    //Desconectar Jugador 2
                    if (nPlayer == 2)
                    {
                        if (playerCharacter.nPlayer == 2)
                        {
                            eventSystem.SetSelectedGameObject(null);
                            Destroy(inputAction.gameObject);
                            Destroy(gameObject);
                            AudioManagerBrackeys.instance.Play("Back");
                        }
                    }


                    break;

                case SelectState.PLAYER_COLOR:
                    playerCharacter.state = SelectState.PLAYER_CHARACTER;
                    eventSystem.SetSelectedGameObject(playerCharacter.lastBtnSelected.gameObject);
                    AudioManagerBrackeys.instance.Play("Back");
                    break;
            }
        }
    }

    public void Pause(InputAction.CallbackContext context)
    {

    }
    #endregion

    public void DisableInputs()
    {
        playerConfiguration.Input.onActionTriggered -= Input_onActionTriggered;
        playerInputActions.SelectCharacter.Disable();
    }

    private void OnDestroy()
    {
        playerInputActions.SelectCharacter.Disable();
    }


}