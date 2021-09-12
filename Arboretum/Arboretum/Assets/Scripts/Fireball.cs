using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;

    private Camera cam;

    private AudioSource audioPlayer;
    public AudioClip explosionSound;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        audioPlayer = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Destroy fireball if offscreen
        Vector3 viewPosition = cam.WorldToViewportPoint(gameObject.transform.position);
        if (viewPosition.x < 0 || viewPosition.x > 1)
            Destroy(gameObject);

        // If scale > 0 move right, else move left
        float scaleSign = Mathf.Abs(transform.localScale.x) / transform.localScale.x;
        transform.Translate(Vector3.right * scaleSign * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Attack"))
        {
            speed = 0;
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
