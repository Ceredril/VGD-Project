using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    private Enemy currentEnemy;
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    bool once = true;

    private void Start()
    {
        currentEnemy = GetComponentInParent<Enemy>();
    }
    private void Update()
    {
        if (once)
        {
            updateShownMaxHealthDisplayed(); // this has to work different somehow
            once = false;
        }
        updateCurrentHealthDisplayed();
    }

    private void updateShownMaxHealthDisplayed()
    {
        slider.maxValue = currentEnemy._currentHealth;
        fill.color = gradient.Evaluate(1f);
    }

    private void updateCurrentHealthDisplayed()
    {
        slider.value = currentEnemy._currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
