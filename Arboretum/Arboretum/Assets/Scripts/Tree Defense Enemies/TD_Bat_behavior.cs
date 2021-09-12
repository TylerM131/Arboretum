using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_Bat_behavior : MonoBehaviour
{
    private GameObject player;
    [SerializeField] float speed = 4.0f;

    private GameObject tree;
    private float distanceToTree;
    [SerializeField] float distanceMoved;
    [SerializeField] float distanceToChangeTarget = 1;
    private Vector3 target;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        
        tree = GameObject.Find("Tree");
        distanceMoved = 0;
        if (tree != null)
        {
            target = tree.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tree != null)
        {
            distanceToTree = Vector3.Distance(transform.position, new Vector3(tree.transform.position.x,
                tree.transform.position.y - 3.0f, tree.transform.position.z));

            if (distanceToTree > 5.0f)
            {
                Vector3 prevPos = transform.position;

                // Move towards tree
                FaceTree();
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(target.x, target.y), speed * Time.deltaTime);

                distanceMoved += Vector3.Distance(transform.position, prevPos);

                if (distanceMoved >= distanceToChangeTarget)
                {
                    distanceMoved = 0;
                    target.y = tree.transform.position.y - 3.0f + ((float)(Random.Range(0, 30)) / 10);
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(tree.transform.position.x, tree.transform.position.y), speed * Time.deltaTime);
            }
        }
    }

    void FaceTree()
    {
        if (tree.transform.position.x - transform.position.x < 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,
                                               transform.localScale.z);
        }
        else if (tree.transform.position.x - transform.position.x > 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,
                                               transform.localScale.z);
        }
    }
}
