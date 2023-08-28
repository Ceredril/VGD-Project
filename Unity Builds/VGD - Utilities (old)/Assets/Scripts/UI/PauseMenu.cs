using UnityEngine;
using Cinemachine;

//REMINDER A ME STESSO (FEDE GIUGNI) --> SISTEMA GAMESAVE E RETURNTOMENU DA INSPECTOR

public class PauseMenu : MonoBehaviour
{
    public void OnGameResume() => GameManager.Resume();   //?


    private void Start()
    {
        gameObject.SetActive(false);
        GameManager.OnGamePause += EnablePauseMenuUI;
        GameManager.OnGameResume += DisablePauseMenuUI;
    }
    private void OnDestroy()
    {
        GameManager.OnGamePause -= EnablePauseMenuUI;
        GameManager.OnGameResume -= DisablePauseMenuUI;
    }

    private void EnablePauseMenuUI() => gameObject.SetActive(true);
    private void DisablePauseMenuUI() => gameObject.SetActive(false);
}

