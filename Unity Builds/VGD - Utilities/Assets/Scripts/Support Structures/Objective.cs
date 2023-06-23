using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Objective : MonoBehaviour
{
    public string ObjectiveType;

    [TextArea(3, 10)]
    public string[] ObjectiveDescription;
}
