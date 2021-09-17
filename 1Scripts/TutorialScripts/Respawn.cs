using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] private Transform respawnPosition;

    void Update()
    {
        if(transform.position.y < respawnPosition.position.y - 15)
        {
            transform.position = respawnPosition.position;
        }
    }
}
