using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Continue : MonoBehaviour
{
    private bool fadeOut = true;
    private float fadeSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (fadeOut)
        {
            Color objectColor = GetComponent<SpriteRenderer>().material.color;
            float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            GetComponent<SpriteRenderer>().material.color = objectColor;

            if (objectColor.a <= 0)
            {
                fadeOut = false;
            }
        }
        else
        {
            Color objectColor = GetComponent<SpriteRenderer>().material.color;
            float fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            GetComponent<SpriteRenderer>().material.color = objectColor;

            if (objectColor.a >= 1)
            {
                fadeOut = true;
            }
        }
    }
    public void OnSubmit()
    {
        // use saved level num to determine which scene to load
        SceneManager.LoadScene("Level1Adventure");
    }
}
