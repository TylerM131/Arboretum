using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    GameObject player;

    [SerializeField]
    float timeOffset;

    [SerializeField]
    Vector2 posOffset = new Vector2(2,4.7f);

    private Vector3 velocity;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        // Camera's current position
        Vector3 startPos = transform.position;

        // Player's current position
        Vector3 endPos = player.transform.position;
        endPos.x += posOffset.x;
        endPos.y += posOffset.y;
        endPos.z = transform.position.z;

        // lerp
        //transform.position = Vector3.Lerp(startPos, endPos, timeOffset * Time.deltaTime);

        // smoothdamp
        transform.position = Vector3.SmoothDamp(startPos, endPos, ref velocity, timeOffset);
    }
}
