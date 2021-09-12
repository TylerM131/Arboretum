using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    public GameObject controlsUI;
    private GameManager gameManager;

    private void Start()
    {
        GameObject gm = GameObject.Find("GameManager");
        gameManager = gm.GetComponent<GameManager>();
    }

    public void OnPause()
    {
        if (gameManager.isGameOver || gameManager.victory)
            return;

        if (isPaused)
        {
            EventSystem.current.SetSelectedGameObject(null);
            controlsUI.SetActive(false);
            Resume();
        }
        else
        {
            Debug.Log("Pausing");
            Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Restart()
    {
        Time.timeScale = 1.0f;
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Controls()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Debug.Log("Controls");
        this.gameObject.SetActive(false);
        controlsUI.SetActive(true);
    }

    public void Menu()
    {
        Time.timeScale = 1.0f;
        isPaused = false;
        SceneManager.LoadScene("TitleScreen");
    }

    public void Quit()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
