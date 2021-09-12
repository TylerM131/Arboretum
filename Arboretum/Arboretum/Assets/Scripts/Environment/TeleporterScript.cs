using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterScript : MonoBehaviour
{
    public bool painful = true;
    // [SerializeField] float painfulDelay = 0.4f;
    [SerializeField] float painlessDelay = 0.1f;


    [SerializeReference] GameObject waypoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
        if (painful)
        {
            if (collision.gameObject.CompareTag("Player") && pc != null && !pc.isDead)
            {
                pc.PlayFallSound();
                pc.health--;
                if (pc.health > 0)
                {
                    pc.gameObject.transform.position = waypoint.transform.position;
                    pc.StartCoroutine(pc.InvincibiltyRoutine());
                }
                else
                {
                    pc.isDead = true;
                    pc.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                    HealthSystem.Instance.SetHealth(pc.health);
                    HealthSystem.Instance.SetMana(pc.ammo);
                }
            }
            else if (collision.gameObject.name == "ShortHitBox")
            {
                pc = collision.gameObject.transform.parent.gameObject.GetComponent<PlayerController>();
                if (pc.isDead)
                    return;
                pc.PlayFallSound();
                pc.health--;
                if (pc.health > 0)
                {
                    pc.gameObject.transform.position = waypoint.transform.position;
                    pc.StartCoroutine(pc.InvincibiltyRoutine());
                }
                else
                {
                    pc.isDead = true;
                    pc.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                    HealthSystem.Instance.SetHealth(pc.health);
                    HealthSystem.Instance.SetMana(pc.ammo);
                }
            }
            else if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Heart") || collision.gameObject.CompareTag("Crystal"))
            {
                if (collision.gameObject.GetComponent<SpriteRenderer>() != null)
                    Destroy(collision.gameObject);
                else
                    Destroy(collision.gameObject.transform.parent.gameObject);
            }

        }
        else if (collision.gameObject.CompareTag("Player") && pc != null)
        {
            pc.PlayTeleportSound();
            StartCoroutine(Move(collision.gameObject, painlessDelay));
        }
    }

    private IEnumerator Move(GameObject thing, float wait)
    {
        yield return new WaitForSeconds(wait);
        thing.transform.position = waypoint.transform.position;
    }
}
