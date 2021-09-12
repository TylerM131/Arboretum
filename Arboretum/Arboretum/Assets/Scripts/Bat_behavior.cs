using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat_behavior : MonoBehaviour
{
    private GameObject player;
    [SerializeField] float speed = 4.0f;
    [SerializeField] float attackSpeed = 4.0f;
    [SerializeField] GameObject[] waypoints;
    [SerializeField] float range = 5.0f;

    private int nextWaypoint = 1;
    private float distToPoint;
    private bool moving = true;
    private bool attacking = false;
    private bool canAttack = true;
    private float distanceToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < range && canAttack)
        {
            attacking = true;
        }

        if (attacking)
        {
            moving = false;
            Attack();
        }

        if (moving)
            Move();
    }

    void Attack()
    {
        FacePlayer();
        // rb.AddForce( 0.33f * (player.transform.position - transform.position), ForceMode2D.Impulse);
        Vector2 batPos = transform.position;
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, player.transform.position.y + 2.0f),
                                    attackSpeed * Time.deltaTime);

        // Player is out of range
        if (distanceToPlayer >= range)
        {
            attacking = false;
            moving = true;
            nextWaypoint = ClosestWaypoint();
            FaceWaypoint();
        }
    }

    void Move()
    {
        FaceWaypoint();
        distToPoint = Vector2.Distance(transform.position, waypoints[nextWaypoint].transform.position);
        transform.position = Vector2.MoveTowards(transform.position, waypoints[nextWaypoint].transform.position,
                                    speed * Time.deltaTime);
        if (distToPoint <= 0.1)
        {
            canAttack = true;
            TakeTurn();
        }
    }

    void TakeTurn()
    {
        ChooseNextWaypoint();
    }

    void ChooseNextWaypoint()
    {
        nextWaypoint++;

        if (nextWaypoint == waypoints.Length)
        {
            nextWaypoint = 0;
        }
    }

    void FacePlayer()
    {
        if (player.transform.position.x - transform.position.x < 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,
                                               transform.localScale.z);
        }
        else if (player.transform.position.x - transform.position.x > 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,
                                               transform.localScale.z);
        }
    }

    void FaceWaypoint()
    {
        if (waypoints[nextWaypoint].transform.position.x - transform.position.x < 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,
                                               transform.localScale.z);
        }
        else if (waypoints[nextWaypoint].transform.position.x - transform.position.x > 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,
                                               transform.localScale.z);
        }
    }

    // Returns the closest waypoint on the same side of the player as the last waypoint
    private int ClosestWaypoint()
    {
        int retval = 0;
        float distance = float.MaxValue;
        int i;
        for (i = 0; i < waypoints.Length; i++)
        {
            float dist = Vector2.Distance(transform.position, waypoints[i].transform.position);
            if (OnSameSide(gameObject, waypoints[i]) && dist < distance)
            {
                retval = i;
                distance = dist;
            }
                
        }
        return retval;
    }

    // Are object 1 and 2 on same side of player
    private bool OnSameSide(GameObject obj1, GameObject obj2)
    {
        float obj1pos = obj1.transform.position.x;
        float obj2pos = obj2.transform.position.x;
        float playerpos = player.transform.position.x;

        return (obj1pos > playerpos && obj2pos > playerpos) || (obj1pos < playerpos && obj2pos < playerpos);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If bat attacks player
        if ((collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Attack")) && attacking == true)
        {
            canAttack = false;
            attacking = false;
            moving = true;
            nextWaypoint = ClosestWaypoint();
            FaceWaypoint();
        }
    }
}
