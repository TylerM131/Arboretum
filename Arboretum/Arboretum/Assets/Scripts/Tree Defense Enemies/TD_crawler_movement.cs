using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_crawler_movement : MonoBehaviour
{
    [SerializeField] float speed = 0.6f;
    private GameObject tree;

    private void Start()
    {
        tree = GameObject.Find("Tree");
        if (tree != null)
            FaceTree();
    }

    // Update is called once per frame
    void Update()
    {
        if (tree != null)
            transform.position = Vector2.MoveTowards(transform.position, tree.transform.position, speed * Time.deltaTime);
    }

    void FaceTree()
    {
        // Face left
        if (tree.transform.position.x - transform.position.x < 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,
                                               transform.localScale.z);
        }
        // Face right
        else if (tree.transform.position.x - transform.position.x > 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,
                                               transform.localScale.z);
        }
    }

}
