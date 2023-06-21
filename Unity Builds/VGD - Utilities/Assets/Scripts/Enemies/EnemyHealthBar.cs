using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    private Enemy _currentEnemy;
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    private void Start()
    {
        _currentEnemy = GetComponentInParent<Enemy>();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateHealthBar();
    }

    private void updateMaxHealthDisplayed()
    {
        slider.maxValue = _currentEnemy._maxHealth;
        fill.color = gradient.Evaluate(1f);
    }

    private void UpdateHealthBar() // two useless parameters
    {
        slider.maxValue = _currentEnemy._maxHealth;
        slider.value = _currentEnemy._currentHealth;
        fill.color = gradient.Evaluate(1f);
        fill.color = gradient.Evaluate(slider.normalizedValue);
        if(_currentEnemy._currentHealth < _currentEnemy._maxHealth) gameObject.SetActive(true);
    }
}