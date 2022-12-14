using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Types of interactables:
 * Executes a later specified game event (Starts combat or sth)
 * Displays a DialogueType
 * Changes the players Stats in some way
 */

public enum InteractableTypes { DialogueType, gameEventType, uselessType };
public class InteractableObjects : MonoBehaviour
{
    public InteractableTypes CurrentInteractableType;
    public CreateDialogue dialogue;
    public float radius = 3f;
    public Transform interactionTransform;
    public KeyCode InteractableKey = KeyCode.E; // Changeable

    public Transform player;       // Reference to the player transform

    void Update()
    {
        float distance = Vector3.Distance(player.position, interactionTransform.position);

        // If the player is close enough
        if (distance <= radius)
        {
            if (Input.GetKeyDown(InteractableKey))
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
            case InteractableTypes.DialogueType:
                FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
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
        Gizmos.DrawWireSphere(interactionTransform.position, radius);
    }
}
