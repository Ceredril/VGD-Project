using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    //GAME EVENTS
    public static event Action OnGameStart,OnGameRestart,OnGameOver,OnGameEnd,OnGamePause,OnGameResume;
    public static event Action OnGameNew, OnGameLoad, OnGameSave;
    public static event Action OnPlayerDeath;
    
    //CHECKPOINT EVENTS
    public static event Action<Transform> OnCheckpointReached;
    public static event Action OnObjectInteraction;

    public static bool GameIsRunning;
    public static bool GameIsPaused;
    public static bool GameIsOver;

    public static bool PlayerIsAlive;

    public static CinemachineBrain cameraBrain;
    
    
    //DEFAULT FUNCTIONS
    private void Start()
    {
        if(PlayerPrefs.GetInt("SaveExists")==1)OnGameLoad?.Invoke();
        else OnGameNew?.Invoke();
        
        cameraBrain = FindObjectOfType<Camera>().GetComponent<CinemachineBrain>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameIsOver = false;
        GameIsRunning = true;
        PlayerIsAlive = true;
        OnGameStart?.Invoke();
        Debug.Log("Game started");
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameIsOver && !GameIsPaused && GameIsRunning) Pause();
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
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
        cameraBrain.enabled = false;
        Debug.Log("Game paused");
    }
    
    public static void Resume()
    {
        OnGameResume?.Invoke();
        GameIsPaused = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cameraBrain.enabled = true;
    }

    public static void GameSave()
    {
        Debug.Log("Game saved");
        OnGameSave?.Invoke();
    }
    
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

    public static void PlayerInteracted()
    {
        Debug.Log("Interacting with an Object");
        OnObjectInteraction?.Invoke();
    }

}