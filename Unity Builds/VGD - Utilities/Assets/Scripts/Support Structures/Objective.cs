using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectiveType { Interacting, Killing, Collecting };
[System.Serializable]
public class Objective : MonoBehaviour
{
    public bool finished = false;
    public ObjectiveType objectiveType;
    public Transform connectedCheckpoint;
    [HideInInspector]
    public string objectiveName;

    public void Awake()
    {
        objectiveName = objectiveType + " " + gameObject.name;
    }

    public void Update()
    {
        if (objectiveType == ObjectiveType.Collecting)
            transform.localRotation = Quaternion.Euler(0f, Time.time * 100f, 0);
    }

    public void objectiveFinished()
    {
        if (!finished) 
        {
            GameManager.audioManager.Play("ObjectiveFinished");
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
