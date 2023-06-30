using System;
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
        objectives = FindObjectsOfType<Objective>();
    }
    private void Update()
    {
        objectives = FindObjectsOfType<Objective>(); //this should only be called after relevant actions (enemy dies, dialogue ends etc.)
        // this script needs to know which level is currently being played
        // showCurrentObjectives(GameManager.Level)
    }

    public void showCurrentObjectives(int level) // looks through all objectives and displays the ones that are not finished and from the current level (or displays nothing if there are none)
    {
        _objectivesTitle.text = "Objectives (Test)";
        string objectivesText = "";
        int counter = 1;
        for (int i = 0; i < objectives.Length; i++)
        {
            if (objectives[i].level == level && !objectives[i].finished)
            {
                objectivesText += counter + ". " + objectives[i].objectiveName[0] + "\n";
                counter++;
            }
        }
        if (objectivesText != "")
        {
            _objectivesText.text = objectivesText;
            animator.SetBool("IsOpen", true);
        }
        else
        {
            animator.SetBool("IsOpen", false);
        }

    }
}
