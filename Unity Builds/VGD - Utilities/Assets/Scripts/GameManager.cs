using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static event Action OnPlayerSpawn, OnPlayerDeath;
    public static event Action<int> OnManaCollected, OnHealthCollected, OnLivesCollected;
    public static event Action<Transform> OnCheckpointReached;
    public static event Action OnGameStart, OnGamePause, OnGameSave, OnGameResume, OnGameEnd, OnGameOver;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        PlayerPrefs.SetInt("ExistingSave", 0);
        GameStart();
        OnGameEnd += MoveToMainMenu;
    }

    private void OnDestroy() => OnGameEnd -= MoveToMainMenu;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))OnGamePause?.Invoke();
        if(Input.GetKeyDown(KeyCode.T))PlayerSpawn();
    }

private void MoveToMainMenu()
{
    SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
}
    private void ShowCursor(){Cursor.visible = true; Cursor.lockState = CursorLockMode.None;}
    private void HideCursor(){Cursor.visible = false; Cursor.lockState = CursorLockMode.Locked;}

    
    public static void ManaCollected(int amount) => OnManaCollected?.Invoke(amount);
    public static void HealthCollected(int amount) => OnHealthCollected?.Invoke(amount);
    public static void LivesCollected(int amount) => OnLivesCollected?.Invoke(amount);
    public static void CheckpointReached(Transform checkpoint) => OnCheckpointReached?.Invoke(checkpoint);
    
    public static void PlayerSpawn() => OnPlayerSpawn?.Invoke();
    public static void PlayerDeath() => OnPlayerDeath?.Invoke();
    public static void GameStart(){Debug.Log("Game has started");OnGameStart?.Invoke();}
    public static void GamePause(){Debug.Log("Game has paused");OnGamePause?.Invoke();}
    public static void GameSave(){Debug.Log("Game has saved");OnGameSave?.Invoke();}
    public static void GameResume(){Debug.Log("Game has resumed");OnGameResume?.Invoke();}
    public static void GameOver(){Debug.Log("Game over");OnGameOver?.Invoke();}
    public static void GameEnd(){Debug.Log("Game has ended");OnGameEnd?.Invoke();}
}
