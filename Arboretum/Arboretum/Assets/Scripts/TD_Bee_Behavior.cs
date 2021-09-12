using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_Bee_Behavior : MonoBehaviour
{
    private Vector2 start;
    private Vector2 center;
    private Vector2 moveTo;

    [SerializeField] float radius;
    private float moveSpeed;
    [SerializeField] float speed = 1.0f;
    private GameObject tree;
    [HideInInspector] public Object explosionRef;

    // Start is called before the first frame update
    void Start()
    {
        start = new Vector2(transform.position.x, transform.position.y);
        center = new Vector2(start.x, start.y);
        moveTo = new Vector2(start.x, start.y);
        moveSpeed = speed;
        explosionRef = Resources.Load("Red Explosion");
        tree = GameObject.Find("Tree");
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        // Go to tree
        center = Vector2.MoveTowards(center, tree.transform.position, speed * Time.deltaTime);
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
        while (x * x + y * y > radius * radius)
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
