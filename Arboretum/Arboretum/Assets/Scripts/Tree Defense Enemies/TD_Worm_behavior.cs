using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_Worm_behavior : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;
    
    private Animator anim;

    // States 0: Moving, 1: Attacking
    private int state;
    const int MOVING = 0;
    const int ATTACKING = 1;
  
    private int prevCollider = 0;
    [SerializeField] GameObject fireball;
    [SerializeField] float startAttackingTime = 5.0f;
    [SerializeField] int fireballCount = 1;
    private int fireballsShot = 0;
    private GameObject tree;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        state = MOVING;

        tree = GameObject.Find("Tree");
        if (tree != null)
            FaceTree();

        Invoke("StartAttacking", startAttackingTime);
    }

    // Update is called once per frame
    void Update()
    {
        // Move to the tree
        if (state == MOVING)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Move"))
                anim.SetTrigger("move");
            Move();
        }

        // Shoot fireballs at the player
        else if (state == ATTACKING)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                anim.SetTrigger("attack");
        }
    }

    private void Move()
    {
        if (tree != null)
            transform.position = Vector2.MoveTowards(transform.position, tree.transform.position, speed * Time.deltaTime);
    }

    public void ChangeCollider(int n)
    {
        gameObject.GetComponents<PolygonCollider2D>()[prevCollider].enabled = false;
        prevCollider = n;
        gameObject.GetComponents<PolygonCollider2D>()[n].enabled = true;
    }

    public void Shoot()
    {
        fireballsShot++;
        GameObject fb = Instantiate(fireball);
        fb.transform.localScale = transform.localScale;
        if (fb.transform.localScale.x > 0)
            fb.transform.position = new Vector3(transform.position.x + 2.0f, transform.position.y - 0.5f, fb.transform.position.z);
        else
            fb.transform.position = new Vector3(transform.position.x - 2.0f, transform.position.y - 0.5f, fb.transform.position.z);

        // Start moving again, but switch back to attacking after some time
        if (fireballsShot >= fireballCount)
        {
            fireballsShot = 0;
            Invoke("StartMoving", 0.8f);
            Invoke("StartAttacking", startAttackingTime);
        }
    }

    private void StartAttacking()
    {
        state = ATTACKING;
    }

    private void StartMoving()
    {
        state = MOVING;
    }

    void FaceTree()
    {
        // Face left
        if (tree.transform.position.x - transform.position.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,
                                               transform.localScale.z);
        }
        // Face right
        else if (tree.transform.position.x - transform.position.x > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,
                                               transform.localScale.z);
        }
    }
}