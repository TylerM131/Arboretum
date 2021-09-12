using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tree_behavior : MonoBehaviour
{
    [SerializeField] TilemapRenderer sr;
    private Material matWhite;
    private Material matDefault;
    private Object explosionRef;
    public int health = 10;
    public int maxhealth = 10;
    private GameObject lastEnemy;
    public bool isDead = false;
    [HideInInspector] public Animator anim;

    private void Start()
    {
        sr = GetComponent<TilemapRenderer>();
        matWhite = Resources.Load("TreeFlash", typeof(Material)) as Material;
        matDefault = sr.material;
        explosionRef = Resources.Load("Red Explosion");
        anim = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Enemies
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy Coll");
            // Kill Enemy
            GameObject enemy = collision.gameObject.transform.root.gameObject;
            if (enemy == lastEnemy)
                return;
            lastEnemy = enemy;
            GameObject explosion = (GameObject)Instantiate(explosionRef);
            explosion.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y + 0.3f, enemy.transform.position.z);
            
            if (enemy.GetComponent<EnemyCoreScript>() != null)
                AudioSource.PlayClipAtPoint(enemy.GetComponent<EnemyCoreScript>().deathSound, enemy.transform.position, 1.0f);
            else if (enemy.GetComponentInChildren<EnemyCoreScript>() != null)
                AudioSource.PlayClipAtPoint(enemy.GetComponentInChildren<EnemyCoreScript>().deathSound, enemy.transform.position, 1.0f);

            Destroy(enemy);

            // Damage Tree
            sr.material = matWhite;
            health--;
            TreeHealthSystem.Instance.SetHealth(health);
            Invoke("ResetMaterial", 0.1f);

            if (health <= 0)
            {
                // Tree is dead
                isDead = true;
            }
        }

        // Fireballs 
        else if (collision.gameObject.CompareTag("Hazard"))
        {
            // Damage Tree
            sr.material = matWhite;
            health--;
            TreeHealthSystem.Instance.SetHealth(health);
            Invoke("ResetMaterial", 0.1f);

            if (health <= 0)
            {
                // Tree is dead
                isDead = true;
            }
        }
    }

    void ResetMaterial()
    {
        sr.material = matDefault;
    }
}
