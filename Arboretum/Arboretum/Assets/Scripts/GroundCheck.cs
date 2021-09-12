using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public bool grounded;

    private const int enemyLayer = 6;

    public void Start()
    {
        grounded = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == enemyLayer)
            return;

        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Hazard"))
            grounded = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == enemyLayer)
            return;

        if ((collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Hazard")) && !grounded)
            grounded = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == enemyLayer)
            return;

        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Hazard"))
            grounded = false;
    }
}
