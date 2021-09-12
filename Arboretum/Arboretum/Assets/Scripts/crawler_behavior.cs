using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crawler_behavior : MonoBehaviour
{
    [SerializeField] float speed = 0.6f;
    [SerializeField] GameObject[] waypoints;
    [SerializeField] float turnRange = 0.02f;

    private int nextWaypoint = 1;
    private float distToPoint;
    public bool moving = true;


    // Update is called once per frame
    void Update()
    {
        if (moving)
            Move();
    }

    void Move()
    {
        distToPoint = Vector2.Distance(transform.position, waypoints[nextWaypoint].transform.position);
        transform.position = Vector2.MoveTowards(transform.position, waypoints[nextWaypoint].transform.position,
                                    speed * Time.deltaTime);
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
