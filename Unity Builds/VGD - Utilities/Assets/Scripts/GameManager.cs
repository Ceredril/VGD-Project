using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static event Action OnPlayerSpawn, OnPlayerDeath;
    public static event Action<int> OnManaCollected, OnHealthCollected, OnLivesCollected;
    public static event Action<Transform> OnCheckpointReached;
    public static event Action OnGameStart, OnGamePause, OnGameSave, OnGameResume, OnGameEnd, OnGameOver;

    public static event Action<int> OnPlayerAttackedMelee;

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
        if(Input.GetKeyDown(KeyCode.Escape) && !GameIsOver)GamePause();
        if(Input.GetKeyDown(KeyCode.T))PlayerSpawn();
    }

private void MoveToMainMenu() => SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);


public static void ManaCollected(int amount) => OnManaCollected?.Invoke(amount);
    public static void HealthCollected(int amount) => OnHealthCollected?.Invoke(amount);
    public static void LivesCollected(int amount) => OnLivesCollected?.Invoke(amount);
    public static void CheckpointReached(Transform checkpoint) => OnCheckpointReached?.Invoke(checkpoint);
    
    public static void PlayerSpawn(){Debug.Log("Player spawned");OnPlayerSpawn?.Invoke();}
    public static void PlayerDeath(){Debug.Log("Player died"); OnPlayerDeath?.Invoke();}

    public static void GameStart()
    {
        Debug.Log("Game started");
        GameIsOver = false;
        OnGameStart?.Invoke();
    }
    public static void GamePause(){Debug.Log("Game paused");OnGamePause?.Invoke();}
    public static void GameSave(){Debug.Log("Game saved");OnGameSave?.Invoke();}
    public static void GameResume(){Debug.Log("Game resumed");OnGameResume?.Invoke();}

    public static void GameOver()
    {
        Debug.Log("Game lost");
        GameIsOver = true;
        OnGameOver?.Invoke();
    }
    public static void GameEnd(){Debug.Log("Game ended");OnGameEnd?.Invoke();}
    
    public static void PlayerAttackedMelee(int damage){Debug.Log("Player took " + damage + " melee damage"); OnPlayerAttackedMelee?.Invoke(damage);}
}
