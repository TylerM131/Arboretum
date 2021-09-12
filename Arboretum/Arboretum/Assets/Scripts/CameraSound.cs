using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSound : MonoBehaviour
{
    private AudioSource music;
    private float volume;
    [SerializeField] float pauseDivisor = 4.0f;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        music = GetComponent<AudioSource>();
        volume = music.volume;

        GameObject gm = GameObject.Find("GameManager");
        gameManager = gm.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.isPaused || gameManager.isGameOver || gameManager.victory)
            music.volume = volume / pauseDivisor;
        else
            music.volume = volume;
    }
}
