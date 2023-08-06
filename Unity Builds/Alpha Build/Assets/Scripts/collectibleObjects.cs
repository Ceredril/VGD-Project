using System;
using UnityEngine;
using Random = UnityEngine.Random;


public class collectibleObjects : MonoBehaviour
{
    private int _amount;
    private bool _wasCollected;

    private void Awake()
    {
        GameManager.OnGameStart += LoadProgress;
        GameManager.OnGameSave += SaveProgress;
    }

    private void Start()
    {
        if(_wasCollected)gameObject.SetActive(false);
        else gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        GameManager.OnGameStart -= LoadProgress;
        GameManager.OnGameSave -= SaveProgress;
    }

    public void OnTriggerEnter(Collider other)
    {
        _amount = Random.Range(33, 50);
        if (other.CompareTag("PlayerBody"))
        {
            if (CompareTag("manaCollectible")) {
                PlayerManager.AddMana(_amount);
                Debug.Log("User has collected " + _amount + " mana");
            }
            if (CompareTag("healthCollectible")){
                PlayerManager.AddHealth(_amount);
                Debug.Log("User has collected " + _amount + " health");
            }
            if (CompareTag("godModeCollectible"))
            {
                //activate godMode
                Debug.Log("User has collected godMode");
            }
            if (CompareTag("speedHackCollectible"))
            {
                //activate speedHak
                Debug.Log("User has collected  speedHack");
            }
            if (CompareTag("fireFistCollectible"))
            {
                //activate firefist
                Debug.Log("User has collected fireFist");
            }
            _wasCollected = true;
            GameManager.Collected(this.gameObject);
            gameObject.SetActive(false);
        }
    }
    
    private void LoadProgress()
    {
        if (PlayerPrefs.GetInt("SaveExists") == 1)_wasCollected = Convert.ToBoolean(PlayerPrefs.GetInt(name));
        else
        {
            _wasCollected = false;
            SaveProgress();
        }
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt(name,Convert.ToInt32(_wasCollected));
        PlayerPrefs.Save();
    }
}
