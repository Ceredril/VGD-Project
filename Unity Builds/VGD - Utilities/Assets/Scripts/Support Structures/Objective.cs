using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectiveType { Interacting, Killing, Collecting };
[System.Serializable]
public class Objective : MonoBehaviour
{
    AudioManager audioManager;
    public bool finished = false;
    public ObjectiveType objectiveType;
    public Transform connectedCheckpoint;
    public string objectiveName;

    public void Start()
    {
        objectiveName = objectiveType + " " + gameObject.name;
        audioManager = FindObjectOfType<AudioManager>();
    }
    public void objectiveFinished()
    {
        if (!finished) 
        {
            audioManager.Play("ObjectiveFinished");
            finished = true;
        }
    }
    private void SaveNew()
    {
        finished = false;
        SaveProgress();
    }
    private void SaveProgress()
    {
        PlayerPrefs.SetInt(objectiveName, finished ? 1 : 0);
        PlayerPrefs.Save();
    }
    private void LoadProgress()
    {
        finished = PlayerPrefs.GetInt(objectiveName) == 1;
    }
}
