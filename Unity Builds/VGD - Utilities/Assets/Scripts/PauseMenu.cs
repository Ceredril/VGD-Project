using UnityEngine;
using Cinemachine;


public class PauseMenu : MonoBehaviour
{
    public void OnGameResume() => GameManager.GameResume();
    public void OnGameSave() => GameManager.GameSave();
    public void OnGameEnd() => GameManager.GameEnd();

    private CinemachineBrain cameraBrain;
    
    private void Start()
    {
        gameObject.SetActive(false);
        GameManager.OnGamePause += Pause;
        GameManager.OnGameResume += Resume;
        GameManager.OnGameEnd += Unpause;
        cameraBrain = FindObjectOfType<Camera>().GetComponent<CinemachineBrain>();
    }

    private void OnDestroy()
    {
        GameManager.OnGamePause -= Pause;
        GameManager.OnGameResume -= Resume;
        GameManager.OnGameEnd -= Unpause;
    }

    private void Pause()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        cameraBrain.enabled = false;
        gameObject.SetActive(true);
    }

    private void Resume()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cameraBrain.enabled = true;
        gameObject.SetActive(false);
    }

    private void Unpause()
    {
        Time.timeScale = 1;
    }
}

