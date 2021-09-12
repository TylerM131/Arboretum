using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog_Damaged : MonoBehaviour
{
    private Frog_behavior parentScript;

    private AudioSource audioPlayer;
    [SerializeReference] AudioClip damaged;

    // Start is called before the first frame update
    void Start()
    {
        parentScript = transform.parent.gameObject.GetComponent<Frog_behavior>();
        audioPlayer = gameObject.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If enemy is attacked
        if (other.gameObject.CompareTag("Attack"))
        {
            // parentScript.health -= 1;
            audioPlayer.PlayOneShot(damaged, 0.5f);
            /*
            // Attacked by melee
            if (other.gameObject.name == "AttackHitBox")
            {
                parentScript.health -= other.gameObject.GetComponentInParent<PlayerController>().meleeDamage;
            }
            // Attacked by projectile
            else
            {
                parentScript.health -= 1;
            }
            */
            /*
            parentScript.sr.material = parentScript.matWhite;

            if (parentScript.health <= 0)
            {
                parentScript.KillSelf();
            }
            else
            {
                parentScript.Invoke("ResetMaterial", 0.1f);
            }*/
        }
    }
}
