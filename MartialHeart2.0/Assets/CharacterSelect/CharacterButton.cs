using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    public Character character;
    Image imgPlayer1;
    Image imgPlayer2;
    Image imgCpu;
    Image imgCharacterPortrait;

    void Awake()
    {
        imgPlayer1 = transform.Find("ImgPlayer1").GetComponent<Image>();
        imgPlayer2 = transform.Find("ImgPlayer2").GetComponent<Image>();
        imgCpu = transform.Find("ImgCpu").GetComponent<Image>();
        imgCharacterPortrait = transform.Find("ImgCharacterPortrait").GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        imgCharacterPortrait.sprite = character.portrait;

        NoneImg();
    }

    public void Select(SelectorCharacter sc, PlayerCharacter pc)
    {
        if (sc.nPlayer == 1 && pc.nPlayer == 1)
            imgPlayer1.gameObject.SetActive(true);

        if (sc.nPlayer == 2 && pc.nPlayer == 2)
            imgPlayer2.gameObject.SetActive(true);

        if (sc.nPlayer == 1 && pc.nPlayer == 2)
            imgCpu.gameObject.SetActive(true);
    }

    public void NoneImg()
    {
        imgPlayer1.gameObject.SetActive(false);
        imgPlayer2.gameObject.SetActive(false);
        imgCpu.gameObject.SetActive(false);
    }
}