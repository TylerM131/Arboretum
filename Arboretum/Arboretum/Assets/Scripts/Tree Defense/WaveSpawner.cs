using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{   
    // States of the WaveSpawner
    public enum SpawnState {Spawning, Waiting, Counting, Finished};

    // Wave object definition
    [System.Serializable]
    public class Wave
    {
        // Name of the wave, prefab spawned by wave, number of enemies in a wave, spawn rate
        public string name;
        public GameObject enemy;
        public int enemyCountMin;
        public int enemyCountMax;
        public float rate;
    }

    // Iterating through waves
    public Wave[] waves;
    private int nextWave = 0;
    public Transform[] spawnPoints;

    // Timing of waves
    public float timeBetweenWaves = 5f;
    public float waveCountdown = 5f;

    // Delay between searching for living enemies
    private float searchDelay = 1f;

    // Set the state to Counting, completed to false
    private SpawnState state = SpawnState.Counting;
    public bool spawnerFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points referenced.");
        }

        waveCountdown = timeBetweenWaves;
    }

    // Update is called once per frame
    void Update()
    {
        // If finished,
        if (state == SpawnState.Finished)
        {
            return;
        }

        // If waiting,
        if (state == SpawnState.Waiting)
        {
            // Check if there are living enemies
            if (EnemyIsAlive())
            {
                return;
            } else
            {
                // Begin another round of wave
                WaveCompleted();
            }
        }

        // If spawner has reached end of the countdown,
        if (waveCountdown <= 0)
        {
            // And it isn't currently spawning,
            if (state != SpawnState.Spawning)
            {
                // Start spawning the wave
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
        } // If currently spawning, 
        else
        {
            // Countdown based on change in time
            waveCountdown -= Time.deltaTime;
        }
    }

    // Return true if there are living enemies, false otherwise
    bool EnemyIsAlive()
    {
        // Decrement the delay
        searchDelay -= Time.deltaTime;

        // If the delay time has passed,
        if (searchDelay <= 0f)
        {
            // Set the delay back to 1
            searchDelay = 1f;
            // And if no enemies are found,
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }

        // Otherwise, return true
        return true;
    }

    void WaveCompleted()
    {
        // Start counting down to the next wave and increment
        state = SpawnState.Counting;
        waveCountdown = timeBetweenWaves;

        if (nextWave + 1 > waves.Length - 1)
        {
            // This spawner is done spawning: UI Popup "Wave Completed"
            Debug.Log("Wave Completed");
            state = SpawnState.Finished;
            spawnerFinished = true;
        } else
        {
            nextWave++;
        }
        
    }

    IEnumerator SpawnWave (Wave _wave)
    {
        Debug.Log("Spawning Wave: " + _wave.name);
        state = SpawnState.Spawning;

        // Select a random number of enemies between min and max
        int enemyCount = Random.Range(_wave.enemyCountMin, _wave.enemyCountMax);

        // Iterate through number of enemies
        for (int i = 0; i < enemyCount; i++)
        {
            // Spawn enemies one by one, at a given rate
            SpawnEnemy(_wave.enemy);
            yield return new WaitForSeconds(1f / _wave.rate);
        }

        // Wait for player to kill the enemies
        state = SpawnState.Waiting;
        
        yield break;
    }

    void SpawnEnemy (GameObject _enemy)
    {
        // Spawn enemy
        Debug.Log("Spawning Enemy: " + _enemy.name);

        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points referenced.");
        }

        Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(_enemy, _sp.position, _sp.rotation);
    }
}
