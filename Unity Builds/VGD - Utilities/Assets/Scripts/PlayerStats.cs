using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // all currently active player stats
    //[SerializeField] public static Transform LastCheckpoint;

    public static int CurrentLives;
    public static int CurrentHealth;
    public static int CurrentMana;
    public static int CurrentStamina;
    public static bool GameIsOver = true;
    public static string StartParams = playerStatsManager.MaxLives + " mL\n" + playerStatsManager.MaxHealth + " mH\n" + playerStatsManager.MaxMana + " mM\n" + playerStatsManager.MaxStamina + " mS";
}
