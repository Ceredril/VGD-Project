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

        GameManager.OnGameSave += SaveEnemies;
        GameManager.OnGameLoad += LoadEnemies;
        GameManager.OnGameNew += NewEnemies;
    }

    private void OnDestroy()
    {
        GameManager.OnGameSave -= SaveEnemies;
        GameManager.OnGameLoad -= LoadEnemies;
        GameManager.OnGameNew -= NewEnemies;
    }

    public void RegisterEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    public static void NewEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            enemy.currentHealth = enemy.maxHealth;
        }
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
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null){
                int currentHealth = PlayerPrefs.GetInt(enemy.name + "_currentHealth");
                bool isAlive = PlayerPrefs.GetInt(enemy.name + "_isAlive") == 1;
                enemy.currentHealth = currentHealth;
                enemy.isAlive = isAlive;
                if (!isAlive) enemy.gameObject.SetActive(false);
                else enemy.gameObject.SetActive(true);
            }
        }
    }
}