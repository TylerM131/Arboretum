using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfGameOver : MonoBehaviour
{
    private PlayerController pc;
    public GameObject gameOverUI;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        GameObject gm = GameObject.Find("GameManager");
        gameManager = gm.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pc.health <= 0)
        {
            Invoke("GameOver", 2);
        }
    }

    // Process a GameOver event
    void GameOver()
    {
        Debug.Log("Game Over!");
        gameManager.isGameOver = true;

        // Pause Game
        Time.timeScale = 0f;
        PauseMenu.isPaused = true;

        // Game Over UI Popup
        gameOverUI.SetActive(true);

    }
}
