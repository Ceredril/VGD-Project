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
    public Animator animator;
    DialogueManager dialogueManager;
    Transform playerTransform;
    public InteractableTypes CurrentInteractableType;
    public CreateDialogue dialogue;
    public float typingSpeed = 0.02f;
    public float radius = 3f;
    public KeyCode InteractableKey = KeyCode.E; // Changeable
    public float InteractionStatus = 0;
    bool isInteracting = false;


    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        playerTransform = GameObject.Find("Player Body").GetComponent<Transform>();
        GameManager.OnGameSave += SaveProgress;
        GameManager.OnGameStart += LoadProgress;
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
                animator.SetBool("isTalking", true);
            }
        }
    }

    // This method is meant to be overwritten
    public virtual void Interact()
    {
        GameManager.PlayerInteracted();
        switch (CurrentInteractableType)
        {
            case InteractableTypes.dialogueType:
                if (isInteracting)
                {
                    isInteracting = dialogueManager.DisplayNextSentence();
                }
                else
                {
                    dialogueManager.StartDialogue(dialogue, typingSpeed, InteractionStatus);
                    InteractionStatus = 0;
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

    private void SaveProgress()
    {
        PlayerPrefs.SetFloat("InteractionStatus", InteractionStatus);
    }
    private void LoadProgress()
    {
        if (PlayerPrefs.GetInt("SaveExists") == 1)
        {
            InteractionStatus = PlayerPrefs.GetFloat("InteractionStatus");
        }
    }

}
