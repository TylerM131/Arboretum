using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LoadTitleScreen : MonoBehaviour
{
    public void OnPause()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void OnSubmit()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void CustomSubmit()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
