using System;
using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //Object references
    public static Animator animator;
    //gameobject for effects
    public static GameObject healthEffect;
    public static GameObject manaEffect;
    //Stats parameters
    public static readonly int MaxLives = 5;
    public static readonly int MaxHealth = 100;
    public static readonly int MaxMana = 100;
    public static readonly float MaxStamina = 100;
    private static readonly int DefaultLives = 3;
    private static readonly int DefaultHealth = 80;
    private static readonly int DefaultMana = 80;
    private static readonly int DefaultStamina = 100;


    //Spawn variables
    public static Transform SpawnPoint;
    public static Transform LastCheckpoint;
    
    //Stats variables
    public static int CurrentLives;
    public static int CurrentHealth;
    public static float CurrentMana;
    public static float CurrentStamina;
    public static bool IsAlive;

    public static int ShieldSManaUse = 2;


    // Start is called before the first frame update
    private void Awake()
    {
        animator = GetComponent<Animator>();
        SpawnPoint = GameObject.Find("Spawn").transform;
        healthEffect = GameObject.Find("Health Effect");
        manaEffect = GameObject.Find("Mana Effect");
        healthEffect.SetActive(false);
        manaEffect.SetActive(false);
        

        //Events
        GameManager.OnGameStart += Spawn;
        GameManager.OnGameStart += LoadProgress;
        GameManager.OnGameSave += SaveProgress;
        GameManager.OnCheckpointReached += SetSpawnPoint;
        GameManager.OnGameSave += SaveProgress;
    }

    private void Update()
    {
        if (!IsAlive) return;
        if (CurrentHealth < 1)
        {
            IsAlive = false;
            animator.SetTrigger("death");
            PlayerAttack.shield.SetActive(false);
            if (CurrentLives > 0) GameManager.PlayerDeath();
            else if (CurrentLives < 1) GameManager.GameOver();
        }

        if (healthEffect.active || manaEffect.active) //mana and health collectible effect duration
            StartCoroutine(Wait());

        if(PlayerAttack._currentSkill == PlayerAttack.Skill.Shield) 
            CurrentMana -= ShieldSManaUse * Time.deltaTime;
    }

    private void OnDestroy()
    {
        //Stats events
        GameManager.OnGameStart -= Spawn;
        GameManager.OnGameStart -= LoadProgress;
        GameManager.OnCheckpointReached -= SetSpawnPoint;
        GameManager.OnGameSave -= SaveProgress;
    }

    //Spawn Functions
    private void Spawn()
    {
        PlayerAttack._currentSkill = PlayerAttack.Skill.Fist;
        animator.Play("Walking Tree");
        GameObject.Find("Player Body").transform.position = SpawnPoint.transform.position;
        Physics.SyncTransforms();
        Debug.Log("Player spawned at " + SpawnPoint);
    }

    private void SetSpawnPoint(Transform checkpoint)
    {
        SpawnPoint = checkpoint;
        Debug.Log("Spawn point set");
    }

    public static void AddLives(int amount)
    {
        if (CurrentLives + amount > MaxLives) CurrentLives = MaxLives;
        else CurrentLives += amount;
        Debug.Log("Lives set to " + CurrentLives);
    }
    public static void AddHealth(int amount)
    {
        if (PlayerPowerUps.GodModeEnabled && amount < 0) return;
        if (PlayerAttack._currentSkill == PlayerAttack.Skill.Shield && amount < 0) { 
            float newAmount = amount - (amount * 0.6f);
            CurrentHealth += (int)newAmount;
        }
        else if (CurrentHealth + amount > MaxHealth) CurrentHealth = MaxHealth;
        else CurrentHealth += amount;
        Debug.Log("Health set to " + CurrentHealth);
    }

    public static void AddMana(int amount)
    {
        if (PlayerPowerUps.InfiniteMana && amount < 0) return;
        if (CurrentMana + amount > MaxMana) CurrentMana = MaxMana;
        else if (CurrentMana + amount < 0) CurrentMana = 0;
        else CurrentMana += amount;
        Debug.Log("Mana set to " + CurrentMana);
    }
    public static void AddStamina(int amount)
    {
        if (CurrentStamina + amount > MaxStamina) CurrentStamina = MaxStamina;
        else if (CurrentStamina + amount < 0) CurrentStamina = 0;
        else CurrentStamina += amount;
        Debug.Log("Stamina set");
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt("SaveExists", 1);
        PlayerPrefs.SetInt("Lives", CurrentLives);
        PlayerPrefs.SetInt("Health", CurrentHealth);
        PlayerPrefs.SetFloat("Mana", CurrentMana);
        PlayerPrefs.SetFloat("Stamina", CurrentStamina);
        PlayerPrefs.SetString("SpawnPoint", SpawnPoint.name);
        PlayerPrefs.Save();
        Debug.Log("Progress saved");
    }
    private void LoadProgress()
    {
        if (PlayerPrefs.GetInt("SaveExists") == 1)
        {
            IsAlive = true;
            CurrentLives = PlayerPrefs.GetInt("Lives");
            CurrentHealth = PlayerPrefs.GetInt("Health");
            CurrentMana = PlayerPrefs.GetInt("Mana");
            CurrentStamina = PlayerPrefs.GetInt("Stamina");
            SpawnPoint = GameObject.Find(PlayerPrefs.GetString("SpawnPoint")).transform;
            LastCheckpoint = GameObject.Find(PlayerPrefs.GetString("SpawnPoint")).transform;
            Debug.Log("Progress loaded");
        }else
        {
            IsAlive = true;
            CurrentLives = DefaultLives;
            CurrentHealth = DefaultHealth;
            CurrentMana = DefaultMana;
            CurrentStamina = DefaultStamina;
            SpawnPoint = GameObject.Find("Spawn").transform;
            LastCheckpoint = GameObject.Find("Spawn").transform;
            Debug.Log("Default stats set.");
        }
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.3f);
        healthEffect.SetActive(false);
        manaEffect.SetActive(false);
    }
}
