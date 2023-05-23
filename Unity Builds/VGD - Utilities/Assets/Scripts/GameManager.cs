using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    //GAME EVENTS
    public static event Action OnGameStart,OnGameRestart,OnGameOver,OnGameEnd,OnGamePause,OnGameResume,OnGameSave;
    public static event Action OnPlayerDeath;
    
    //COLLECTIBLE EVENTS
    public static event Action<int> OnManaCollected, OnHealthCollected, OnLivesCollected;
    //CHECKPOINT EVENTS
    public static event Action<Transform> OnCheckpointReached;
    public static event Action OnObjectInteraction;
    public static event Action<KeyCode> OnAbilityButtonPressed;
    public static event Action<int> OnPlayerAttackedMelee, OnPlayerAttackedRanged;

    public static bool GameIsRunning;
    public static bool GameIsPaused;
    public static bool GameIsOver;

    public static bool PlayerIsAlive;
    
    
    //DEFAULT FUNCTIONS
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameStart();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameIsOver && !GameIsPaused && GameIsRunning) Pause();
        /*foreach (KeyValuePair<KeyCode, Cooldown> ability in AbilityManager.PlayerAbilities) // I WOULD MAYBE TRY TO INTEGRATE ALL KEYS INTO THIS
        {
            // Check if the player tries to execute an ability
            if (Input.GetKeyDown(ability.Key))
            {
                AbilityButtonPressed(ability.Key);
            }
        }*/
    }


    //EVENT RELATED FUNCTIONS
    public static void GameStart()
    {
        Debug.Log("Game started");
        GameIsOver = false;
        GameIsRunning = true;
        PlayerIsAlive = true;
        OnGameStart?.Invoke();
    }
    
    public static void GameRestart()
    {
        OnGameRestart?.Invoke();
        GameIsRunning = true;
        PlayerIsAlive = true;
        Debug.Log("Game restarted");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public static void GameOver()
    {
        Debug.Log("Game lost");
        GameIsOver = true;
        GameIsRunning = false;
        OnGameOver?.Invoke();
    }
    
    public static void GameEnd()
    {
        GameIsRunning = false;
        Debug.Log("Game ended");
        OnGameEnd?.Invoke();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    
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

    public static void GameSave()
    {
        Debug.Log("Game saved");
        OnGameSave?.Invoke();
    }

    public static void ManaCollected(int amount) => OnManaCollected?.Invoke(amount);
    public static void HealthCollected(int amount) => OnHealthCollected?.Invoke(amount);
    public static void LivesCollected(int amount) => OnLivesCollected?.Invoke(amount);
    public static void CheckpointReached(Transform checkpoint) => OnCheckpointReached?.Invoke(checkpoint);

    public static void PlayerDeath()
    {
        Debug.Log("Player died");
        GameIsRunning = false;
        PlayerIsAlive = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        OnPlayerDeath?.Invoke();
    }

    public static void MeleeEnemyAttacks(int damage)
    {
        Debug.Log("Player took " + damage + " melee damage");
        OnPlayerAttackedMelee?.Invoke(damage);
    }

    public static void RangedEnemyAttacks(int damage)
    {
        Debug.Log("Player took " + damage + " ranged damage");
        OnPlayerAttackedRanged?.Invoke(damage);
    }

    public static void PlayerInteracted()
    {
        Debug.Log("Interacting with an Object");
        OnObjectInteraction?.Invoke();
    }

    public static void PlayerPressedAbilityButton(KeyCode keyBind)
    {
        Debug.Log("Ability Button: " + keyBind + " pressed");
        OnAbilityButtonPressed?.Invoke(keyBind);
    }
}
