using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    [SerializeField] int levelNum;
    private Button button;

    // Start is called before the first frame update
    void Start()
    {
       button = GetComponent<Button>();
       button.onClick.AddListener(EnterLevel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void EnterLevel()
    {
        // save level num
        SceneManager.LoadScene("controls");
    }
}
