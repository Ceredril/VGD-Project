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
    
    private void Start()
    {
        GameManager.OnGameStart += SetStats;
        GameManager.OnGameSave += SaveProgress;
        GameManager.OnManaCollected += AddMana;
        GameManager.OnHealthCollected += AddHealth;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStart -= SetStats;
        GameManager.OnGameSave -= SaveProgress;
        GameManager.OnManaCollected -= AddMana;
        GameManager.OnHealthCollected -= AddHealth;
    }

    private void SetStats()
    {
        if(PlayerPrefs.GetInt("ExistingSave")==1)LoadProgress();
        else if(PlayerPrefs.GetInt("ExistingSave")==0)LoadDefaultStats();
        else Debug.Log("Error while setting the stats");
    }
    
    private void SaveProgress()
    {
        PlayerPrefs.SetInt("ExistingSave", 1);
        PlayerPrefs.SetInt("Lives", PlayerStats.CurrentLives);
        PlayerPrefs.SetInt("Health", PlayerStats.CurrentHealth);
        PlayerPrefs.SetInt("Mana", PlayerStats.CurrentMana);
        PlayerPrefs.SetInt("Stamina", PlayerStats.CurrentStamina);
        //last position
        PlayerPrefs.SetFloat("LastPosX", BodyMovement.instance.transform.position.x);
        PlayerPrefs.SetFloat("LastPosY", BodyMovement.instance.transform.position.y);
        PlayerPrefs.SetFloat("LastPosZ", BodyMovement.instance.transform.position.z);
        //last reached checkpoint
        PlayerPrefs.SetString("LastCheckpoint", spawnManager.LastCheckpoint.name);
        Debug.Log("Progress saved");
    }

    private void LoadProgress()
    {
        PlayerStats.CurrentLives = PlayerPrefs.GetInt("Lives");
        PlayerStats.CurrentHealth = PlayerPrefs.GetInt("Health");
        PlayerStats.CurrentMana = PlayerPrefs.GetInt("Mana");
        PlayerStats.CurrentStamina = PlayerPrefs.GetInt("Stamina");
        //last position
        BodyMovement.instance.transform.position = new Vector3(PlayerPrefs.GetFloat("LastPosX"), PlayerPrefs.GetFloat("LastPosY"), PlayerPrefs.GetFloat("LastPosZ"));
        //last reached checkpoint
        spawnManager.LastCheckpoint = GameObject.Find(PlayerPrefs.GetString("LastCheckpoint")).transform;
        Debug.Log("Progress loaded");
    }

    private void LoadDefaultStats()
    {
        PlayerPrefs.SetString("ExistingSave", "FALSE");
        PlayerStats.CurrentLives = DefaultLives;
        PlayerStats.CurrentHealth = DefaultHealth;
        PlayerStats.CurrentMana = DefaultMana;
        PlayerStats.CurrentStamina = DefaultStamina;
        Debug.Log("Default stats set.");
    }
    
    private static void SetCurrentLives(int amount)
    {
        if (amount > MaxLives) PlayerStats.CurrentLives = MaxLives;
        else if (amount < 0) {
            PlayerStats.CurrentLives = 0;
            GameManager.GameOver();
        }else PlayerStats.CurrentLives = amount;
    }
    
    private static void SetCurrentHealth(int amount)
    {
        if (amount > MaxHealth) PlayerStats.CurrentHealth = MaxHealth;
        else if (amount < 0) {
            PlayerStats.CurrentHealth = 0;
            GameManager.PlayerDeath();
        }else PlayerStats.CurrentHealth = amount;
        Debug.Log("Health: " + PlayerStats.CurrentHealth);
    }
    
    private static void SetCurrentMana(int amount)
    {
        if (amount > MaxMana) PlayerStats.CurrentMana = MaxMana;
        else if (amount < 0) PlayerStats.CurrentMana = 0;
        else PlayerStats.CurrentMana = amount;
        Debug.Log("Mana: " + PlayerStats.CurrentMana);
    }
    
    public static void SetCurrentStamina(int amount)
    {
        if (amount > MaxStamina) PlayerStats.CurrentStamina = MaxStamina;
        else if (amount < 0) PlayerStats.CurrentStamina = 0;
        else PlayerStats.CurrentStamina = amount;
    }


    private void IncreaseLives()
    {
        int newLive = PlayerStats.CurrentLives + 1;
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
