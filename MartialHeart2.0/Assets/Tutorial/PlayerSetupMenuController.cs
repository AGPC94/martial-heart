using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetupMenuController : MonoBehaviour
{
    int PlayerIndex;

    [SerializeField] Text txtTitle;
    [SerializeField] GameObject readyPanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] Button BtnReady;

    float ignoreInputTime = 1.5f;
    bool inputEnabled;

    // Update is called once per frame
    void Update()
    {
        if (Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }
    }

    public void SetPlayerIndex(int pi)
    {
        PlayerIndex = pi;
        txtTitle.text = "Player" + pi + 1.ToString();
        ignoreInputTime = Time.time + ignoreInputTime;
    }

    public void SetColor(Material color)
    {
        if (!inputEnabled)
            return;

        PlayerConfigurationManager.instance.SetPlayerColor(PlayerIndex, color);
        readyPanel.SetActive(true);
        BtnReady.Select();
        menuPanel.SetActive(false);
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled)
            return;

        PlayerConfigurationManager.instance.ReadyPlayer(PlayerIndex);
        BtnReady.gameObject.SetActive(false);
    }
}
