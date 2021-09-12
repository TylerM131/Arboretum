using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalFireball : MonoBehaviour
{
    private Vector2 start;
    private Rigidbody2D rb;
    private AudioSource audioPlayer;
    public AudioClip explosionSound;
    
    // Start is called before the first frame update
    void Start()
    {
        start.x = transform.position.x;
        start.y = transform.position.y;

        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(0, 750));
        audioPlayer = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((rb.velocity.y > 0 && transform.localScale.x < 0) || (rb.velocity.y < 0 && transform.localScale.x > 0))
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);

        float distanceToStart = transform.position.y - start.y;
        if (distanceToStart < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionY;
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, 1f);
            GetComponent<Animator>().SetTrigger("explode");
        }
    }

    public void MakeInactive()
    {
        gameObject.tag = "InactiveHazard";
    }

    public void DestroyFireBall()
    {
        Destroy(gameObject);
    }
}
