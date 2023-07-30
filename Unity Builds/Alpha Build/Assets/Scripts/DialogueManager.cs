using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{


    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Animator animator;
    float _typingSpeed;
    float _drop;

    private Queue<string> sentences;

    // Use this for initialization
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(CreateDialogue dialogue, float typingspeed, float drop)
    {
        _typingSpeed= typingspeed;
        _drop = drop;
        Debug.Log("Starting conversation with: " + dialogue.name);
        animator.SetBool("IsOpen", true);

        nameText.text = dialogue.name;

        sentences.Clear();

        if (!dialogue.interacted)
        {
            foreach (string sentence in dialogue.sentences)
            {
                sentences.Enqueue(sentence);
            }
            dialogue.interacted = true;
        }
        else
        {
            foreach (string sentence in dialogue.repeatedSentences)
            {
                sentences.Enqueue(sentence);
            }
        }
    }

    public bool DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return false;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
        return true;
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(_typingSpeed);
        }
    }

    void EndDialogue()
    {
        switch (_drop)
        {
            case 0:
                break;
            case 1:
                PlayerManager.AddLives(1);
                break;
            case 2:
                PlayerManager.AddHealth(20);
                break;
            case 3:
                // enable Fist attack
                break;
        }
        animator.SetBool("IsOpen", false);
    }
}
