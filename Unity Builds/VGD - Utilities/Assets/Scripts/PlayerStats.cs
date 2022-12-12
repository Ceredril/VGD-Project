using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // Player stats with initial value
    //[SerializeField] public static Transform LastCheckpoint;

    private static Dictionary<KeyCode, int> _maxPowerupCooldown =
        new Dictionary<KeyCode, int>() { // declare all Cooldowns here
            { KeyCode.E, 3 },
            { KeyCode.Q, 10 },
            { KeyCode.K, 15 },
        };

    public static int CurrentLives;
    public static int CurrentHealth;
    public static int CurrentMana;
    public static int CurrentStamina;
    public static bool GameIsOver=true;
    public static Dictionary<KeyCode, int> CurrentPowerupCooldown = new Dictionary<KeyCode, int>();

    public static string StartParams = playerStatsManager.MaxLives + " mL\n" + playerStatsManager.MaxHealth + " mH\n" + playerStatsManager.MaxMana + " mM\n" + playerStatsManager.MaxStamina + " mS";

    // Start is called before the first frame update
    private void Start()
    {
        foreach (KeyValuePair<KeyCode, int> powerupCooldown in PlayerStats._maxPowerupCooldown) // initializes all cooldowns as 0
        {
            CurrentPowerupCooldown[powerupCooldown.Key] = 0;
        }
    }
}
