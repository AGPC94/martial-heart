using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class SelectorCharacterSpawnManager : MonoBehaviour
{
    [HideInInspector]
    public PlayerInputManager inputManager;

    [SerializeField] float maxPlayers;
    
    List<PlayerConfiguration> playerConfigs;

    public static SelectorCharacterSpawnManager instance;

    void Awake()
    {
        inputManager = GetComponent<PlayerInputManager>();
        playerConfigs = new List<PlayerConfiguration>();

        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

    }
    public List<PlayerConfiguration> GetPlayerConfigs()
    {
        return playerConfigs;
    }

    public void SetPlayerCharacter(int index, Character character)
    {
        playerConfigs[index].character = character;
    }
    public void SetPlayerColor(int playerIndex, int skinIndex)
    {
        playerConfigs[playerIndex].skinIndex = skinIndex;
    }

    void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("PlayerJoined " + playerInput.playerIndex);

        playerInput.transform.SetParent(transform);

        if (playerConfigs.Count >= 2)
        {
            playerConfigs.RemoveAt(playerConfigs.Count - 1);
        }

        playerConfigs.Add(new PlayerConfiguration(playerInput));

        if (inputManager.playerCount == inputManager.maxPlayerCount)
        {
            SelectorCharacter[] selectors = FindObjectsOfType<SelectorCharacter>();
            foreach (SelectorCharacter selector in selectors)
                selector.CancelCPU();

            inputManager.DisableJoining();
        }

        AudioManagerBrackeys.instance.Play("Join");
    }

    void OnPlayerLeft(PlayerInput playerInput)
    {
        if (inputManager.playerCount != inputManager.maxPlayerCount)
        {
            inputManager.EnableJoining();

            foreach (PlayerConfiguration config in playerConfigs)
            {
                if (config.Input.Equals(playerInput))
                {
                    playerConfigs.Remove(config);
                    break;
                }
            }

            Debug.Log("PlayerLeft " + playerInput.playerIndex);
        }
    }
}

/*
SelectorCharacter sc = FindObjectOfType<SelectorCharacter>();
if (inputManager.playerCount == 1)
    sc.nPlayer = inputManager.playerCount;

if (inputManager.playerCount == 2)
{
    sc.nPlayer = inputManager.playerCount;

    SelectorCharacter[] selectors = FindObjectsOfType<SelectorCharacter>();
    foreach (SelectorCharacter selector in selectors)
        selector.CancelCPU();
}
*/
