using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Player Controls
    private float moveInput;

    // Resources
    public int health = 5;
    public int maxhealth = 5;
    public int ammo = 3;
    public int maxammo = 3;

    public int meleeDamage = 1;

    [SerializeField] float playerSpeed = 6;
    [SerializeField] float jumpForce = 15;
    [SerializeField] float rollForce = 6;
    [SerializeField] float attackDelay = 0.33f;
    [SerializeField] float attackComboTimer = 1.0f;
    // [SerializeField] float block_cooldown = 3.0f;
    [SerializeField] float invincibilty_duration = 1.0f;
    [SerializeField] float invincibility_fade = 0.4f;
    
    [SerializeField] LayerMask groundLayer;
    [SerializeReference] GameObject projectilePrefab;
    float projectileOffsetX = 2.5f;
    float projectileOffsetY = 1.5f;

    private Rigidbody2D playerRb;
    private BoxCollider2D playerCollider;
    private Animator animator;
    private SpriteRenderer sr;
    private GroundCheck playerGrounded;
    private GameObject attackHitBox;
    private GameObject shortHitBox;


    // Resources
    private int facing_direction;
    private bool rolling;
    private int currentAttack;
    private float timeSinceAttack;
    [HideInInspector] public  bool can_block;
    public bool isInvincible;
    public bool isDead;
    private Animator shieldUIAnim;

    //Ceiling
    private bool touching_ceil;
    private float ceil_height;
    private Vector2 ceil_pos;

    // Sound effects
    private AudioSource playerAudio;
    [SerializeReference] AudioClip attack1Sound;
    [SerializeReference] AudioClip attack2Sound;
    [SerializeReference] AudioClip attack3Sound;
    [SerializeReference] AudioClip jumpSound;
    [SerializeReference] AudioClip hurtSound;
    [SerializeReference] AudioClip rollSound;
    [SerializeReference] AudioClip deathSound;
    public AudioClip pickupSound;
    [SerializeReference] AudioClip fallSound;
    [SerializeReference] AudioClip blockSound;
    [SerializeReference] AudioClip teleportSound;


    // Start is called before the first frame update
    void Start()
    {
        playerAudio = GetComponent<AudioSource>();
        playerRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<BoxCollider2D>();
        attackHitBox = transform.Find("AttackHitBox").gameObject;
        shortHitBox = transform.Find("ShortHitBox").gameObject;
        playerGrounded = transform.Find("GroundSensor").GetComponent<GroundCheck>();
        facing_direction = 1;
        rolling = false;
        currentAttack = 0;
        timeSinceAttack = 0.0f;
        can_block = true;
        isInvincible = false;
        isDead = false;
        touching_ceil = false;
        ceil_height = playerCollider.bounds.center.y + playerCollider.bounds.extents.y - transform.position.y;
        ceil_pos = new Vector2(transform.position.x, transform.position.y + ceil_height);
        shieldUIAnim = GameObject.Find("ShieldUI").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead || PauseMenu.isPaused)
            return;

        if (health <= 0)
        {
            isDead = true;
            playerRb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            animator.SetTrigger("Death");
            HealthSystem.Instance.SetHealth(health);
            HealthSystem.Instance.SetMana(ammo);
            return;
        }

        // Increase timer that controls attack combo
        timeSinceAttack += Time.deltaTime;

        // Touching ceiling
        ceil_pos = new Vector2(transform.position.x, transform.position.y + ceil_height);
        // Debug.DrawLine(transform.position, new Vector3(ceil_pos.x, ceil_pos.y, -10), new Color(1, 1, 1, 1), 10.0f);
        if (Physics2D.OverlapCircle(ceil_pos, 0.5f, groundLayer))
            touching_ceil = true;
        else
            touching_ceil = false;

        // Falling animations
        animator.SetBool("Grounded", playerGrounded.grounded);
        animator.SetFloat("AirSpeedY", playerRb.velocity.y);

        Move(moveInput);

        HealthSystem.Instance.SetHealth(health);
        HealthSystem.Instance.SetMana(ammo);
    }

    // Movement
    public void OnMove(InputValue input)
    {
        moveInput = input.Get<float>();
    }

    private void Move(float move)
    {
        if (move > 0 && !rolling)
        {
            //Set direction
            facing_direction = 1;
            transform.localScale = new Vector3(1, 1, 1);
            animator.SetInteger("AnimState", 1);

            // Move
            playerRb.velocity = new Vector2(move * playerSpeed, playerRb.velocity.y);
        }
        else if (move < 0 && !rolling)
        {
            //Set direction
            facing_direction = -1;
            transform.localScale = new Vector3(-1, 1, 1);
            animator.SetInteger("AnimState", 1);

            // Move
            playerRb.velocity = new Vector2(move * playerSpeed, playerRb.velocity.y);
        }
        else
        {
            animator.SetInteger("AnimState", 0);
            playerRb.WakeUp();
            if (!rolling)
                playerRb.velocity = new Vector2(0, playerRb.velocity.y);
        }
    }

    // Can you take an action?
    private bool CanAct()
    {
        if (PauseMenu.isPaused || isDead || rolling)
            return false;
        return true;
    }

    //Melee Attack
    public void OnAttack()
    {
        if (timeSinceAttack < attackDelay || !CanAct())
            return;

        currentAttack++;

        // Loop back to one after third attack
        if (currentAttack > 3)
            currentAttack = 1;

        // Reset Attack combo if time since last attack is too large
        if (timeSinceAttack > attackComboTimer)
            currentAttack = 1;

        if (currentAttack == 1)
            playerAudio.PlayOneShot(attack1Sound, 0.5f);
        else if (currentAttack == 2)
            playerAudio.PlayOneShot(attack2Sound, 0.5f);
        else if (currentAttack == 3)
            playerAudio.PlayOneShot(attack3Sound, 0.5f);

        // Call one of three attack animations "Attack1", "Attack2", "Attack3"
        animator.SetTrigger("Attack" + currentAttack);
        Invoke("StartAttack", 0.08f);
        // Reset timer
        timeSinceAttack = 0.0f;
    }

    private void StartAttack()
    {
        attackHitBox.SetActive(true);
        Invoke("FinishAttack", 0.1f);
    }

    private void FinishAttack()
    {
        attackHitBox.SetActive(false);
    }

    public void OnJump()
    {
        if (!playerGrounded.grounded || !CanAct())
            return;

        playerRb.velocity = new Vector2(playerRb.velocity.x, jumpForce);
        playerAudio.PlayOneShot(jumpSound, 0.09f);
        animator.SetTrigger("Jump");
    }

    public void OnBlock()
    {
        if (!can_block || !CanAct())
            return;
        playerAudio.PlayOneShot(blockSound, 0.5f);
        animator.SetTrigger("Block");
        can_block = false;
        StartCoroutine(InvincibiltyRoutine());
        // StartCoroutine(BlockRoutine());
        // shieldUIAnim.SetFloat("fillAnimMult", 16.0f * Time.deltaTime / block_cooldown);
        shieldUIAnim.SetTrigger("fill");
        //animator.SetBool("IdleBlock", true);
    }

    // Old cooldown for the blocking
    // private IEnumerator BlockRoutine()
    // {
    //     yield return new WaitForSeconds(block_cooldown);
    //     can_block = true;
    // }

    // Invincibility
    public IEnumerator InvincibiltyRoutine()
    {
        isInvincible = true;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, invincibility_fade);
        yield return new WaitForSeconds(invincibilty_duration);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
        yield return new WaitForSeconds(0.2f);
        isInvincible = false;
    }

    public void OnRoll()
    {
        if (!CanAct())
            return;

        playerAudio.PlayOneShot(rollSound, 0.1f);
        rolling = true;
        animator.SetTrigger("Roll");
        shortHitBox.SetActive(true);
        GetComponent<BoxCollider2D>().enabled = false;
        playerRb.velocity = new Vector2(facing_direction * rollForce, playerRb.velocity.y);
    }


    // Handles the end of the roll
    void AE_ResetRoll()
    {
        if (health > 0)
        {
            // Check to see if there is a ceiling above you. If there is, roll again automatically;
            if (playerGrounded.grounded && touching_ceil) {
                playerAudio.PlayOneShot(rollSound, 0.1f);
                animator.SetTrigger("Roll");
                playerRb.velocity = new Vector2(facing_direction * rollForce, playerRb.velocity.y);
            }
            else
            {
                playerCollider.enabled = true;
                shortHitBox.SetActive(false);
                rolling = false;
            }
        }
    }

    public void OnShoot()
    {
        if (ammo > 0 && CanAct())
        {
            animator.SetTrigger("Attack2");
            GameObject projectile = Instantiate(projectilePrefab, new Vector3(transform.position.x + projectileOffsetX * facing_direction, transform.position.y + projectileOffsetY, transform.position.z), transform.rotation);
            projectile.GetComponent<Projectile>().StartShoot(facing_direction);
            ammo--;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        triggerhurtcheck(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        triggerhurtcheck(collision.collider);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        triggerhurtcheck(collision);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        triggerhurtcheck(collision);
    }

    private void triggerhurtcheck(Collider2D collision)
    {
        if ((collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Hazard")) && !isDead)
        {
            if (!isInvincible && (collision.IsTouching(playerCollider) || rolling))
            {
                animator.SetTrigger("Hurt");
                health -= CalcDamage(collision.gameObject);
                playerAudio.PlayOneShot(hurtSound, 0.3f);

                // Bee death (this is terrible ugh)
                if (collision.gameObject.GetComponent<Bee_behavior>() != null)
                {
                    AudioSource.PlayClipAtPoint(collision.gameObject.GetComponent<EnemyCoreScript>().deathSound, collision.gameObject.transform.position, 1f);
                    GameObject explosion = (GameObject)Instantiate(collision.gameObject.GetComponent<Bee_behavior>().explosionRef);
                    explosion.transform.position = new Vector3(collision.gameObject.transform.position.x, collision.gameObject.transform.position.y + 0.3f, collision.gameObject.transform.position.z);
                    Destroy(collision.gameObject);
                }
                else if (collision.gameObject.GetComponent<TD_Bee_Behavior>() != null)
                {
                    AudioSource.PlayClipAtPoint(collision.gameObject.GetComponent<EnemyCoreScript>().deathSound, collision.gameObject.transform.position, 1f);
                    GameObject explosion = (GameObject)Instantiate(collision.gameObject.GetComponent<TD_Bee_Behavior>().explosionRef);
                    explosion.transform.position = new Vector3(collision.gameObject.transform.position.x, collision.gameObject.transform.position.y + 0.3f, collision.gameObject.transform.position.z);
                    Destroy(collision.gameObject);
                }

                if (rolling && touching_ceil && playerGrounded.grounded)
                {
                    facing_direction *= -1;
                    transform.localScale = new Vector3(facing_direction, 1, 1);
                }

                AE_ResetRoll();
                StartCoroutine(InvincibiltyRoutine());
            }
        }
    }

    private int CalcDamage(GameObject obj)
    {
        if (obj.CompareTag("Enemy"))
        {
            if (obj.GetComponent<EnemyCoreScript>() != null)
                return obj.GetComponent<EnemyCoreScript>().damage;
            else
                return obj.GetComponent<WormCoreScript>().damage;
        }
            
        else 
            return 1;
    }

    public void PlayFallSound()
    {
        playerAudio.PlayOneShot(fallSound, 0.1f);
    }

    public void PlayTeleportSound()
    {
        playerAudio.PlayOneShot(teleportSound, 0.1f);
    }
}
