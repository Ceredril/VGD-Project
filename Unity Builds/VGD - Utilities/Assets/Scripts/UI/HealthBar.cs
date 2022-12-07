using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    private void Update()
    {
        updateShownMaxHealthDisplayed();
        updateCurrentHealthDisplayed();
    }

    public void updateShownMaxHealthDisplayed()
    {
        slider.maxValue = playerStatsManager.MaxHealth;
        fill.color = gradient.Evaluate(1f);
    }

    public void updateCurrentHealthDisplayed()
    {
        slider.value = PlayerStats.CurrentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
