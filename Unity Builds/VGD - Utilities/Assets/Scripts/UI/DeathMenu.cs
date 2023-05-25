using UnityEngine;

public class DeathMenu : MonoBehaviour
{
    public void OnGameRestart() => GameManager.GameRestart();
    private void Start()
    {
        gameObject.SetActive(false);
        GameManager.OnPlayerDeath += EnableDeathMenuUI;
        GameManager.OnGameRestart += DisableDeathMenuUI;
        GameManager.OnGameOver += DisableDeathMenuUI;
        GameManager.OnGameEnd += DisableDeathMenuUI;
    }

    private void OnDestroy()
    {
        GameManager.OnPlayerDeath -= EnableDeathMenuUI;
        GameManager.OnGameRestart -= DisableDeathMenuUI;
        GameManager.OnGameOver -= DisableDeathMenuUI;
        GameManager.OnGameEnd -= DisableDeathMenuUI;
    }

    private void EnableDeathMenuUI() => gameObject.SetActive(true);
    private void DisableDeathMenuUI() => gameObject.SetActive(false);

}