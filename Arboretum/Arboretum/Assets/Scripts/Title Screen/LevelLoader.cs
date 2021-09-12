using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] GameObject[] backgroundImages;
    private GameObject currentBackground;
    private int currentBackgroundNum;
    [SerializeField] GameObject spawnLoc;
    private bool reverting;
    public int levelsUnlocked;

    // Start is called before the first frame update
    void Start()
    {
        currentBackground = Instantiate(backgroundImages[0], spawnLoc.transform);
        currentBackgroundNum = 0;
        reverting = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Hovering(int levelNum)
    {
        if (levelNum <= levelsUnlocked)
        {
            reverting = false;
            if (currentBackgroundNum != levelNum)
            {
                Destroy(currentBackground);
                currentBackground = Instantiate(backgroundImages[levelNum], spawnLoc.transform);
                currentBackgroundNum = levelNum;
            }
        }
    }

    public void RevertBackground(int levelNum)
    {
        if (levelNum <= levelsUnlocked)
        {
            StartCoroutine(RevertBackgroundHelper());
            reverting = true;
        }
    }

    IEnumerator RevertBackgroundHelper()
    {
        yield return new WaitForSeconds(0);

        if (reverting == true && currentBackgroundNum != 0)
        {
            Destroy(currentBackground);
            currentBackground = Instantiate(backgroundImages[0], spawnLoc.transform);
            currentBackgroundNum = 0;
        }
    }
}
