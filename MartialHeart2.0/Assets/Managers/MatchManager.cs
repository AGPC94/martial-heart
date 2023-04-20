using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.EventSystems;

public class MatchManager : MonoBehaviour
{
    public static MatchManager instance;

    //Strings
    string ready = "Kamae!";
    string start = "Hajime!";
    string stop = "Yame!";
    string ippon = "Ippon!";
    string restart1 = "Nihonme!";
    string restart2 = "Shobu!";
    string timeUp = "Shobu-Ari!";
    string draw = "Hikiwake!";
    string p1Wins = "Player 1 wins!";
    string p2Wins = "Player 2 wins!";

    [Header("Players Stats")]
    [SerializeField] PlayerStats ps1;
    [SerializeField] PlayerStats ps2;

    [Header("General Stats")]
    [SerializeField] float maxHeath;

    [Header("UI")]
    [SerializeField] Text txtInfo;
    [SerializeField] GameObject goPause;
    [SerializeField] Button btnContinue;

    [SerializeField] bool isPaused;

    [SerializeField] Player player1;
    [SerializeField] Player player2;

    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject cpuPrefab;

    [SerializeField] Vector3 player1Pos;
    [SerializeField] Vector3 player2Pos;

    List<PlayerConfiguration> playerConfigs;

    PlayerInputController[] players;
    AI aiPlayer;

    [SerializeField] SpriteRenderer background;

    [Serializable] 
    class PlayerStats
    {
        public float health;
        public float roundsWins;
        public Image[] imgHealth;
        public Image imgDeath;
        public PlayerConfiguration configuration;
        public float Nplayer;
    }

    void Awake()
    {
        instance = this;

        maxHeath = GameManager.instance.maxHealth;

        ps1.health = maxHeath;
        ps2.health = maxHeath;

        player1 = Instantiate(playerPrefab, player1Pos, Quaternion.identity).GetComponent<PlayerInputController>();

        switch (GameManager.instance.gameMode)
        {
            case GameMode.VSCPU:
                player2 = Instantiate(cpuPrefab, player2Pos, Quaternion.identity).GetComponent<AI>();
                break;

            case GameMode.VSPLAYER:
                player2 = Instantiate(playerPrefab, player2Pos, Quaternion.identity).GetComponent<PlayerInputController>();
                break;
        }

        CinemachineTargetGroup cineMachineTargetGroup = GameObject.Find("TargetGroup").GetComponent<CinemachineTargetGroup>();
        cineMachineTargetGroup.AddMember(player1.transform, 1, 0);
        cineMachineTargetGroup.AddMember(player2.transform, 1, 0);

        playerConfigs = SelectorCharacterSpawnManager.instance.GetPlayerConfigs();

        foreach (PlayerConfiguration config in playerConfigs)
        {
            if (config.PlayerIndex == 0)
            {
                player1.configuration = config;
                player1.GetComponent<Player>().NPlayer = config.PlayerIndex + 1;
            }
            if (config.PlayerIndex == 1)
            {
                player2.configuration = config;
                player2.NPlayer = config.PlayerIndex + 1;

                Vector3 reverseScale = player2.transform.localScale;
                reverseScale.x = -1;
                player2.transform.localScale = reverseScale;
            }
        }

        player1.name = "Player1";
        player2.name = "Player2";
    }

    // Start is called before the first frame update
    void Start()
    {
        background.sprite = GameManager.instance.stage.background;
        AudioManagerBrackeys.instance.PlayMusic(GameManager.instance.stage.music);
        StartCoroutine(StartMatch());
    }

    // Update is called once per frame
    void Update()
    {
        DisplayHealth(ps1);
        DisplayHealth(ps2);

        DisplayDeath();
    }

    void DisplayHealth(PlayerStats ps)
    {
        for (int i = 0; i < ps.imgHealth.Length; i++)
            if (i < ps.health)
                ps.imgHealth[i].gameObject.SetActive(true);
            else
                ps.imgHealth[i].gameObject.SetActive(false);

        ps.health = Mathf.Clamp(ps.health, 0, maxHeath);
    }

    void DisplayDeath()
    {
        //1
        if (player1.IsWalled)
            ps1.imgDeath.gameObject.SetActive(true);
        else
            ps1.imgDeath.gameObject.SetActive(false);

        //2
        if (player2.IsWalled)
            ps2.imgDeath.gameObject.SetActive(true);
        else
            ps2.imgDeath.gameObject.SetActive(false);

        //Finish
        if (ps1.health <= 0 || ps2.health <= 0)
        {
            ps1.imgDeath.gameObject.SetActive(false);
            ps2.imgDeath.gameObject.SetActive(false);
        }
    }

    public void TakeDamage(float damage, float nPlayer)
    {
        switch (nPlayer)
        {
            case 1:
                ps1.health -= damage;
                break;

            case 2:
                ps2.health -= damage;
                break;
        }
    }

    public void HitStop(float stopTIme, float slowMotionTime)
    {
        StartCoroutine(HitStopCo(stopTIme, slowMotionTime));
    }


    #region Coroutines
    public void ClashStop()
    {
        StartCoroutine(ClashStopCo());
    }

    IEnumerator ClashStopCo()
    {
        Debug.Log("ClashStopCo");
        SwitchActionMap("PauseMenu");
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.25f);
        SwitchActionMap("Player");
        Time.timeScale = 1;
    }

    IEnumerator HitStopCo(float stopTIme, float slowMotionTime)
    {
        SwitchActionMap("PauseMenu");
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(stopTIme);
        Time.timeScale = 0.5f;
        yield return new WaitForSecondsRealtime(slowMotionTime);
        SwitchActionMap("Player");
        Time.timeScale = 1;

        if (ps1.health <= 0 || ps2.health <= 0)
        {
            StartCoroutine(Finish());
        }

    }

    IEnumerator StartMatch()
    {
        goPause.SetActive(false);
        SwitchActionMap("PauseMenu");

        player1.Intro();
        player2.Intro();
        yield return new WaitForSecondsRealtime(1.5f);

        txtInfo.text = ready;
        txtInfo.GetComponent<Animator>().SetTrigger("ZoomIn");
        yield return new WaitForSecondsRealtime(1);

        txtInfo.text = start;
        txtInfo.GetComponent<Animator>().SetTrigger("ZoomIn");
        yield return new WaitForSecondsRealtime(1);

        txtInfo.text = string.Empty;
        SwitchActionMap("Player");
        foreach (var item in FindObjectsOfType<Player>())
            item.ChangeAction(Player.Action.MOVE);
    }

    IEnumerator Finish()
    {
        goPause.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        AudioManagerBrackeys.instance.StopMusic();

        SwitchActionMap("PauseMenu");

        foreach (var item in FindObjectsOfType<AI>())
        {
            item.StopAllCoroutines();
            item.ActionAllowed = false;
        }

        player1.Sheathe();
        player2.Sheathe();

        yield return new WaitForSecondsRealtime(3f);

        if (ps1.health <= 0)
        {
            txtInfo.text = "Player two wins!";
            txtInfo.GetComponent<Animator>().SetTrigger("ZoomIn");
            player2.Victory();

            player1.Die();
        }
        else if (ps2.health <= 0)
        {
            txtInfo.text = "Player one wins!";
            txtInfo.GetComponent<Animator>().SetTrigger("ZoomIn");
            player1.Victory();

            player2.Die();
        }
        
        if (ps1.health <= 0 && ps2.health <= 0)
        {

            txtInfo.text = "Draw!";
            txtInfo.GetComponent<Animator>().SetTrigger("ZoomIn");
            player1.Die();
            player2.Die();
        }

        yield return new WaitForSecondsRealtime(2);

        BackToCharacterSelect();
    }
    #endregion

    public void SwitchActionMap(string actionMap)
    {
        foreach (var item in SelectorCharacterSpawnManager.instance.GetPlayerConfigs())
            if (item.Input != null)
                item.Input.SwitchCurrentActionMap(actionMap);
    }

    #region Pause
    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        SwitchActionMap("PauseMenu");
        txtInfo.text = stop;
        txtInfo.GetComponent<Animator>().SetTrigger("ZoomIn");
        Time.timeScale = 0;
        isPaused = true;
        goPause.SetActive(true);
        ForceSelectGameObject(btnContinue.gameObject);
        AudioManagerBrackeys.instance.AdjustVolume(AudioManagerBrackeys.instance.currentMusic, 0.2f);
    }

    void ForceSelectGameObject(GameObject gameObject)
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
            EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void Resume()
    {
        StartCoroutine(ResumeCo());
    }

    IEnumerator ResumeCo()
    {
        //DisableControls();
        goPause.SetActive(false);

        txtInfo.text = ready;
        txtInfo.GetComponent<Animator>().SetTrigger("ZoomIn");
        yield return new WaitForSecondsRealtime(0.5f);
        txtInfo.text = start;
        txtInfo.GetComponent<Animator>().SetTrigger("ZoomIn");
        yield return new WaitForSecondsRealtime(0.5f);

        SwitchActionMap("Player");
        txtInfo.text = string.Empty;
        Time.timeScale = 1;
        isPaused = false;
        AudioManagerBrackeys.instance.AdjustVolume(AudioManagerBrackeys.instance.currentMusic, 1f);
    }

    #endregion

    public void BackToCharacterSelect()
    {
        Debug.Log("BackToCharacterSelect");

        foreach (var item in FindObjectsOfType<PlayerInputController>())
            item.RemoveInputs();

        Destroy(SelectorCharacterSpawnManager.instance.gameObject);

        Time.timeScale = 1;

        AudioManagerBrackeys.instance.AdjustVolume(AudioManagerBrackeys.instance.currentMusic, 1f);

        LevelLoader.instance.LoadLevel("CharacterSelect");

    }


}