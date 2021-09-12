using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] bool treeDefense;
    [SerializeField] WaveSpawner[] waveSpawners;

    [SerializeField] GameObject gameOverUI;
    [SerializeField] GameObject victoryUI;

    public bool isGameOver = false;

    private PlayerController player;
    private Tree_behavior tree;

    [HideInInspector] public bool victory = false;
    [HideInInspector] public bool goingToNextLevel = false;

    private void Start()
    {
        player = GetComponentInChildren<PlayerController>();
        tree = GetComponentInChildren<Tree_behavior>();
    }


    // Update is called once per frame
    void Update()
    {
        // Do nothing if victory is achieved
        if (victory || goingToNextLevel)
            return;

        // Check if Player is dead, or if in treeDefense, tree is Dead
        if (player.isDead || (treeDefense && tree.isDead))
        {
            // If so, Game Over
            Invoke("GameOver", 2);
        } // If not, check if the waves are over
        else if (treeDefense && WavesCompleted())
        {
            // If so, Victory
            Victory();
        }
    }

    // Returns true if all waves are completed, false otherwise
    bool WavesCompleted()
    {
        // Iterate through array of waveSpawners
        for (int i = 0; i < waveSpawners.Length; i++)
        {
            // Check if ith waveSpawner is not done spawning
            if (!waveSpawners[i].spawnerFinished)
            {
                // If not done spawning, return false
                return false;
            }
        }

        // If arriving here, all waves are completed
        return true;
    }

    // Process a Victory event
    void Victory()
    {
        // Change the Tree to fully alive animation
        tree.anim.SetTrigger("grow");

        // Debug.Log("Victory!");
        victory = true;

        // Increment number of levels unlocked
        if (SceneManager.GetActiveScene().name == "Level1TreeDefense")
        {
            LevelData.levelsUnlocked[1] = true; 
        }
        else if (SceneManager.GetActiveScene().name == "Level2TreeDefense")
        {
            LevelData.levelsUnlocked[2] = true;
        }
        else if (SceneManager.GetActiveScene().name == "Level3TreeDefense")
        {
            LevelData.levelsUnlocked[3] = true;
        }
        else if (SceneManager.GetActiveScene().name == "Level4TreeDefense")
        {
            LevelData.levelsUnlocked[4] = true;
        }

        Invoke("VictoryUI", 3.5f);
    }

    // Victory UI Popup
    void VictoryUI()
    {
        // Pause Game
        Time.timeScale = 0f;
        PauseMenu.isPaused = true;

        // Popup
        victoryUI.SetActive(true);
    }

    // Process a GameOver event
    void GameOver()
    {
        Debug.Log("Game Over!");
        isGameOver = true;

        // Pause Game
        Time.timeScale = 0f;
        PauseMenu.isPaused = true;

        // Game Over UI Popup
        gameOverUI.SetActive(true);

    }

    // Respawn player to last scene, restoring game state
    public void Restart()
    {
        Time.timeScale = 1.0f;
        PauseMenu.isPaused = false;
        SceneManager.LoadScene(sceneName);
        Debug.Log(sceneName);
    }

    // Change scene to Title Screen
    public void Menu()
    {
        Time.timeScale = 1.0f;
        PauseMenu.isPaused = false;
        SceneManager.LoadScene("TitleScreen");
    }

    // Close the application
    public void Quit()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
