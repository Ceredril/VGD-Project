using System;
using UnityEngine;

/*
 * To be applied to checkpoint objects.
 */
public class checkPoint : MonoBehaviour
{
    private void Awake() => enabled = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //This modifies the spawnPoint.
            GameManager.CheckpointReached(transform);
            //This disables the checkPoint after it's been reached.
            GetComponent<Collider>().enabled = false;
            Debug.Log("Checkpoint reached! " + name);
        }
    }
}
