using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vulture_behavior : MonoBehaviour
{
    private GameObject player;
    [SerializeField] float speed = 4.0f;
    [SerializeField] float attackSpeed = 4.0f;

    // Waypoints to fly between
    [SerializeField] GameObject[] waypoints;

    // Subset of waypoints, when 1 is reached, there's a landPercent chance of landing at the nearest toLand point
    [SerializeField] GameObject[] fromLand;
    [SerializeField] int landPercent = 20;

    // Set of landing locations
    [SerializeField] GameObject[] toLand;

    // X distance from landing location that the Vulture actually lands
    [SerializeField] float landingDistRange = 2.5f;
    private int closestPoint;

    // Starts attacking player when within this range, and returns to previous state when out of range
    [SerializeField] float range = 5.0f;

    private int nextWaypoint;
    private float distToPoint;
    private bool canAttack = true;
    private bool idling = false;
    private bool attacking = false;
    private float landingDistance = 0;
    private float distanceToPlayer;

    // States 0: Idle, 1: Moving, 2: Attacking, 3: Landing
    public int state;
    public int prevState;
    const int IDLE = 0;
    const int MOVING = 1;
    const int ATTACKING = 2;
    const int LANDING = 3;

    // Half of box collider height, offset from toLand point
    private float halfHeight;

    private Animator anim;

    // Every moveTime seconds there's a movePercent chance to change state from landing to moving
    [SerializeField] float moveTime = 1.0f;
    [SerializeField] float movePercent = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        anim = GetComponent<Animator>();
        halfHeight = (GetComponent<BoxCollider2D>().size.y / 2) * transform.localScale.y;
        nextWaypoint = ClosestWaypoint();

        // start by landing to the ground
        StartLand();

        // Timer to change state
        StartCoroutine("SwitchStates");

    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (!attacking && canAttack && distanceToPlayer < range)
        {
            anim.SetTrigger("attack");
            prevState = state;
            state = ATTACKING;
        }

        // Idle state
        if (state == IDLE)
        {
            if (!idling)
            {
                idling = true;
                FacePlayer();
                anim.SetTrigger("idle");
            }
        }
        else
        {
            idling = false;
        }

        // Attack state
        if (state == ATTACKING)
        {
            if (!attacking)
            {
                attacking = true;
                anim.SetTrigger("attack");
            }
            Attack();
        }
        else
        {
            attacking = false;
        }

        // Moving state
        if (state == MOVING)
        {
            anim.SetTrigger("fly");
            Move();
        }

        // Landing state
        if (state == LANDING)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Vulture_fly"))
                anim.SetTrigger("fly");
            Land();
        }
    }

    private IEnumerator SwitchStates()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveTime);
            if (state == IDLE)
            {
                float r = Random.Range(0, 100);
                // Debug.Log(r);
                if (r < movePercent)
                {
                    state = MOVING;
                }
            }
        }
    }

    void Attack()
    {
        FacePlayer();
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, player.transform.position.y + 2.0f),
                                    attackSpeed * Time.deltaTime);

        // Player is out of range
        if (distanceToPlayer >= range)
        {
            anim.SetTrigger("fly");
            canAttack = false;

            if (prevState == MOVING)
            {
                nextWaypoint = ClosestWaypoint();
                FaceWaypoint();
                state = MOVING;
            }
            else
            {
                StartLand();
            }
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

            // At fromLand point
            bool isFromLandPoint = false;
            for (int i = 0; i < fromLand.Length; i++)
            {
                if (waypoints[nextWaypoint] == fromLand[i])
                {
                    isFromLandPoint = true;
                }
            }

            if (isFromLandPoint)
            {
                // Land
                int r = Random.Range(0, 100);
                Debug.Log(r);
                if (r < landPercent)
                {
                    StartLand();
                }
                // Keep flying
                else
                {
                    TakeTurn();
                }
            }
            // At any other waypoint
            else
            {
                TakeTurn();
            }
        }
    }

    void StartLand()
    {
        // Calculate nearest landing point
        int i = 0;
        closestPoint = 0;
        float minDist = Vector2.Distance(transform.position, new Vector3(toLand[i].transform.position.x, toLand[i].transform.position.y + halfHeight, toLand[i].transform.position.z));
        for (i = 1; i < toLand.Length; i++)
        {
            float iDist = Vector2.Distance(transform.position, new Vector3(toLand[i].transform.position.x, toLand[i].transform.position.y + halfHeight, toLand[i].transform.position.z));
            if (iDist < minDist)
            {
                minDist = iDist;
                closestPoint = i;
            }
        }

        // Calculate distance from landing point to actually land
        landingDistance = Random.Range(-landingDistRange * 1000, landingDistRange * 1000) / 1000;
        state = LANDING;
    }

    void Land()
    {
        FaceWaypoint(toLand[closestPoint]);

        // Move to closest toLand point
        if (Vector2.Distance(transform.position, new Vector3(toLand[closestPoint].transform.position.x + landingDistance, toLand[closestPoint].transform.position.y + halfHeight, toLand[closestPoint].transform.position.z)) > 0.1)
            transform.position = Vector2.MoveTowards(transform.position, new Vector3(toLand[closestPoint].transform.position.x + landingDistance, toLand[closestPoint].transform.position.y + halfHeight, toLand[closestPoint].transform.position.z),
                                 speed * Time.deltaTime);
        // Stop moving
        else
        {
            canAttack = true;
            state = IDLE;
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

    // Face the next waypoint
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

    // Face a specific waypoint
    void FaceWaypoint(GameObject point)
    {
        if (point.transform.position.x + landingDistance - transform.position.x < 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,
                                               transform.localScale.z);
        }
        else if (point.transform.position.x + landingDistance - transform.position.x > 0 && transform.localScale.x > 0)
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
        Debug.Log("triggered");
        Debug.Log(collision.gameObject.tag);

        // If vulture attacks player
        if (canAttack && (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Attack")))
        {
            Debug.Log("attacked");
            anim.SetTrigger("fly");
            canAttack = false;

            if (prevState == MOVING)
            {
                Debug.Log("back to moving");
                nextWaypoint = ClosestWaypoint();
                FaceWaypoint();
                state = MOVING;
            }
            else
            {
                Debug.Log("back to landing");
                StartLand();
            }
        }
    }
}
