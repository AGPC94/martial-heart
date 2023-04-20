using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SelectState { PLAYER_CHARACTER, PLAYER_COLOR };

public class PlayerCharacter : MonoBehaviour
{
    public Button sldColor;
    public Text txtCharacterName;
    public Button lastBtnSelected;
    public Image imgCharacter;
    public Text txtJoin;

    public Character character;
    public SelectState state;

    public int nPlayer;
    public int skinIndex;

    public Text txtColor;

    // Start is called before the first frame update
    void Start()
    {
        state = SelectState.PLAYER_CHARACTER;
    }

    // Update is called once per frame
    void Update()
    {
        if (character != null)
        {
            //sldColor.maxValue = character.skins.Length - 1;

            txtCharacterName.text = character.name;

            //imgCharacter.gameObject.SetActive(true);
            imgCharacter.sprite = character.artwork;

            Skin theSkin = character.skins[skinIndex];

            imgCharacter.material.SetTexture("_MainText", theSkin.mainTexture);

            imgCharacter.material.SetColor("_Color1", theSkin.color1);
            imgCharacter.material.SetColor("_NewColor1", theSkin.newColor1);

            imgCharacter.material.SetColor("_Color2", theSkin.color2);
            imgCharacter.material.SetColor("_NewColor2", theSkin.newColor2);

            imgCharacter.material.SetColor("_Color3", theSkin.color3);
            imgCharacter.material.SetColor("_NewColor3", theSkin.newColor3);

            txtColor.text = "Color " + (skinIndex + 1).ToString();
        }
        else
        {
            //imgCharacter.gameObject.SetActive(false);
        }

        if (nPlayer <= SelectorCharacterSpawnManager.instance.inputManager.playerCount)
        {
            txtJoin.gameObject.SetActive(false);
            /*
            switch (state)
            {
                case SelectState.PLAYER_CHARACTER:
                    txtJoin.text = "Select warrior!";
                    break;
                case SelectState.PLAYER_COLOR:
                    txtJoin.text = "Ready!";
                    break;
            }
            */
        }
        else
        {
            //txtJoin.text = "Press a button!";
            txtJoin.gameObject.SetActive(true);
        }

    }

    public void ChangeSkin(float index)
    {
        skinIndex = (int)index;
    }

    public void ResetSkinIndex()
    {
        PlayerCharacter[] pcs = FindObjectsOfType<PlayerCharacter>();

        foreach (var item in pcs)
        {
            if (item.skinIndex == skinIndex && item.nPlayer != nPlayer && character == item.character)
            {
                skinIndex++;
                if (skinIndex > character.skins.Length)
                {
                    skinIndex = 0;
                }
            }
        }
    }

    public void NextSkin()
    {
        skinIndex = (skinIndex + 1) % character.skins.Length;
        PlayerCharacter otherPC = null;

        foreach (var item in FindObjectsOfType<PlayerCharacter>())
            if (item.nPlayer != nPlayer)
                otherPC = item;

        if (SelectorCharacterSpawnManager.instance.GetPlayerConfigs().Count != 1)
            while (character == otherPC.character && skinIndex == otherPC.skinIndex)
                NextSkin();

        SelectorCharacterSpawnManager.instance.SetPlayerColor(nPlayer - 1, skinIndex);
    }
    public void PreviousSkin()
    {
        skinIndex--;
        PlayerCharacter otherPC = null;

        foreach (var item in FindObjectsOfType<PlayerCharacter>())
            if (item.nPlayer != nPlayer)
                otherPC = item;

        if (SelectorCharacterSpawnManager.instance.GetPlayerConfigs().Count != 1)
            while (character == otherPC.character && skinIndex == otherPC.skinIndex)
                PreviousSkin();

        if (skinIndex < 0)
            skinIndex += character.skins.Length;

        SelectorCharacterSpawnManager.instance.SetPlayerColor(nPlayer - 1, skinIndex);
    }

    public void AvoidRepeatSkin()
    {
        if (SelectorCharacterSpawnManager.instance.GetPlayerConfigs().Count == 1)
            return;
        
        PlayerCharacter otherPC = null;

        foreach (var item in FindObjectsOfType<PlayerCharacter>())
            if (item.nPlayer != nPlayer)
                otherPC = item;

        while (character == otherPC.character && skinIndex == otherPC.skinIndex)
            NextSkin();

    }
}
