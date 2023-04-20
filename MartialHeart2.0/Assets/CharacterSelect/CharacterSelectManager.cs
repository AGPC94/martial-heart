using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("UI")]

    [SerializeField] GameObject goDifficulty;

    [SerializeField] Slider sldMaxHealth;
    [SerializeField] Slider sldDifficulty;

    [SerializeField] Text txtSelect;


    public static CharacterSelectManager instance;

    void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {/*
        switch (ModeManager.instance.gameMode)
        {
            case ModeManager.GameMode.ARCADE:
                singlePlayerEventSystem.SetActive(true);
                player2Layout.SetActive(false);
                break;

            case ModeManager.GameMode.VSCPU:
                singlePlayerEventSystem.SetActive(true);
                player2Layout.SetActive(false);
                break;

            case ModeManager.GameMode.VSPLAYER:
                multiPlayer1EventSystem.SetActive(true);
                multiPlayer2EventSystem.SetActive(true);
                sldDifficulty.SetActive(false);
                break;
        }
        */

        AudioManagerBrackeys.instance.PlayMusic("MainMenu");
    }

    // Update is called once per frame
    void Update()
    {
        SetGameMode();

        SetSliders();

        if (ArePlayersReady())
            txtSelect.text = "Ready to fight!";
        else
            txtSelect.text = "Select your Warrior!";
    }

    public void SetGameMode()
    {
        if (SelectorCharacterSpawnManager.instance.inputManager.playerCount == 2)
        {
            GameManager.instance.gameMode = GameMode.VSPLAYER;
            goDifficulty.gameObject.SetActive(false);
        }
        else
        {
            GameManager.instance.gameMode = GameMode.VSCPU;
            goDifficulty.gameObject.SetActive(true);
        }
    }

    public void SetSliders()
    {
        sldMaxHealth.value = Mathf.Clamp(sldMaxHealth.value, 1, 10);
        GameManager.instance.maxHealth = sldMaxHealth.value;

        sldDifficulty.value = Mathf.Clamp(sldDifficulty.value, 1, 3);
        GameManager.instance.cpuDifficulty = sldDifficulty.value;
    }

    public bool ArePlayersReady()
    {
        foreach (PlayerCharacter pc in FindObjectsOfType<PlayerCharacter>())
            if (pc.state != SelectState.PLAYER_COLOR)
                return false;
        return true;
    }

    public void BackToMainMenu()
    {
        Destroy(GameObject.Find("PlayerInputManager"));
        LevelLoader.instance.LoadLevel("MainMenu");
    }


    public void SetMaxHealth(float value)
    {
        //GameManager.instance.maxHealth = value;
    }

    public void SeCPUDifficulty(float value)
    {
        //GameManager.instance.cpuDifficulty = value;
    }
}
