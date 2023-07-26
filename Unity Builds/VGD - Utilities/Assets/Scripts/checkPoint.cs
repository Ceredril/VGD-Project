using System;
using UnityEngine;

/*
 * To be applied to checkpoint objects.
 */
public class checkPoint : MonoBehaviour
{
    AudioManager audioManager;
    public ObjectivesManager objectivesManager;
    private void Start()
    {
        objectivesManager = FindObjectOfType<ObjectivesManager>();
        audioManager = FindObjectOfType<AudioManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBody") && objectivesManager.objectivesCompleted)
        {
            audioManager.Play("Checkpoint");
            //This modifies the spawnPoint.
            GameManager.CheckpointReached(transform);
            GameManager.GameSave();
            //This disables the checkPoint after it's been reached.
            GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<Collider>().isTrigger = false;
            Debug.Log("Checkpoint reached! " + name);
        }
    }
}
