using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menus")]
    GameObject openMenu;
    [SerializeField] GameObject mainMenu;

    [SerializeField] Slide[] slides;
    [SerializeField] Text titleSlide;
    [SerializeField] Image imgSlide;
    [SerializeField] int slideIndex;


    // Start is called before the first frame update
    void Start()
    {
        openMenu = mainMenu;
        AudioManagerBrackeys.instance.PlayMusic("MainMenu");
    }

    void Update()
    {
        ShowSlide();
    }

    public void GoToCharacterSelect()
    {
        LevelLoader.instance.LoadLevel("CharacterSelect");
    }

    public void Quit()
    {
        Application.Quit();
    }
    public void OpenMenu(GameObject menu)
    {
        openMenu.SetActive(false);
        menu.SetActive(true);
        openMenu = menu;

        ForceSelectGameObject(menu.GetComponentInChildren<Button>().gameObject);
    }
    void ForceSelectGameObject(GameObject gameObject)
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
            EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void ShowSlide()
    {
        titleSlide.text = slides[slideIndex].title;
        if (slides[slideIndex].sprite != null)
            imgSlide.sprite = slides[slideIndex].sprite;
    }

    public void NextSlide()
    {
        slideIndex = (slideIndex + 1) % slides.Length;
    }

    public void PreviousSlide()
    {
        slideIndex--;

        if (slideIndex < 0)
            slideIndex += slides.Length;

    }
}
