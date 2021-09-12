using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBlockAgain : MonoBehaviour
{
    private PlayerController pc;

    // Start is called before the first frame update
    void Start()
    {
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    public void CanBlock()
    {
        pc.can_block = true;
    }
}
