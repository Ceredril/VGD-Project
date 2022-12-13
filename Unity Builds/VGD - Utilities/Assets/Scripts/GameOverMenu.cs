using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    
    public void OnGameEnd() => GameManager.GameEnd();
    
    private void Start()
    {
        gameObject.SetActive(false);
        GameManager.OnGameOver += Show;
    }

    private void OnDestroy()
    {
        GameManager.OnGameOver -= Show;
    }

    void Show()
    {
        gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

}
