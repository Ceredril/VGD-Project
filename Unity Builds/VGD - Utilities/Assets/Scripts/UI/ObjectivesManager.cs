using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ObjectivesManager : MonoBehaviour
{
    public Animator animator;
    public Objective[] objectives;
    public TextMeshProUGUI _objectivesTitle;
    public TextMeshProUGUI _objectivesText;

    private void Start()
    {
        animator = GetComponent<Animator>();
        _objectivesTitle = GameObject.Find("ObjectivesTitle").GetComponent<TextMeshProUGUI>();
        _objectivesText = GameObject.Find("ObjectivesText").GetComponent<TextMeshProUGUI>();
    }

    // this script needs a way to identify the currently left over objectives in the level. A possible way to implement might be to attach a component to GameObjects that are connected to the Objectives (enemies, npcs etc)
    private void Update()
    {
        /* objectives = Object.FindObjectsOfType<Objective>();
         * 
         */
        objectives = FindObjectsOfType<Objective>();
        if (objectives.Length > 0)
        {
            showObjectives();
        }
        //else  hideObjectives();
    }


    public void showObjectives()
    {
        _objectivesTitle.text = "Test";
        string objectivesText = "";
        for (int i = 0; i < objectives.Length; i++)
        {
            objectivesText += i + ". " + objectives[i].ObjectiveDescription[0] + "\n";
        }
        _objectivesText.text = objectivesText;
        animator.SetBool("IsOpen", true);

    }
    public void hideObjectives()
    {
        animator.SetBool("IsOpen", false);
    }
}
