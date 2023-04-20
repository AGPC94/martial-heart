using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{
    PlayerConfigurationTutorial playerConfiguration;
    
    SpriteRenderer renderer;

    PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        renderer = GetComponent<SpriteRenderer>();
    }

    public void InitializePlayer(PlayerConfigurationTutorial pc)
    {
        playerConfiguration = pc;
        renderer.material = pc.PlayerMaterial;
        playerConfiguration.Input.onActionTriggered += Input_onActionTriggered;
    }

    private void Input_onActionTriggered(CallbackContext obj)
    {
        if (obj.action.name == inputActions.Player.Movement.name)
        {
            OnMove(obj);
        }
    }

    public void OnMove(CallbackContext context)
    {
        Debug.Log("Soy " + name);
    }
}
