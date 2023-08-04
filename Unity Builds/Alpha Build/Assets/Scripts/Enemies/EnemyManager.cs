using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private static List<Enemy> enemies = new List<Enemy>();

    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
        else
            Destroy(gameObject);

        GameManager.OnGameStart += LoadEnemies;
        GameManager.OnGameSave += SaveEnemies;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStart -= LoadEnemies;
        GameManager.OnGameSave -= SaveEnemies;
    }

    public void RegisterEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
    }
    
    public static void SaveEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            PlayerPrefs.SetInt(enemy.name + "_currentHealth", enemy.currentHealth);
            PlayerPrefs.SetInt(enemy.name + "_isAlive", enemy.isAlive ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    public static void LoadEnemies()
    {
        
        if (PlayerPrefs.GetInt("SaveExists") == 1)
        {
            Debug.Log("Loading saved enemies");
            foreach (Enemy enemy in enemies)
            {
                if (enemy != null){
                    enemy.currentHealth = PlayerPrefs.GetInt(enemy.name + "_currentHealth");
                    enemy.isAlive = PlayerPrefs.GetInt(enemy.name + "_isAlive") == 1;
                    //if(!enemy.isAlive) ANIMATOR - DEATH   
                }
            }
        }
        else
        {
            Debug.Log("Loading new enemies");
            foreach (Enemy enemy in enemies)
            {
                enemy.currentHealth = enemy.maxHealth;
                enemy.isAlive = true;
            }
        }
    }
}