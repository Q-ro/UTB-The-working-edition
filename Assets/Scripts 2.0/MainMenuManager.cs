using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    #region inspector Variables

    [SerializeField] Button playGameButton;
    [SerializeField] Button creditsButton;
    [SerializeField] Button exitButton;

    #endregion

    // Use this for initialization
    void Start()
    {
        playGameButton.onClick.AddListener(LoadGame);
        creditsButton.onClick.AddListener(LoadCredits);
        exitButton.onClick.AddListener(ExitGame);
    }

    void LoadGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("New Main");
    }
    void LoadCredits()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Video");
    }
    void ExitGame()
    {
        Application.Quit();
    }
}
