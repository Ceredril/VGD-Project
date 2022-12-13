using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayPlayerStats : MonoBehaviour
{
    private TextMeshProUGUI Lives, Health, Mana, Stamina, StartParameters, PowerupCooldowns, OtherInfos;
    // Start is called before the first frame update
    void Start()
    {
        Lives = GameObject.Find("LivesDisplay").GetComponent<TextMeshProUGUI>();
        Health = GameObject.Find("HealthDisplay").GetComponent<TextMeshProUGUI>();
        Mana = GameObject.Find("ManaDisplay").GetComponent<TextMeshProUGUI>();
        Stamina = GameObject.Find("StaminaDisplay").GetComponent<TextMeshProUGUI>();
        StartParameters = GameObject.Find("StartParametersDisplay").GetComponent<TextMeshProUGUI>();
        PowerupCooldowns = GameObject.Find("PowerupCooldownsDisplay").GetComponent<TextMeshProUGUI>();
        OtherInfos = GameObject.Find("OtherInfosDisplay").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.GameIsOver)gameObject.SetActive(false);
        Lives.text = "Lives:\n" + PlayerStats.CurrentLives.ToString();
        Health.text = "Health:\n" + PlayerStats.CurrentHealth.ToString();
        Mana.text = "Mana:\n" + PlayerStats.CurrentMana.ToString();
        Stamina.text = "Stamina:\n" + PlayerStats.CurrentStamina.ToString();
        StartParameters.text = "Start Parameters:\n" + PlayerStats.StartParams;
        string displayString = "Powerup Cooldowns:\n";
        foreach (KeyValuePair<KeyCode, int> powerupCooldown in PlayerStats.CurrentPowerupCooldown)
        {
            displayString += powerupCooldown.Key.ToString() + ": " + powerupCooldown.Value.ToString() + "\n";
        }
        PowerupCooldowns.text = displayString;
        OtherInfos.text = "Last checkpoint:\n" + spawnManager.Instance.lastCheckpoint.ToString();
    }
}
