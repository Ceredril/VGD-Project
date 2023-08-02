using System;
using UnityEngine;

/*
 * Types of interactables:
 * Executes a later specified game event (Starts combat or sth)
 * Displays a Dialogue
 * Changes the players Stats in some way
 */

public enum InteractableType { dialogueType, gameEventType, uselessType };
public class InteractableObjects : MonoBehaviour
{
    public Animator animator;
    DialogueManager dialogueManager;
    Transform playerTransform;
    public InteractableType interactableType;
    public CreateDialogue dialogue;
    public float typingSpeed = 0.02f;
    public float radius = 3f;
    public KeyCode interactableKey = KeyCode.E; // Changeable
    public float interactionStatus = 0;
    bool isInteracting = false;


    private void Awake()
    {
        GameManager.OnGameStart += LoadProgress;
        GameManager.OnGameSave += SaveProgress;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStart -= LoadProgress;
        GameManager.OnGameSave -= SaveProgress;

    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        playerTransform = GameObject.Find("Player Body").GetComponent<Transform>();

    }

    void Update()
    {
        float distance = Vector3.Distance(playerTransform.position, this.transform.position);

        // If the player is close enough
        if (Input.GetKeyDown(interactableKey))
        {
            if (distance <= radius || isInteracting)
            {
                Interact();
                animator.SetBool("isTalking", true);
            }
            if(!isInteracting)
                animator.SetBool("isTalking", false);
        }
    }

    // This method is meant to be overwritten
    public virtual void Interact()
    {
        GameManager.PlayerInteracted(this.gameObject);
        switch (interactableType)
        {
            case InteractableType.dialogueType:
                if (isInteracting)
                {
                    isInteracting = dialogueManager.DisplayNextSentence();
                }
                else
                {
                    dialogueManager.StartDialogue(dialogue, typingSpeed, interactionStatus);
                    interactionStatus = 0;
                    isInteracting = dialogueManager.DisplayNextSentence();
                }
                break;
            case InteractableType.gameEventType:
                break;
            case InteractableType.uselessType:
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
    
    private void SaveProgress()
    {
        PlayerPrefs.SetFloat(dialogue.name, interactionStatus);
        PlayerPrefs.Save();
    }
    private void LoadProgress()
    {
        if (PlayerPrefs.GetInt("SaveExists") == 1) interactionStatus = PlayerPrefs.GetFloat(dialogue.name);
        else
        {
            interactionStatus = 0;
            SaveProgress();
        }
    }
}
