using UnityEngine;

public class DeathMenu : MonoBehaviour
{
    public void OnGameStart()
    {
        PlayerManager.AddLives(-1);
        PlayerPrefs.SetInt("Lives",PlayerManager.CurrentLives);
        PlayerPrefs.Save();
        GameManager.GameStart();
    }

    private void Start()
    {
        gameObject.SetActive(false);
        GameManager.OnPlayerDeath += EnableDeathMenuUI;
        GameManager.OnGameStart += DisableDeathMenuUI;
        GameManager.OnGameOver += DisableDeathMenuUI;
        GameManager.OnGameEnd += DisableDeathMenuUI;
    }

    private void OnDestroy()
    {
        GameManager.OnPlayerDeath -= EnableDeathMenuUI;
        GameManager.OnGameStart -= DisableDeathMenuUI;
        GameManager.OnGameOver -= DisableDeathMenuUI;
        GameManager.OnGameEnd -= DisableDeathMenuUI;
    }

    private void EnableDeathMenuUI() => gameObject.SetActive(true);
    private void DisableDeathMenuUI() => gameObject.SetActive(false);

}