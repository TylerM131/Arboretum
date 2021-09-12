using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelSelectScreen : MonoBehaviour
{
    [SerializeField] Button[] buttons;

    [SerializeField] string[] scenes; 

    // Start is called before the first frame update
    void Start()
    {
        // Initialize all buttons
        for (int i = 0; i < LevelData.totalLevels; i++)
        {
            if (LevelData.levelsUnlocked[i])
                buttons[i].gameObject.SetActive(true);
        }
    }

    public void EnterLevel(int i)
    {
        SceneManager.LoadScene(scenes[i]);
    }

    public void SelectButton(int i)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttons[i].gameObject);
    }
}
