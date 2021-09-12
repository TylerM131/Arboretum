using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class MenuButtons : MonoBehaviour
{
    [SerializeField] GameObject[] buttonText;
    private int defaultFontSize;
    [SerializeField] string nextLevel;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject controlMenu;
    private GameManager gameManager;
    
    // Start is called before the first frame update
    void Awake()
    {
        GameObject gm = GameObject.Find("GameManager");
        if (gm == null)
            gameManager = null;
        else
            gameManager = gm.GetComponent<GameManager>();

        if (buttonText.Length > 0)
            defaultFontSize = buttonText[0].GetComponent<Text>().fontSize;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectButton(int n)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttonText[n].transform.parent.gameObject);
    }

    public void GrowButtonText(int n)
    {
        buttonText[n].GetComponent<Text>().fontSize = defaultFontSize + 5;
        buttonText[n].GetComponent<Text>().color = Color.yellow;
    }

    public void ShrinkButtonText(int n)
    {
        buttonText[n].GetComponent<Text>().fontSize = defaultFontSize;
        buttonText[n].GetComponent<Text>().color = Color.white;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("LevelSelection");
    }

    public void Settings()
    {
        Debug.Log("Settings");
    }

    public void Quit()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    public void Restart()
    {
        Time.timeScale = 1.0f;
        PauseMenu.isPaused = false;
        gameManager.isGameOver = false;
        gameManager.victory = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Menu()
    {
        Time.timeScale = 1.0f;
        PauseMenu.isPaused = false;
        gameManager.isGameOver = false;
        gameManager.victory = false;
        SceneManager.LoadScene("TitleScreen");
    }

    public void NextLevel()
    {
        // Debug.Log("Going to next level");
        GameObject.Find("GameManager").GetComponent<GameManager>().goingToNextLevel = true;
        Time.timeScale = 1.0f;
        PauseMenu.isPaused = false;
        gameManager.isGameOver = false;
        gameManager.victory = false;
        SceneManager.LoadScene(nextLevel);
    }

    public void Controls()
    {
        if (mainMenu != null && controlMenu != null)
        {
            mainMenu.SetActive(false);
            controlMenu.SetActive(true);
        }
    }

    public void OnPause()
    {
        if (SceneManager.GetActiveScene().name == "LevelSelection")
        {
            SceneManager.LoadScene("TitleScreen");
            return;
        }

        if (gameManager != null && gameManager.victory)
            return; 
        
        if (mainMenu != null && controlMenu != null && controlMenu.activeSelf)
        {
            controlMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
    }

    public void Help()
    {
        SceneManager.LoadScene("LoreExplanation");
    }

    public void OnCustomSubmit()
    {
        Debug.Log(controlMenu);
        if (controlMenu != null)
        {
            Debug.Log(controlMenu.activeSelf);
            if (controlMenu.activeSelf)
                return;
        }

        Debug.Log(mainMenu + " " + PauseMenu.isPaused);
        if (mainMenu != null || PauseMenu.isPaused)
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
    }
}
