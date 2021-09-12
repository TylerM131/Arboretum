using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickupScript : MonoBehaviour
{
    [SerializeField] int health_increase = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
            if(pc == null)
                pc = collision.gameObject.transform.parent.gameObject.GetComponent<PlayerController>();

            if (pc.health < pc.maxhealth)
            {
                if (pc.health + health_increase <= pc.maxhealth)
                    pc.health += health_increase;
                else
                    pc.health = pc.maxhealth;

                AudioSource.PlayClipAtPoint(pc.pickupSound, transform.position);
                Destroy(gameObject);
            }
        }
    }
}
