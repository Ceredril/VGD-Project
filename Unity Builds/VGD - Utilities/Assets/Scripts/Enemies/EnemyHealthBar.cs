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

    private void Start()
    {
        currentEnemy = GetComponentInParent<Enemy>();
        gameObject.SetActive(false);
        GameManager.OnPlayerAttack += UpdateHealthBar;
    }

    private void updateMaxHealthDisplayed()
    {
        slider.maxValue = currentEnemy._maxHealth;
        fill.color = gradient.Evaluate(1f);
    }

    private void UpdateHealthBar( int a, Enemy b) // two useless parameters
    {
        slider.maxValue = currentEnemy._maxHealth;
        slider.value = currentEnemy._currentHealth;
        fill.color = gradient.Evaluate(1f);
        fill.color = gradient.Evaluate(slider.normalizedValue);
        if(currentEnemy._currentHealth < currentEnemy._maxHealth) gameObject.SetActive(true);
    }
}