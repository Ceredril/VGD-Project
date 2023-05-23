using UnityEngine;
using Cinemachine;

//REMINDER A ME STESSO (FEDE GIUGNI) --> SISTEMA GAMESAVE E RETURNTOMENU DA INSPECTOR

public class PauseMenu : MonoBehaviour
{
    public void OnGameResume() => GameManager.Resume();
    public void OnGameSave() => GameManager.GameSave();
    
    private CinemachineBrain cameraBrain;
    
    private void Start()
    {
        gameObject.SetActive(false);
        GameManager.OnGamePause += DisableCamera;
        GameManager.OnGamePause += EnablePauseMenuUI;
        GameManager.OnGameResume += EnableCamera;
        GameManager.OnGameResume += DisablePauseMenuUI;
        GameManager.OnGameEnd += GameManager.Resume;
        cameraBrain = FindObjectOfType<Camera>().GetComponent<CinemachineBrain>();
    }
    private void OnDestroy()
    {
        GameManager.OnGamePause -= DisableCamera;
        GameManager.OnGamePause -= EnablePauseMenuUI;
        GameManager.OnGameResume -= EnableCamera;
        GameManager.OnGameResume -= DisablePauseMenuUI;
    }

    private void EnableCamera() => cameraBrain.enabled = true;
    private void DisableCamera() => cameraBrain.enabled = false;

    private void EnablePauseMenuUI() => gameObject.SetActive(true);
    private void DisablePauseMenuUI() => gameObject.SetActive(false);
}

