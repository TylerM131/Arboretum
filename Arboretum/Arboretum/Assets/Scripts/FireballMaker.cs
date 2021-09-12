using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballMaker : MonoBehaviour
{
    [SerializeField] GameObject fireball;
    [SerializeField] float timeBetweenFireballs = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("StartSpawning", Random.Range(0, 10.0f));
    }

    private void StartSpawning()
    {
        StartCoroutine("SpawnFireballs");
    }

    // Create new fireball every timeBetweenFireballs seconds
    private IEnumerator SpawnFireballs()
    {
        GameObject fb;

        while (true)
        {
            yield return new WaitForSeconds(timeBetweenFireballs);
            fb = Instantiate(fireball);
            fb.transform.position = new Vector3(transform.position.x, transform.position.y, 0.0f);
        }
    }
}
