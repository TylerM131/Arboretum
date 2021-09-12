using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueBoxSpawn : MonoBehaviour
{
    private GameObject extendedTongue;

    private void Start()
    {
        extendedTongue = transform.Find("ExtendedTongue").gameObject;

    }

    void SpawnSecondTongueCollider()
    {
        extendedTongue.SetActive(true);
    }

    void DespawnSecondTongueCollider()
    {
        extendedTongue.SetActive(false);
    }
}
