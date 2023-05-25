using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    bool once = true;

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
        slider.maxValue = Enemy._currentHealth;
        fill.color = gradient.Evaluate(1f);
    }

    private void updateCurrentHealthDisplayed()
    {
        slider.value = Enemy._currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
