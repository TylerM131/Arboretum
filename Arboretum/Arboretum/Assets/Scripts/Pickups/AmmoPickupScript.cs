using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickupScript : MonoBehaviour
{
    [SerializeField] int ammo_increase = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
            if (pc == null)
                pc = collision.gameObject.transform.parent.gameObject.GetComponent<PlayerController>();

            if (pc.ammo < pc.maxammo)
            {
                if (pc.ammo + ammo_increase <= pc.maxammo)
                    pc.ammo += ammo_increase;
                else
                    pc.ammo = pc.maxammo;

                AudioSource.PlayClipAtPoint(pc.pickupSound, transform.position);
                Destroy(gameObject);
            }
        }
    }
}
