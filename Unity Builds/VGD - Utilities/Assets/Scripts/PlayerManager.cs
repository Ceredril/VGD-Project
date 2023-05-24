using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //Object references
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform characterCamera;
    [HideInInspector] public Animator animator;
    
    //Stats parameters
    public static readonly int MaxLives = 5;
    public static readonly int MaxHealth = 100;
    public static readonly int MaxMana = 100;
    public static readonly int MaxStamina = 100;
    private static readonly int DefaultLives = 3;
    private static readonly int DefaultHealth = 80;
    private static readonly int DefaultMana = 80;
    private static readonly int DefaultStamina = 100;
    //Cooldown parameters
    private readonly int GodModeCooldown = 5;
    private readonly int SpeedHackCooldown = 15;
    private readonly int FireFistsCooldown = 10;
    
    //Movement variables
    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float sprintSpeed = 4f;
    [SerializeField] private float jumpForce = 4f;
    private float _speedOffset;
    //Gravity variables
    private readonly float _gravity = 9.81f;
    private float _verticalSpeed;
    //Spawn variables
    public static Transform SpawnPoint;
    public static Transform LastCheckpoint;
    //Attack variables
    private bool _canHit=true;
    private static float _attackCooldown=5f;
    //Powerup variables
    private static bool godModeEnabled;
    //Stats variables
    public static int CurrentLives;
    public static int CurrentHealth;
    public static int CurrentMana;
    public static int CurrentStamina;
    public static string StartParams = 
        MaxLives + " mL\n" + 
        MaxHealth + " mH\n" + 
        MaxMana + " mM\n" + 
        MaxStamina + " mS";


    
    //Utility functions
    private IEnumerator Wait(float amount)
    {
        _canHit = false;
        yield return new WaitForSeconds(amount);
        _canHit = true;
    }
    
    //Movement functions
    private void Move()
    {
        if (GameManager.GameIsOver || !GameManager.GameIsRunning || !GameManager.PlayerIsAlive) return;
        //Get keyboard inputs
        float keyboardInputHorizontal = Input.GetAxis("Horizontal");
        float keyboardInputVertical = Input.GetAxis("Vertical");
        Vector3 movementDirection = new Vector3(keyboardInputHorizontal, 0f, keyboardInputVertical).normalized;


        // Get the camera's forward direction without the vertical component
        Vector3 cameraForward = characterCamera.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        // Rotate the character to face the camera direction
        if (cameraForward.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            transform.rotation = targetRotation;
        }

        // Calculate the movement vector in the camera's relative direction
        Vector3 movement = movementDirection.x * characterCamera.right + movementDirection.z * cameraForward;
        movement.Normalize();

        // If the character is moving, apply the movement to the character controller
        if (movementDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg + characterCamera.eulerAngles.y;
            movementDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        
        //If user presses Spacebar and the character is grounded, apply jump force. Otherwise let it fall :D
        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded) {
            _verticalSpeed = jumpForce;
            animator.SetBool("jump", true);
        } else _verticalSpeed -= _gravity * Time.deltaTime;


        //If the user is holding Shift, assign sprintSpeed to the speedOffset. 
        if (Input.GetKey(KeyCode.LeftShift)) _speedOffset = sprintSpeed;
        else _speedOffset = walkingSpeed;

        //Update the movement vector with the new values
        movementDirection.y = _verticalSpeed;
        movementDirection.x *= _speedOffset;
        movementDirection.z *= _speedOffset;


        //Apply the movement to the character
        characterController.Move(movementDirection * Time.deltaTime);

        //Passing the horizontal and vertical value to the animator
        animator.SetFloat("hInput", keyboardInputHorizontal);
        animator.SetFloat("vInput", keyboardInputVertical);


        // Setting the jump and hit states based on keystroke
        if (Input.GetKeyUp(KeyCode.Space)) animator.SetBool("jump", false);
        if (Input.GetMouseButtonDown(0) && _canHit) Attack();

            // Switch between states based on whether you are running or walking
        if (_speedOffset == sprintSpeed) animator.SetBool("running", true);
        else animator.SetBool("running", false);

    }
        
    //Spawn Functions
    private void Spawn()
    {
        GameObject.Find("Player Body").transform.position = SpawnPoint.transform.position;
        Physics.SyncTransforms();
        Debug.Log("Player life =" + CurrentHealth);
    }
    private void Respawn()
    {
        if (GameManager.GameIsOver) return;
        GameObject.Find("Player Body").transform.position = LastCheckpoint.transform.position;
        Physics.SyncTransforms();
    }
    private void SetSpawnPoint(Transform checkpoint)
    {
        LastCheckpoint = checkpoint;
        Debug.Log("Spawn point set");
    }
    
    //Attack functions
    private void Attack()
    {
        animator.SetTrigger("hook");
        StartCoroutine(Wait(_attackCooldown));
    }
    
    //Powerup functions
    private void GodMode() {}
    private void SpeedHack(){}
    private void FireFists(){}
    
    //Stats functions
    private void SetStats()
    {
        if(PlayerPrefs.GetInt("SaveExists")==1)LoadProgress();
        else if(PlayerPrefs.GetInt("SaveExists")==0)LoadDefaultStats();
        else Debug.Log("Error while setting the stats");
    }
    
    private void SaveProgress()
    {
        PlayerPrefs.SetInt("SaveExists", 1);
        PlayerPrefs.SetInt("Lives", CurrentLives);
        PlayerPrefs.SetInt("Health", CurrentHealth);
        PlayerPrefs.SetInt("Mana", CurrentMana);
        PlayerPrefs.SetInt("Stamina", CurrentStamina);
        PlayerPrefs.SetString("LastCheckpoint", LastCheckpoint.name);
        PlayerPrefs.Save();
        Debug.Log("Progress saved");
    }

    private void LoadProgress()
    {
        CurrentLives = PlayerPrefs.GetInt("Lives");
        CurrentHealth = PlayerPrefs.GetInt("Health");
        CurrentMana = PlayerPrefs.GetInt("Mana");
        CurrentStamina = PlayerPrefs.GetInt("Stamina");
        SpawnPoint = GameObject.Find(PlayerPrefs.GetString("LastCheckpoint")).transform;
        LastCheckpoint = GameObject.Find(PlayerPrefs.GetString("LastCheckpoint")).transform;
        Debug.Log("Progress loaded");
    }
    private void LoadDefaultStats()
    {
        CurrentLives = DefaultLives;
        CurrentHealth = DefaultHealth;
        CurrentMana = DefaultMana;
        CurrentStamina = DefaultStamina;
        SpawnPoint = GameObject.Find("checkPoint_0").transform;
        LastCheckpoint = GameObject.Find("checkPoint_0").transform;
        Debug.Log("Default stats set.");
    }
    
    static void AddLives(int amount)
    {
        if (CurrentLives + amount > MaxLives) CurrentLives = MaxLives;
        else if (CurrentLives + amount < 1) {
            CurrentLives = 0;
            GameManager.GameOver();
        }else CurrentLives += amount;
        Debug.Log("Lives set to " + CurrentLives);
    }
    public static void AddHealth(int amount)
    {
        if (CurrentHealth+amount > MaxHealth) CurrentHealth = MaxHealth;
        else if (CurrentHealth+amount < 1) {
            GameManager.PlayerDeath();
            AddLives(-1);
            CurrentHealth = DefaultHealth;
            CurrentMana = DefaultMana;
            CurrentStamina = DefaultStamina;
        }else CurrentHealth += amount;
        Debug.Log("Health set to " + CurrentHealth);
    }
    public static void AddMana(int amount)
    {
        if (CurrentMana+amount > MaxMana) CurrentMana = MaxMana;
        else if (CurrentMana+amount < 0) CurrentMana = 0;
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

    // Start is called before the first frame update
    private void Awake()
    {
        //Components
        characterController = GetComponentInChildren<CharacterController>();
        characterCamera = GetComponentInChildren<Camera>().transform;
        animator = GetComponentInChildren<Animator>();
        SpawnPoint = GameObject.Find("checkPoint_0").transform;
        LastCheckpoint = SpawnPoint;

        //Events
        GameManager.OnGameStart += SetStats;
        GameManager.OnGameStart += Spawn;
        GameManager.OnGameRestart += Respawn;
        GameManager.OnGameSave += SaveProgress;
        GameManager.OnGameOver += LoadDefaultStats;
        GameManager.OnManaCollected += AddMana;
        GameManager.OnHealthCollected += AddHealth;
        GameManager.OnLivesCollected += AddLives;
        GameManager.OnPlayerAttackedMelee += AddHealth;
        GameManager.OnPlayerAttackedRanged += AddHealth;
        GameManager.OnCheckpointReached += SetSpawnPoint;
    }
    
    private void OnDestroy()
    {
        //Stats events
        GameManager.OnGameStart -= SetStats;
        GameManager.OnGameStart -= Spawn;
        GameManager.OnGameRestart -= Respawn;
        GameManager.OnGameSave -= SaveProgress;
        GameManager.OnGameOver -= LoadDefaultStats;
        GameManager.OnManaCollected -= AddMana;
        GameManager.OnHealthCollected -= AddHealth;
        GameManager.OnLivesCollected -= AddLives;
        GameManager.OnPlayerAttackedMelee -= AddHealth;
        GameManager.OnPlayerAttackedRanged -= AddHealth;
        GameManager.OnCheckpointReached -= SetSpawnPoint;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
}
