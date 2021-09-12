using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hyena_behavior : MonoBehaviour
{
    [SerializeField] float speed = 3f;
    [SerializeField] float runSpeed = 3.5f;
    [SerializeField] float speedAnimationMult = 1.3f;
    [SerializeField] GameObject[] waypoints;

    public int nextWaypoint = 1;
    private float distToPoint;
    private bool doingAction = false;
    private bool canRunOrAttack = true;
    [SerializeField] float viewConeAngle = 85;
    [SerializeField] float backViewConeAngle = 30;

    /*
    // View viewCone and range circle
    [SerializeField] GameObject viewConePointPrefab;
    private GameObject[] viewConePoints;
    [SerializeField] GameObject viewConeCirclePrefab;
    private GameObject viewConeCircle;
    */

    private GameObject player;
    [SerializeField] float rangemin = 2.3f;
    [SerializeField] float rangemax = 9;
    public float distanceToPlayer;

    private Animator anim;
    private BoxCollider2D bc;
    private float playerOffset;

    private void Start()
    {
        player = GameObject.Find("Player");
        anim = GetComponent<Animator>();
        bc = GetComponent<BoxCollider2D>();
        playerOffset = player.GetComponent<BoxCollider2D>().size.y;
        
        /*
        // View viewCone and range circle
        viewConePoints = new GameObject[9];
        for (int i = 0; i < 9; i++)
        {
            viewConePoints[i] = Instantiate(viewConePointPrefab);
        }
        viewConeCircle = Instantiate(viewConeCirclePrefab);
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (doingAction)
            return;

        // 1: left, -1: right
        float faceDirection = Mathf.Abs(transform.localScale.x) / transform.localScale.x;

        /*
        // View viewCone and range circle
        viewConePoints[0].transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        viewConePoints[1].transform.position = new Vector3(player.transform.position.x, player.transform.position.y + playerOffset, player.transform.position.z);
        viewConePoints[2].transform.position = transform.position;

        for (int i = 3; i < 9; i+=2)
        {
            viewConePoints[i].transform.position = new Vector3(transform.position.x - 2 * i * faceDirection, transform.position.y + 2 * i * Mathf.Tan(Mathf.Deg2Rad * viewConeAngle), transform.position.z);
            viewConePoints[i+1].transform.position = new Vector3(transform.position.x - 2 * i * faceDirection, transform.position.y - 2 * i * Mathf.Tan(Mathf.Deg2Rad * viewConeAngle), transform.position.z);
        }
        viewConeCircle.transform.position = transform.position;
        viewConeCircle.transform.localScale = new Vector3(2 * rangemax, 2 * rangemax, 1);
        */
        
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Determine if hyena can see player
        Vector2 hyenaToPlayerHead = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y + playerOffset - transform.position.y);
        Vector2 hyenaToPlayerFeet = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);

        Vector2 hyenaFaceDirection;
        // Player left of golem
        if (transform.position.x - player.transform.position.x >= 0)
        {
            hyenaFaceDirection = new Vector2(-1, 0);
        }
        // Player right of golem
        else
        {
            hyenaFaceDirection = new Vector2(1, 0);
        }

        hyenaToPlayerHead.Normalize();
        hyenaToPlayerFeet.Normalize();
        bool canSee = Vector2.Dot(hyenaToPlayerHead, hyenaFaceDirection) > Mathf.Cos(Mathf.Deg2Rad * (IsFacingPlayer() ? viewConeAngle : backViewConeAngle)) || Vector2.Dot(hyenaToPlayerFeet, hyenaFaceDirection) > Mathf.Cos(Mathf.Deg2Rad * (IsFacingPlayer() ? viewConeAngle : backViewConeAngle));

        Debug.Log(canSee);

        // Run
        if (canSee && canRunOrAttack && Mathf.Abs(distanceToPlayer) < rangemax && Mathf.Abs(distanceToPlayer) > rangemin)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Hyena_walk"))
                anim.SetTrigger("walk");
            anim.SetFloat("speedMult", speedAnimationMult);
            doingAction = true;
            bc.offset = new Vector2(0, 0);
            bc.size = new Vector2(0.33f, 0.26f);
            Run();
        }
        // Walk
        else if (!canSee || (!canRunOrAttack || Mathf.Abs(distanceToPlayer) >= rangemax))
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Hyena_walk"))
                anim.SetTrigger("walk");
            anim.SetFloat("speedMult", 1.0f);
            doingAction = true;
            bc.offset = new Vector2(0, 0);
            bc.size = new Vector2(0.33f, 0.26f);
            Walk();
        }
        // Attack
        else if (canSee && canRunOrAttack && Mathf.Abs(distanceToPlayer) <= rangemin)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Hyena_attack"))
                anim.SetTrigger("attack");
            doingAction = true;
            Attack();
        }
    }

    void Attack()
    {
        FacePlayer();
        Invoke("StopAttack", 1.5f);
        bc.offset = new Vector2(-0.03f, 0);
        bc.size = new Vector2(0.39f, 0.26f);


    }
    
    void StopAttack()
    {
        doingAction = false;
    }

    void Run()
    {
        FacePlayer();
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, transform.position.y),
                                    runSpeed * Time.deltaTime);
        doingAction = false;

        /*
        // Player is out of range
        if (distanceToPlayer >= rangemax)
        {
            running = false;
            walking = true;
            nextWaypoint = closestWaypoint();
            FaceWaypoint();
        }
        // Player is too close
        else if (distanceToPlayer <= rangemin)
        {
            running = false;
            doingAction = true;
        }*/
    }

    void Walk()
    {
        FaceWaypoint();
        distToPoint = Mathf.Abs(transform.position.x - waypoints[nextWaypoint].transform.position.x);
        transform.position = Vector2.MoveTowards(transform.position, new Vector3(waypoints[nextWaypoint].transform.position.x, transform.position.y, transform.position.z),
                                    speed * Time.deltaTime);

        if (distToPoint <= 0.1)
        {
            ChooseNextWaypoint();

            // Stop just walking back from hitting barrier
            canRunOrAttack = true;
        }

        doingAction = false;

        /*
        // Player is too close
        if (distanceToPlayer <= rangemin)
        {
            walking = false;
            doingAction = true;
        }*/
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

    bool IsFacingPlayer()
    {
        return !(player.transform.position.x - transform.position.x < 0 && transform.localScale.x < 0) && !(player.transform.position.x - transform.position.x > 0 && transform.localScale.x > 0);
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

    int closestWaypoint()
    {
        int retval = 0;
        float distance = Vector2.Distance(transform.position, waypoints[0].transform.position);
        int i;
        for (i = 1; i < waypoints.Length; i++)
        {
            if (Vector2.Distance(transform.position, waypoints[i].transform.position) < distance)
                retval = i;
        }
        return retval;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If you hit the barrier, start walking again
        if (collision.gameObject.CompareTag("Barrier"))
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Hyena_walk"))
                anim.SetTrigger("walk");
            doingAction = true;
            canRunOrAttack = false;
            bc.offset = new Vector2(0, 0);
            bc.size = new Vector2(0.33f, 0.26f);
            Walk();
        }
    }
}
