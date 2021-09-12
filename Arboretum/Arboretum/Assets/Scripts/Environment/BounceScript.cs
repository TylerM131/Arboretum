using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script for bouncy objects, apply to a collider and set the direction of bounce.
// If multiple faces are visible and bouncy might want to use 4 edge colliders
public class BounceScript : MonoBehaviour
{
    [SerializeField] float bounceForce = 20;
    // Vertical or horizontal axis
    [SerializeField] bool vertical = true;
    // Positive or negative direction (positive = up/right)
    [SerializeField] bool positive = true;

    // When contact is established
    private void OnCollisionEnter2D(Collision2D collision)
    {
        int dir;
        if (positive)
            dir = 1;
        else
            dir = -1;

        if (vertical)
        {
            collision.rigidbody.velocity = new Vector2(collision.rigidbody.velocity.x, 0);
            collision.rigidbody.AddForce(Vector2.up * bounceForce * dir, ForceMode2D.Impulse);
        }
        else
        {
            collision.rigidbody.velocity = new Vector2(0, collision.rigidbody.velocity.x);
            collision.rigidbody.AddForce(Vector2.right * bounceForce * dir, ForceMode2D.Impulse);

        }
    }
}
