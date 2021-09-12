using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crab_behavior_2 : MonoBehaviour
{
    // Normal movement
    [SerializeField] float speed = 1.0f;
    [SerializeField] GameObject[] waypoints = new GameObject[2]; // Left: 0, Right: 1
    [SerializeField] float turnRange = 0.02f;
    [SerializeField] int nextWaypoint = 1;
    private float distToPoint;

    // Fast movement
    [SerializeField] float fastSpeed = 2.0f;
    [SerializeField] float speedAnimMult = 2.5f;
    private float distToPlayer;
    private GameObject player;
    private Animator anim;

    void Start()
    {
        player = GameObject.Find("Player");
        anim = GetComponent<Animator>();
        FaceWaypoint();
    }

    // Update is called once per frame
    void Update()
    {
        distToPoint = Vector2.Distance(transform.position, waypoints[nextWaypoint].transform.position);

        // In between waypoints
        if (player.transform.position.x > waypoints[0].transform.position.x && player.transform.position.x < waypoints[1].transform.position.x)
        {
            anim.SetFloat("walkMult", speedAnimMult);
            transform.position = Vector2.MoveTowards(transform.position, waypoints[nextWaypoint].transform.position, fastSpeed * Time.deltaTime);
        }

        // Not between waypoints
        else
        {
            anim.SetFloat("walkMult", 1.0f);
            transform.position = Vector2.MoveTowards(transform.position, waypoints[nextWaypoint].transform.position, speed * Time.deltaTime);
        }

        if (distToPoint <= turnRange)
        {
            TakeTurn();
        }
    }

    void TakeTurn()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,
                                               transform.localScale.z);
        nextWaypoint = 1 - nextWaypoint;
    }

    void FaceWaypoint()
    {
        // Waypoint is left of player
        if (waypoints[nextWaypoint].transform.position.x - transform.position.x < 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,
                                               transform.localScale.z);
        }
        // Waypoing is right of player
        else if (waypoints[nextWaypoint].transform.position.x - transform.position.x > 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,
                                               transform.localScale.z);
        }
    }
}
