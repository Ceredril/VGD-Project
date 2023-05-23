using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static event Action OnPlayerSpawn, OnPlayerDeath;
    public static event Action<int> OnManaCollected, OnHealthCollected, OnLivesCollected;
    public static event Action<Transform> OnCheckpointReached;
    public static event Action OnGameStart, OnGamePause, OnGameSave, OnGameRestart, OnGameResume, OnGameEnd, OnGameOver;
    public static event Action OnObjectInteraction;
    public static event Action<KeyCode> OnAbilityButtonPressed;


    public static event Action<int> OnPlayerAttackedMelee, OnPlayerAttackedRanged;

    public static bool GameIsRunning;
    public static bool GameIsPaused;
    public static bool GameIsOver;
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameStart();
        OnGameEnd += MoveToMainMenu;
    }

    private void OnDestroy() => OnGameEnd -= MoveToMainMenu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameIsOver && !GameIsPaused) Pause();
        if (Input.GetKeyDown(KeyCode.T)) PlayerSpawn();
        foreach (KeyValuePair<KeyCode, Cooldown> ability in AbilityManager.PlayerAbilities) // I WOULD MAYBE TRY TO INTEGRATE ALL KEYS INTO THIS
        {
            // Check if the player tries to execute an ability
            if (Input.GetKeyDown(ability.Key))
            {
                AbilityButtonPressed(ability.Key);
            }
        }
    }

    private void MoveToMainMenu() => SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    
    public static void Pause()
    {
        OnGamePause?.Invoke();
        GameIsPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Game paused");
    }
    public static void Resume()
    {
        OnGameResume?.Invoke();
        GameIsPaused = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static void ManaCollected(int amount) => OnManaCollected?.Invoke(amount);
    public static void HealthCollected(int amount) => OnHealthCollected?.Invoke(amount);
    public static void LivesCollected(int amount) => OnLivesCollected?.Invoke(amount);
    public static void CheckpointReached(Transform checkpoint) => OnCheckpointReached?.Invoke(checkpoint);

    public static void PlayerSpawn() { Debug.Log("Player spawned"); OnPlayerSpawn?.Invoke(); }

    public static void PlayerDeath()
    {
        Debug.Log("Player died");
        GameIsRunning = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        OnPlayerDeath?.Invoke();
    }

    public static void GameStart()
    {
        Debug.Log("Game started");
        GameIsOver = false;
        GameIsRunning = true;
        OnGameStart?.Invoke();
    }

    public static void GameRestart()
    {
        OnGameRestart?.Invoke();
        GameIsRunning = true;
        Debug.Log("Game restarted");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public static void GameSave() { Debug.Log("Game saved"); OnGameSave?.Invoke(); }
    public static void GameResume() { Debug.Log("Game resumed"); OnGameResume?.Invoke(); }

    public static void GameOver()
    {
        Debug.Log("Game lost");
        GameIsOver = true;
        
        OnGameOver?.Invoke();
    }
    public static void GameEnd() { Debug.Log("Game ended"); OnGameEnd?.Invoke(); }

    public static void PlayerAttackedMelee(int damage) { Debug.Log("Player took " + damage + " melee damage"); OnPlayerAttackedMelee?.Invoke(damage); }
    public static void PlayerAttackedRanged(int damage) { Debug.Log("Player took " + damage + " ranged damage"); OnPlayerAttackedRanged?.Invoke(damage); }
    public static void ObjectInteraction() { Debug.Log("Interacting with an Object"); OnObjectInteraction?.Invoke(); }

    public static void AbilityButtonPressed(KeyCode keyBind) { Debug.Log("Ability Button: " + keyBind + " pressed"); OnAbilityButtonPressed?.Invoke(keyBind); }
}
