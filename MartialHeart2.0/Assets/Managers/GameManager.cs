using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode { VSCPU, VSPLAYER }
public class GameManager : MonoBehaviour
{
    public GameMode gameMode;
    public float maxHealth;
    public float cpuDifficulty;
    public Stage stage;


    public static GameManager instance;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        Application.targetFrameRate = 60;
    }
    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 5;
        cpuDifficulty = 2;
    }


    public void SetStage(Stage stg)
    {
        stage = stg;
    }
}
