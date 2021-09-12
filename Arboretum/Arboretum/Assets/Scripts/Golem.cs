using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;
    [SerializeField] float viewConeAngle = 60;
    [SerializeField] float range = 12.0f;

    private GameObject player;
    private Animator anim;
    private BoxCollider2D bc;
    private float playerOffset;
    private bool canSee;
    private GroundCheck groundSensor;

    public float distanceToPlayer;

    // States 0: Idle, 1: Moving
    public int state;
    const int IDLE = 0;
    const int MOVING = 1;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        anim = GetComponent<Animator>();
        bc = GetComponent<BoxCollider2D>();
        playerOffset = player.GetComponent<BoxCollider2D>().size.y;
        groundSensor = transform.Find("GroundSensor").gameObject.GetComponent<GroundCheck>();
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        
        // Determine if golem can see player
        Vector2 golemToPlayerHead = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y + playerOffset - transform.position.y);
        Vector2 golemToPlayerFeet = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
        
        Vector2 golemFaceDirection;
        // Player left of golem
        if (transform.position.x - player.transform.position.x >= 0)
        {
            golemFaceDirection = new Vector2(-1, 0);
        }
        // Player right of golem
        else
        {
            golemFaceDirection = new Vector2(1, 0);
        }

        golemToPlayerHead.Normalize();
        golemToPlayerFeet.Normalize();
        canSee = Vector2.Dot(golemToPlayerHead, golemFaceDirection) > Mathf.Cos(Mathf.Deg2Rad * viewConeAngle) && Vector2.Dot(golemToPlayerFeet, golemFaceDirection) > Mathf.Cos(Mathf.Deg2Rad * viewConeAngle);

        // Set state
        if (canSee && distanceToPlayer < range)
        {
            FacePlayer();
            if (groundSensor.grounded)
            {
                state = MOVING;
            }
            else
            {
                state = IDLE;
            }
        }
        else
        {
            state = IDLE;
        }

        // Walk towards player
        if (state == MOVING)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("GolemWalk"))
                anim.SetTrigger("walk");

            transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, transform.position.y), speed * Time.deltaTime);
        }
        // Stay idle
        else
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("GolemIdle"))
                anim.SetTrigger("idle");
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
}
