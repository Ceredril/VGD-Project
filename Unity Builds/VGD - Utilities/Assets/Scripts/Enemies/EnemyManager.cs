using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private List<Enemy> enemies = new List<Enemy>();

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

    public void NewEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            switch (enemy.enemyType)
            {
                case Enemy.EnemyType.Melee:
                    enemy._sightRange = 10f;
                    enemy._walkPointRange = 6f;
                    enemy._attackRange = 2f;
                    enemy._attackCooldown = 4f;
                    enemy._maxHealth = 60;
                    enemy._currentHealth = enemy._maxHealth;
                    break;
                case Enemy.EnemyType.Ranged:
                    enemy._sightRange = 18f;
                    enemy._walkPointRange = 6f;
                    enemy._attackRange = 12f;
                    enemy._attackCooldown = 3f;
                    enemy._maxHealth = 90;
                    enemy._currentHealth = enemy._maxHealth;
                    break;
                case Enemy.EnemyType.Boss:
                    enemy._sightRange = 18f;
                    enemy._walkPointRange = 6f;
                    enemy._attackRange = 12f;
                    enemy._attackCooldown = 3f;
                    enemy._maxHealth = 500;
                    enemy._currentHealth = enemy._maxHealth;
                    break;
            }

            enemy._isAlive = true;
        }
    }
    public void SaveEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            PlayerPrefs.SetInt(enemy.name + "_currentHealth", enemy.GetCurrentHealth());
            PlayerPrefs.SetInt(enemy.name + "_isAlive", enemy.IsAlive() ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    public void LoadEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            switch (enemy.enemyType)
            {
                case Enemy.EnemyType.Melee:
                    enemy._sightRange = 10f;
                    enemy._walkPointRange = 6f;
                    enemy._attackRange = 2f;
                    enemy._attackCooldown = 4f;
                    enemy._maxHealth = 60;
                    break;
                case Enemy.EnemyType.Ranged:
                    enemy._sightRange = 18f;
                    enemy._walkPointRange = 6f;
                    enemy._attackRange = 12f;
                    enemy._attackCooldown = 3f;
                    enemy._maxHealth = 90;
                    break;
                case Enemy.EnemyType.Boss:
                    enemy._sightRange = 18f;
                    enemy._walkPointRange = 6f;
                    enemy._attackRange = 12f;
                    enemy._attackCooldown = 3f;
                    enemy._maxHealth = 500;
                    break;
            }

            int currentHealth = PlayerPrefs.GetInt(enemy.name + "_currentHealth");
            bool isAlive = PlayerPrefs.GetInt(enemy.name + "_isAlive") == 1;
            enemy.SetCurrentHealth(currentHealth);
            enemy.SetIsAlive(isAlive);
            if(!isAlive)enemy.gameObject.SetActive(false);
        }
    }
}