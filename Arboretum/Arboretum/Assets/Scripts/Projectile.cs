using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] int damage;

    private Camera cam;

    public void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    public void Update()
    {
        Vector3 viewPosition = cam.WorldToViewportPoint(gameObject.transform.position);
        if (viewPosition.x < 0 || viewPosition.x > 1)
            Destroy(gameObject);
    }

    public void StartShoot(int facing_direction)
    {
        transform.localScale = new Vector3(transform.localScale.x * facing_direction, transform.localScale.y, transform.localScale.z);
        GetComponent<Rigidbody2D>().velocity = new Vector2(speed * facing_direction, 0);
    }
}
