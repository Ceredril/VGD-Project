using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
    }

    private void EnableOptionsMenuUI() => gameObject.SetActive(true);
    private void DisableOptionsMenuUI() => gameObject.SetActive(false);
}
