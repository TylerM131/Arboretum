using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crab_behavior : MonoBehaviour
{
    // Normal movement
    [SerializeField] float speed = 1.0f;
    [SerializeField] GameObject[] waypoints;
    [SerializeField] float turnRange = 0.02f;
    private int nextWaypoint = 1;
    private float distToPoint;

    // Fast movement
    [SerializeField] float fastSpeed = 2.0f;
    [SerializeField] float speedAnimMult = 2.5f;
    [SerializeField] float runRange = 7.0f;
    private float distToPlayer;
    private GameObject player;
    private Animator anim;

    void Start()
    {
        player = GameObject.Find("Player");
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        distToPoint = Vector2.Distance(transform.position, waypoints[nextWaypoint].transform.position);
        distToPlayer = Vector2.Distance(transform.position, player.transform.position);

        // In runRange
        if (distToPlayer < runRange)
        {
            anim.SetFloat("walkMult", speedAnimMult);
            transform.position = Vector2.MoveTowards(transform.position, waypoints[nextWaypoint].transform.position, fastSpeed * Time.deltaTime);
        }

        // Out of runRange
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
        Vector3 currRot = transform.eulerAngles;
        currRot.x += waypoints[nextWaypoint].transform.eulerAngles.x;
        currRot.y += waypoints[nextWaypoint].transform.eulerAngles.y;
        currRot.z += waypoints[nextWaypoint].transform.eulerAngles.z;
        transform.eulerAngles = currRot;
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
}
