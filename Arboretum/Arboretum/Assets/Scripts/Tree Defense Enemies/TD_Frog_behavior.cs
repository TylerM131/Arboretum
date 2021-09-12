using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_Frog_behavior : MonoBehaviour
{
    private GameObject tree;
    [SerializeField] float xrangemin = 6;

    private bool isJumping;
    private bool isAttacking;
    private bool canUseTongue;

    private float xdistance;
    private float ydistance;

    private Animator anim;

    private GameObject tongue;
    private GroundCheck groundSensor;
    private Rigidbody2D rb;

    private bool wasGrounded = false;
    public bool living = true;

    private float jumpCooldown = 1f;
    private float timeSinceJumpEnd = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        isJumping = false;
        isAttacking = false;
        canUseTongue = false;
        tree = GameObject.Find("Tree");
        tongue = transform.Find("Tongue").gameObject;
        groundSensor = transform.Find("GroundSensor").gameObject.GetComponent<GroundCheck>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!living)
            return;

        rb.WakeUp();

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

        // neg: tree is left of frog | pos: tree is right of frog
        xdistance = tree.transform.position.x - transform.position.x;

        Facetree();

        if (Mathf.Abs(xdistance) < xrangemin)
        {
            Attack();
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

    void Facetree()
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
