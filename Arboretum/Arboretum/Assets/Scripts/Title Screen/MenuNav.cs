using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuNav : MonoBehaviour
{
    [SerializeReference] GameObject firstBtn;

    private void OnEnable()
    {
        StartCoroutine("SelectButton");
    }

    private IEnumerator SelectButton()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstBtn.gameObject);
    }
}
