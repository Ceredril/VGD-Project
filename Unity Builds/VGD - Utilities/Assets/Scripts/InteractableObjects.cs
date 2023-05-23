using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Types of interactables:
 * Executes a later specified game event (Starts combat or sth)
 * Displays a Dialogue
 * Changes the players Stats in some way
 */

public enum InteractableTypes { dialogueType, gameEventType, uselessType };
public class InteractableObjects : MonoBehaviour
{
    DialogueManager dialogueManager;
    Transform playerTransform;
    public InteractableTypes CurrentInteractableType;
    public CreateDialogue dialogue;
    public float typingSpeed = 0.02f;
    public float radius = 3f;
    public KeyCode InteractableKey = KeyCode.E; // Changeable
    public float drop = 0;
    bool isInteracting = false;


    private void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        playerTransform = GameObject.Find("Player Body").GetComponent<Transform>();
    }

    void Update()
    {
        float distance = Vector3.Distance(playerTransform.position, this.transform.position);

        // If the player is close enough
        if (Input.GetKeyDown(InteractableKey))
        {
            if (distance <= radius || isInteracting)
            {
                Interact();
            }
        }
    }

    // This method is meant to be overwritten
    public virtual void Interact()
    {
        GameManager.ObjectInteraction();
        switch (CurrentInteractableType)
        {
            case InteractableTypes.dialogueType:
                if (isInteracting)
                {
                    isInteracting = dialogueManager.DisplayNextSentence();
                }
                else
                {
                    dialogueManager.StartDialogue(dialogue, typingSpeed, drop);
                    drop = 0;
                    isInteracting = dialogueManager.DisplayNextSentence();
                }
                break;
            case InteractableTypes.gameEventType:
                break;
            case InteractableTypes.uselessType:
                break;
            default: break;
        }
    }

    // to show a range Wire Sphere in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, radius);
    }
}
