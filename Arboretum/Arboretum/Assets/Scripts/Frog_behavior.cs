using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog_behavior : MonoBehaviour
{
    private GameObject player;
    [SerializeField] float xrangemin = 6;
    [SerializeField] float yrangemin = 1;
    [SerializeField] float xrangemax = 12;
    [SerializeField] float yrangemax = 2;

    public bool isJumping;
    public bool isAttacking;
    public bool canUseTongue;

    private float xdistance;
    private float ydistance;

    private Animator anim;

    private GameObject tongue;
    private GroundCheck groundSensor;
    private Rigidbody2D rb;

    public bool wasGrounded = false;
    public bool living = true;

    private float jumpCooldown = 0.4f;
    private float timeSinceJumpEnd = 0.0f;

    // SpriteRenderer tongueSr;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        isJumping = false;
        isAttacking = false;
        canUseTongue = false;
        player = GameObject.Find("Player");
        tongue = transform.Find("Tongue").gameObject;
        groundSensor = transform.Find("GroundSensor").gameObject.GetComponent<GroundCheck>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!living)
            return;

        // Increase timer that prevents ending jump twice in a row
        timeSinceJumpEnd += Time.deltaTime;

        if (!wasGrounded && groundSensor.grounded && isJumping && !isAttacking && timeSinceJumpEnd > jumpCooldown)
        {
            anim.SetTrigger("returnToIdle");
            Invoke("FinishedJumping", jumpCooldown);

            // Reset timer
            timeSinceJumpEnd = 0.0f;
        }

        else if (!isJumping && !isAttacking && wasGrounded)
            MakeDecision();

        wasGrounded = groundSensor.grounded;
    }

    // If the frog is not jumping or attacking, decide which action to take
    void MakeDecision()
    {
      
        // neg: player is left of frog | pos: player is right of frog
        xdistance = player.transform.position.x - transform.position.x;
        // neg: player is under from | pos: player is above frog
        ydistance = player.transform.position.y - transform.position.y;

        FacePlayer();

        if (Mathf.Abs(xdistance) < xrangemin && Mathf.Abs(ydistance) < yrangemin)
        {
            Attack();
        }
        else if (Mathf.Abs(xdistance) > xrangemax || Mathf.Abs(ydistance) > yrangemax)
        {
            // Don't Move
        }
        else
        {
            Jump();
        }
    }

    void Attack()
    {
        isAttacking = true;
        canUseTongue = true;
        AttackAnim();
        Invoke("DespawnTongue", 3f);
    }

    void AttackAnim()
    {
        anim.SetTrigger("doAttackAnim");
    }

    // Triggered within attack animation
    void SpawnTongue()
    {
        if (!tongue.activeSelf && canUseTongue)
        {
            tongue.SetActive(true);
        }
    }

    void DespawnTongue()
    {
        canUseTongue = false;
        tongue.SetActive(false);
        Invoke("StopAttack", 0.1f);
    }

    void StopAttack()
    {
        anim.SetTrigger("stopAttackAnim");
        isAttacking = false;
    }

    void Jump()
    {
        isJumping = true;
        anim.SetTrigger("doJumpAnim");
    }

    void JumpMovement()
    {
        float xVect;
        // Facing Left
        if (transform.localScale.x < 0)
        {
            xVect = Random.Range(-0.75f, -0.5f);
            rb.AddForce(new Vector2(5 * xVect, 25 * (1 + xVect)), ForceMode2D.Impulse);
        }
        // Facing Right
        else
        {
            xVect = Random.Range(0.5f, 0.75f);
            rb.AddForce(new Vector2(5 * xVect, 25 * (1 - xVect)), ForceMode2D.Impulse);
        }
    }

    void FinishedJumping()
    {
        isJumping = false;
    }

    void FacePlayer()
    {
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
}
