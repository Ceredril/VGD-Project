using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public static int GameDifficulty = 0;
    public void SetDifficulty(int difficulty)
    {
        GameDifficulty = difficulty;
    }
    public void NewGame()
    {
        if (GameDifficulty > 0)
        {
            PlayerPrefs.SetInt("SaveExists", 0);
            PlayerPrefs.Save();
            SceneManager.LoadScene("GameView", LoadSceneMode.Single);
            Debug.Log("Started a new Game with difficulty: " + GameDifficulty);
        }
        else
        {
            Debug.Log("Tried starting the game without selecting a difficulty.");
        }
    }
    public void LoadGame()
    {
        if (PlayerPrefs.GetInt("SaveExists") == 1)
        {
            SceneManager.LoadScene("GameView", LoadSceneMode.Single);
            Debug.Log("Loaded an existing Game");
        }
        else
        {
            Debug.Log("Save does not exist");
        }
    }
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void playAudio(string sound)
    {
        audioManager.Play(sound);
    }
}
