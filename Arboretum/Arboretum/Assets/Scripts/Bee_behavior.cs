using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee_behavior : MonoBehaviour
{
    private Vector2 start;
    private Vector2 center;
    private Vector2 moveTo;

    [SerializeField] float radius;
    private float moveSpeed;
    [SerializeField] float speed = 1.0f;
    [SerializeField] float attackSpeed = 3.0f;
    [SerializeField] float range = 12.0f;
    private GameObject player;
    private float headOffset;
    private Vector2 playerHead;

    [HideInInspector] public Object explosionRef;

    // Start is called before the first frame update
    void Start()
    {
        start = new Vector2(transform.position.x, transform.position.y);
        center = new Vector2(start.x, start.y);
        moveTo = new Vector2(start.x, start.y);
        player = GameObject.Find("Player");
        moveSpeed = speed;
        headOffset = player.GetComponent<BoxCollider2D>().size.y;
        explosionRef = Resources.Load("Red Explosion");
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        // Follow player
        if (distanceToPlayer < range)
        {
            playerHead.x = player.transform.position.x;
            playerHead.y = player.transform.position.y + headOffset;
            center = Vector2.MoveTowards(center, playerHead, attackSpeed * Time.deltaTime);
            moveSpeed = attackSpeed;

        }

        // Return to start
        else if (center.x != start.x && center.y != start.y)
        {
            center = Vector2.MoveTowards(center, start, speed * Time.deltaTime);
            moveSpeed = speed;
        }

    }

    private void Move()
    {
        if (transform.position.x == moveTo.x && transform.position.y == moveTo.y)
        {
            moveTo = RandPos();
        }

        FaceMoveTo();
        transform.position = Vector2.MoveTowards(transform.position, moveTo, moveSpeed * Time.deltaTime);
    }

    // Generate random transform in circle around start
    private Vector2 RandPos()
    {
        // Generate random coords in square
        float x = Random.Range(-radius, radius);
        float y = Random.Range(-radius, radius);
        // If coords are outside circle, generate new ones
        while (x*x + y*y > radius * radius)
        {
            x = Random.Range(-radius, radius);
            y = Random.Range(-radius, radius);
        }

        return new Vector2(center.x + x, center.y + y);
    }

    void FaceMoveTo()
    {
        if (moveTo.x - transform.position.x < 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,
                                               transform.localScale.z);
        }
        else if (moveTo.x - transform.position.x > 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,
                                               transform.localScale.z);
        }
    }
}
