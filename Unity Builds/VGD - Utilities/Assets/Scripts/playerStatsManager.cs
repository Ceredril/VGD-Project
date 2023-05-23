using UnityEngine;

public class playerStatsManager : MonoBehaviour
// Start is called before the first frame update
{
    public static readonly int MaxLives = 5;
    public static readonly int MaxHealth = 100;
    public static readonly int MaxMana = 100;
    public static readonly int MaxStamina = 100;

    private static readonly int DefaultLives = 3;
    private static readonly int DefaultHealth = 80;
    private static readonly int DefaultMana = 80;
    private static readonly int DefaultStamina = 100;

    private Transform _lastPosition;
    private void Start()
    {
        GameManager.OnGameStart += SetStats;
        GameManager.OnGameRestart += SetStats;
        GameManager.OnGameSave += SaveProgress;
        GameManager.OnGameOver += LoadDefaultStats;
        GameManager.OnManaCollected += AddMana;
        GameManager.OnHealthCollected += AddHealth;
        GameManager.OnLivesCollected += IncreaseLives;
        GameManager.OnPlayerDeath += ReduceLives;
        GameManager.OnPlayerAttackedMelee += AddHealth;
        GameManager.OnPlayerAttackedRanged += AddHealth;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStart -= SetStats;
        GameManager.OnGameRestart -= SetStats;
        GameManager.OnGameSave -= SaveProgress;
        GameManager.OnGameOver -= LoadDefaultStats;
        GameManager.OnManaCollected -= AddMana;
        GameManager.OnHealthCollected -= AddHealth;
        GameManager.OnLivesCollected -= IncreaseLives;
        GameManager.OnPlayerDeath -= ReduceLives;
        GameManager.OnPlayerAttackedMelee -= AddHealth;
        GameManager.OnPlayerAttackedRanged -= AddHealth;
    }

    private void SetStats()
    {
        if(PlayerPrefs.GetInt("SaveExists")==1)LoadProgress();
        else if(PlayerPrefs.GetInt("SaveExists")==0)LoadDefaultStats();
        else Debug.Log("Error while setting the stats");
    }
    
    private void SaveProgress()
    {
        PlayerPrefs.SetInt("SaveExists", 1);
        PlayerPrefs.SetInt("Lives", PlayerStats.CurrentLives);
        PlayerPrefs.SetInt("Health", PlayerStats.CurrentHealth);
        PlayerPrefs.SetInt("Mana", PlayerStats.CurrentMana);
        PlayerPrefs.SetInt("Stamina", PlayerStats.CurrentStamina);
        PlayerPrefs.SetString("LastCheckpoint", spawnManager.Instance.lastCheckpoint.name);
        PlayerPrefs.Save();
        Debug.Log("Progress saved");
    }

    private void LoadProgress()
    {
        PlayerStats.CurrentLives = PlayerPrefs.GetInt("Lives");
        PlayerStats.CurrentHealth = PlayerPrefs.GetInt("Health");
        PlayerStats.CurrentMana = PlayerPrefs.GetInt("Mana");
        PlayerStats.CurrentStamina = PlayerPrefs.GetInt("Stamina");
        spawnManager.Instance.spawnPoint = GameObject.Find(PlayerPrefs.GetString("LastCheckpoint")).transform;
        spawnManager.Instance.lastCheckpoint = GameObject.Find(PlayerPrefs.GetString("LastCheckpoint")).transform;
        Debug.Log("Progress loaded");
    }

    private void LoadDefaultStats()
    {
        PlayerStats.CurrentLives = DefaultLives;
        PlayerStats.CurrentHealth = DefaultHealth;
        PlayerStats.CurrentMana = DefaultMana;
        PlayerStats.CurrentStamina = DefaultStamina;
        spawnManager.Instance.spawnPoint = GameObject.Find("checkPoint_0").transform;
        spawnManager.Instance.lastCheckpoint = GameObject.Find("checkPoint_0").transform;
        Debug.Log("Default stats set.");
    }
    
    private static void SetCurrentLives(int amount)
    {
        if (amount > MaxLives) PlayerStats.CurrentLives = MaxLives;
        else if (amount < 0) {
            PlayerStats.CurrentLives = 0;
            GameManager.GameOver();
        }else PlayerStats.CurrentLives = amount;
        Debug.Log("Lives set to " + PlayerStats.CurrentLives);
    }
    
    private static void SetCurrentHealth(int amount)
    {
        if (amount > MaxHealth) PlayerStats.CurrentHealth = MaxHealth;
        else if (amount < 1) {
            PlayerStats.CurrentHealth = 0;
            GameManager.PlayerDeath();
        }else PlayerStats.CurrentHealth = amount;
        Debug.Log("Health set to " + PlayerStats.CurrentHealth);
    }
    
    private static void SetCurrentMana(int amount)
    {
        if (amount > MaxMana) PlayerStats.CurrentMana = MaxMana;
        else if (amount < 0) PlayerStats.CurrentMana = 0;
        else PlayerStats.CurrentMana = amount;
        Debug.Log("Mana set to " + PlayerStats.CurrentMana);
    }
    
    public static void SetCurrentStamina(int amount)
    {
        if (amount > MaxStamina) PlayerStats.CurrentStamina = MaxStamina;
        else if (amount < 0) PlayerStats.CurrentStamina = 0;
        else PlayerStats.CurrentStamina = amount;
        Debug.Log("Stamina set");
    }


    private void IncreaseLives(int amount)
    {
        int newLive = PlayerStats.CurrentLives + amount;
        SetCurrentLives(newLive);
    }
    
    private void ReduceLives()
    {
        int newLives = PlayerStats.CurrentLives - 1;
        SetCurrentLives(newLives);
    }
    
    public static void AddHealth(int amount)
    {
        int newHealth = PlayerStats.CurrentHealth + amount;
        SetCurrentHealth(newHealth);
    }
    
    public static void AddMana(int amount)
    {
        int newMana = PlayerStats.CurrentMana + amount;
        SetCurrentMana(newMana);
    }
    
    public static void AddStamina(int amount)
    {
        int newStamina = PlayerStats.CurrentStamina + amount;
        SetCurrentStamina(newStamina);
    }
}
