using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerConfigurationManager : MonoBehaviour
{
    List<PlayerConfigurationTutorial> playerConfigs;

    [SerializeField]
    int MaxPlayers = 2;

    public static PlayerConfigurationManager instance { get; private set; }

    void Awake()
    {
        if (instance != null)
        {
            Debug.Log("SINGLETON - Trying to create another instance of singleton!!");
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance);
            playerConfigs = new List<PlayerConfigurationTutorial>();
        }
    }

    public List<PlayerConfigurationTutorial> GetPlayerConfigs()
    {
        return playerConfigs;
    }

    public void SetPlayerColor(int index, Material color)
    {
        playerConfigs[index].PlayerMaterial = color;
    }

    public void ReadyPlayer(int index)
    {
        playerConfigs[index].IsReady = true;

        if (playerConfigs.Count == MaxPlayers && playerConfigs.All(p => p.IsReady == true))
        {
            SceneManager.LoadScene("TutorialStage");
        }
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log("PlayerJoined " + pi.playerIndex);


        if (!playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            pi.transform.SetParent(transform);
            playerConfigs.Add(new PlayerConfigurationTutorial(pi));
        }
    }
}

public class PlayerConfigurationTutorial
{
    public PlayerConfigurationTutorial(PlayerInput pi)
    {
        PlayerIndex = pi.playerIndex;
        Input = pi;
    }
    public PlayerInput Input { get; set; }
    public int PlayerIndex { get; set; }
    public bool IsReady { get; set; }
    public Material PlayerMaterial { get; set; }

}