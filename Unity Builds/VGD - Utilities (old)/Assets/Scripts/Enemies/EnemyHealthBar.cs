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
        slider = GetComponent<Slider>();
        fill = GetComponentInChildren<Image>();
        gameObject.SetActive(false);
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (_currentEnemy.currentHealth < _currentEnemy.maxHealth) gameObject.SetActive(true);
        slider.maxValue = _currentEnemy.maxHealth;
        slider.value = _currentEnemy.currentHealth;
        fill.color = gradient.Evaluate(1f);
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}