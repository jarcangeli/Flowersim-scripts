using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject tutorialScreen = null;
    [SerializeField]
    GameObject firstHint = null;
    [SerializeField]
    GameObject flowerBoardHelp = null;
    AudioManager audioManager;

    bool firstClick = true;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        tutorialScreen.SetActive(true);
        flowerBoardHelp.SetActive(false);
        firstHint.SetActive(false);
        audioManager.PlayTheme("ThemeEP");
    }
    public void ToggleTutorial()
    {
        if (firstClick) 
        { 
            audioManager.PlayTheme("ThemeDrums", 4);
            firstClick = false;
            firstHint.SetActive(true);
        }
        tutorialScreen.SetActive(!tutorialScreen.activeInHierarchy);
    }

    public void ToggleFlowerBoardHelp()
    {
        flowerBoardHelp.SetActive(!flowerBoardHelp.activeInHierarchy);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
