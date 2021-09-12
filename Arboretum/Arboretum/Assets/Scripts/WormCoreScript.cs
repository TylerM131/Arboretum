using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormCoreScript : MonoBehaviour
{
    // Core Attributes
    [SerializeField] EnemyScriptObj obj;
    [HideInInspector] public int health;
    [HideInInspector] public int damage;

    // Damage and death animations
    SpriteRenderer sr;
    private Material matWhite;
    private Material matDefault;
    private Object explosionRef;
    private PolygonCollider2D pc;

    // Drops
    [SerializeField] int dropPercent = 20;
    [SerializeField] GameObject[] drops;

    // Audio
    private AudioSource audioPlayer;
    [SerializeReference] AudioClip damagedSound;
    public AudioClip deathSound;

    // Can this enemy take damage
    [SerializeField] bool isInvincible;

    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = gameObject.GetComponent<AudioSource>();

        sr = GetComponent<SpriteRenderer>();
        matWhite = Resources.Load("WhiteFlash", typeof(Material)) as Material;

        if (sr != null)
            matDefault = sr.material;
        else
        {
            sr = transform.parent.GetComponent<SpriteRenderer>();
            if (sr != null)
                matDefault = sr.material;
        }

        explosionRef = Resources.Load("Explosion");

        health = obj.initialHealth;
        damage = obj.damage;

        pc = gameObject.GetComponent<PolygonCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If enemy is attacked
        if (collision.gameObject.CompareTag("Attack") && !isInvincible)
        {
            isInvincible = true;
            Debug.Log(gameObject.name);
            health -= 1;
            audioPlayer.PlayOneShot(damagedSound, 0.1f);

            sr.material = matWhite;

            if (health <= 0)
            {
                KillSelf();
            }
            else
            {
                Invoke("ResetMaterial", 0.1f);
            }
        }
    }

    void ResetMaterial()
    {
        sr.material = matDefault;
        isInvincible = false;
    }

    private void KillSelf()
    {
        // Kill Enemy
        AudioSource.PlayClipAtPoint(deathSound, transform.position, 1f);
        GameObject explosion = (GameObject)Instantiate(explosionRef);
        explosion.transform.position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
        Vector3 enemyPos = transform.position;

        if (GetComponent<SpriteRenderer>() != null)
            Destroy(gameObject);
        else
            Destroy(transform.parent.gameObject);

        // Drop Crystal
        if (drops.Length >= 1)
        {
            int r = Random.Range(0, 100);
            // Debug.Log(r);
            // Debug.Log(dropPercent);
            if (r < dropPercent)
            {
                GameObject crystal = Instantiate(drops[Random.Range(0, drops.Length)]);
                crystal.transform.position = new Vector3(enemyPos.x, enemyPos.y + 0.2f, enemyPos.z);
            }
        }
    }
}
