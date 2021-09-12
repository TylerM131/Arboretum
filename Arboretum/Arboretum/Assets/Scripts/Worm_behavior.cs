using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm_behavior : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;
    [SerializeField] float leftDistance = 1.0f;
    [SerializeField] float rightDistance = 1.0f;
    [SerializeField] float range = 20.0f;

    private Vector2 start;
    private Vector2[] waypoints = new Vector2[2]; // Left: 0, Right: 1

    private Animator anim;

    // States 0: Moving, 1: Attacking
    private int state;
    private bool canAttack = false;
    const int MOVING = 0;
    const int ATTACKING = 1;

    [SerializeField] float turnRange = 0.02f;
    [SerializeField] int nextWaypoint = 1;
    private float distToPoint;
    private float distanceToPlayer;
    private GameObject player;
    private int prevCollider = 0;

    [SerializeField] GameObject fireball;
    [SerializeField] float startAttackingTime = 5.0f;
    [SerializeField] int fireballCount = 1;
    private int fireballsShot = 0;

    // [SerializeField] bool facePlayerWhenAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        start = new Vector2(transform.position.x, transform.position.y);
        waypoints[0] = new Vector2(start.x - leftDistance, start.y);
        waypoints[1] = new Vector2(start.x + rightDistance, start.y);

        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        state = MOVING;

        Invoke("StartAttacking", startAttackingTime);
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer < range)
            canAttack = true;
        else
            canAttack = false;

        // Move between the waypoints
        if (state == MOVING)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Move"))
                anim.SetTrigger("move");
            Move();
        }

        // Shoot fireballs at the player
        else if (state == ATTACKING)
        {
            if (canAttack)
            {
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                    anim.SetTrigger("attack");

                FacePlayer();

                // if (facePlayerWhenAttacking)
                //    FacePlayer();
            }
            else
            {
                state = MOVING;
                Invoke("StartAttacking", startAttackingTime);
            }
        }
    }

    private void Move()
    {
        // Move to next waypoint, ignore y value of point positions
        FaceWaypoint();
        distToPoint = Vector2.Distance(transform.position, new Vector2(waypoints[nextWaypoint].x, transform.position.y));
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(waypoints[nextWaypoint].x, transform.position.y), speed * Time.deltaTime);

        if (distToPoint <= turnRange)
        {
            TakeTurn();
        }
    }

    // Start attacking if in range
    private void StartAttacking()
    {
        state = ATTACKING;
    }

    private void StartMoving()
    {
        state = MOVING;
    }

    void TakeTurn()
    {
        nextWaypoint = 1 - nextWaypoint;   
    }

    public void ChangeCollider(int n)
    {
        gameObject.GetComponents<PolygonCollider2D>()[prevCollider].enabled = false;
        prevCollider = n;
        gameObject.GetComponents<PolygonCollider2D>()[n].enabled = true;
    }

    void FacePlayer()
    {
        float xdistance = player.transform.position.x - transform.position.x;
        if (xdistance < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,
                                               transform.localScale.z);
        }
        else if (xdistance > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,
                                               transform.localScale.z);
        }
    }

    void FaceWaypoint()
    {
        // Waypoint is left of player
        if (waypoints[nextWaypoint].x - transform.position.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,
                                               transform.localScale.z);
        }
        // Waypoing is right of player
        else if (waypoints[nextWaypoint].x - transform.position.x > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,
                                               transform.localScale.z);
        }
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
}
