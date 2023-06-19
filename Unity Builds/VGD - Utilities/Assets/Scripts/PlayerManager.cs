using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //ENUMS
    enum AttackType {None,Melee,Ranged}
    //Object references
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform characterCamera;
    public static Animator animator;
    private static PlayerManager Instance;
    //Stats parameters
    public static readonly int MaxLives = 5;
    public static readonly int MaxHealth = 100;
    public static readonly int MaxMana = 100;
    public static readonly int MaxStamina = 100;
    private static readonly int DefaultLives = 3;
    private static readonly int DefaultHealth = 80;
    private static readonly int DefaultMana = 80;
    private static readonly int DefaultStamina = 100;
    private static bool _hasMeleeAttack = true;
    private static bool _hasRangedAttack = true;
    //Stamina parameters
    private bool canSprint = true;
    [SerializeField] private float StaminaUseMultiplier = 4;
    [SerializeField] private float TimeBeforeStaminaRegenStarts = 1.5f;
    private float StaminaValueIncrement = 2;
    private float StaminaTimeIncrement = 0.1f;
    public static float CurrentStamina;
    private Coroutine regeneratingStamina;
    public static Action<float> OnStaminaChange;
    //Cooldown parameters
    private readonly int GodModeCooldown = 5;
    private readonly int SpeedHackCooldown = 15;
    private readonly int FireFistsCooldown = 10;

    //Movement variables
    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float sprintSpeed = 4f;
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float JumpMultiplier = 10;
    private bool isJumping = false;
    private float _speedOffset;
    //Gravity variables
    private readonly float _gravity = 9.81f;
    private float _verticalSpeed;
    //Spawn variables
    public static Transform SpawnPoint;
    public static Transform LastCheckpoint;
    //Attack variables
    public static bool _canHit = true;
    private bool _isNear;
    private static float _attackCooldown = 1f;
    private AttackType _currentAttack;
    //Powerup variables
    private static bool godModeEnabled = false;
    //Stats variables
    public static int CurrentLives;
    public static int CurrentHealth;
    public static int CurrentMana;
    public static string StartParams =
        MaxLives + " mL\n" +
        MaxHealth + " mH\n" +
        MaxMana + " mM\n" +
        MaxStamina + " mS";


    //Utility functions
    private static IEnumerator Wait(float amount)
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
        
        if (movementDirection.magnitude >= 0.1f)
        {   // If the character is moving, apply the movement to the character controller
            float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg + characterCamera.eulerAngles.y;
            movementDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }


        //If user presses Spacebar and the character is grounded, apply jump force. Otherwise let it fall :D
        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded && CurrentStamina >= JumpMultiplier)
        {
            _verticalSpeed = jumpForce;
            animator.SetBool("jump", true);
            isJumping = true;
        }
        else _verticalSpeed -= _gravity * Time.deltaTime;


        //If the user is holding Shift, assign sprintSpeed to the speedOffset. 
        if (Input.GetKey(KeyCode.LeftShift) && canSprint) _speedOffset = sprintSpeed;
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
        if (Input.GetKeyUp(KeyCode.Space)) { animator.SetBool("jump", false); isJumping = false; }
        // Switch between states based on whether you are running or walking and managing stamina level
        // first if, manage the stamina when the character is running or is running and jumping at the same time
        if (_speedOffset == sprintSpeed && movementDirection.magnitude >= 0.1f)
        {
            animator.SetBool("running", true);
            if (regeneratingStamina != null)
            {
                StopCoroutine(regeneratingStamina);
                regeneratingStamina = null;
            }
            if (isJumping)   // that means it's running and jumping 
                CurrentStamina -= (JumpMultiplier + StaminaUseMultiplier) * Time.deltaTime;
            else            // here is just running 
                CurrentStamina -= StaminaUseMultiplier * Time.deltaTime;
            if (CurrentStamina < 0)
                CurrentStamina = 0;
            OnStaminaChange?.Invoke(CurrentStamina);
            if (CurrentStamina <= 0)
            {
                canSprint = false;
            }
        }
        else if (_speedOffset != sprintSpeed && isJumping)  // this second if, manage the stamina when the character is walking or standing still but is jumping
        {
            if (regeneratingStamina != null)
            {
                StopCoroutine(regeneratingStamina);
                regeneratingStamina = null;
            }
            CurrentStamina -= JumpMultiplier * Time.deltaTime;
            if (CurrentStamina < 0)
                CurrentStamina = 0;
            OnStaminaChange?.Invoke(CurrentStamina);
            if (CurrentStamina <= 0)
            {
                canSprint = false;
            }
        }
        else if (_speedOffset != sprintSpeed && CurrentStamina < MaxStamina && regeneratingStamina == null)// when the character is still,it begin to regenerate stamina
        {
            animator.SetBool("running", false);
            regeneratingStamina = StartCoroutine(RegenerateStamina());
        }
    }

    //Sprint and stamina function
    private IEnumerator RegenerateStamina()
    {
        yield return new WaitForSeconds(TimeBeforeStaminaRegenStarts);
        WaitForSeconds timeToWait = new WaitForSeconds(StaminaTimeIncrement);
        while (CurrentStamina < MaxStamina)
        {
            if (CurrentStamina > 0)
            {
                canSprint = true;
                //canJump = true;
            }
            CurrentStamina += StaminaValueIncrement;
            if (CurrentStamina > MaxStamina)
                CurrentStamina = MaxStamina;
            CurrentStamina -= StaminaUseMultiplier * Time.deltaTime;
            yield return timeToWait;
        }
        regeneratingStamina = null;
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
        animator.SetTrigger("alive");
        Physics.SyncTransforms();
    }
    private void SetSpawnPoint(Transform checkpoint)
    {
        LastCheckpoint = checkpoint;
        Debug.Log("Spawn point set");
    }

    //Attack functions
    private void Attack(AttackType attackType)
    {
        switch (attackType)
        {
            case AttackType.None:
                //Player starts a dialogue saying "I should learn how to fight.. Maybe some villager here could help me to do so"
                break;
            case AttackType.Melee:
                animator.SetTrigger("hook");
                Instance.StartCoroutine(Wait(_attackCooldown));
                break;
            case AttackType.Ranged:
                //Ranged attack to be implemented
                break;
        }

    }

    //Powerup functions
    private IEnumerator GodMode(float cooldownTime)
    {
        godModeEnabled = true;
        yield return new WaitForSeconds(cooldownTime);
        godModeEnabled = false;
    }

    private IEnumerator SpeedHack(float cooldownTime)
    {
        walkingSpeed *= 2;
        sprintSpeed *= 2;
        yield return new WaitForSeconds(cooldownTime);
        walkingSpeed /= 2;
        sprintSpeed /= 2;
    }

    private IEnumerator FireFists(float cooldownTime)
    {
        _attackCooldown /= 4;
        yield return new WaitForSeconds(cooldownTime);
        _attackCooldown *= 4;
    }

    //Stats functions
    private void SetStats()
    {
        if (PlayerPrefs.GetInt("SaveExists") == 1) LoadProgress();
        else if (PlayerPrefs.GetInt("SaveExists") == 0) LoadDefaultStats();
        else Debug.Log("Error while setting the stats");
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt("SaveExists", 1);
        PlayerPrefs.SetInt("Lives", CurrentLives);
        PlayerPrefs.SetInt("Health", CurrentHealth);
        PlayerPrefs.SetInt("Mana", CurrentMana);
        PlayerPrefs.SetFloat("Stamina", CurrentStamina);
        PlayerPrefs.SetString("LastCheckpoint", LastCheckpoint.name);
        PlayerPrefs.SetInt("HasMeleeAttack", Convert.ToInt32(_hasMeleeAttack));
        PlayerPrefs.SetInt("HasRangedAttack", Convert.ToInt32(_hasRangedAttack));
        PlayerPrefs.SetInt("CurrentAttack", (int)_currentAttack);
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
        _hasMeleeAttack = Convert.ToBoolean(PlayerPrefs.GetInt("HasMeleeAttack"));
        _hasRangedAttack = Convert.ToBoolean(PlayerPrefs.GetInt("hasRangedAttack"));
        _currentAttack = (AttackType)PlayerPrefs.GetInt("CurrentAttack");
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
        else if (CurrentLives + amount < 1)
        {
            CurrentLives = 0;
            GameManager.GameOver();
        }
        else CurrentLives += amount;
        Debug.Log("Lives set to " + CurrentLives);
    }
    public static void AddHealth(int amount)
    {
        if (godModeEnabled && amount < 0) return;
        if (CurrentHealth + amount > MaxHealth) CurrentHealth = MaxHealth;
        else if (CurrentHealth + amount < 1)
        {
            GameManager.PlayerDeath();
            animator.SetTrigger("death");
            AddLives(-1);
            CurrentHealth = DefaultHealth;
            CurrentMana = DefaultMana;
            CurrentStamina = DefaultStamina;
        }
        else CurrentHealth += amount;
        Debug.Log("Health set to " + CurrentHealth);
    }

    public static void AddMana(int amount)
    {
        if (amount < 0 && godModeEnabled) return;
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

    // Start is called before the first frame update
    private void Awake()
    {
        //Components
        characterController = GetComponent<CharacterController>();
        characterCamera = GameObject.Find("Main Camera").transform;
        animator = GetComponent<Animator>();
        SpawnPoint = GameObject.Find("checkPoint_0").transform;
        LastCheckpoint = SpawnPoint;
        Instance = this;
        CurrentStamina = DefaultStamina;

        //Events
        GameManager.OnGameStart += SetStats;
        GameManager.OnGameStart += Spawn;
        GameManager.OnGameRestart += Respawn;
        GameManager.OnGameSave += SaveProgress;
        GameManager.OnGameOver += LoadDefaultStats;
        GameManager.OnManaCollected += AddMana;
        GameManager.OnHealthCollected += AddHealth;
        GameManager.OnLivesCollected += AddLives;
        GameManager.OnMeleeEnemyAttacks += AddHealth;
        GameManager.OnRangedEnemyAttacks += AddHealth;
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
        GameManager.OnMeleeEnemyAttacks -= AddHealth;
        GameManager.OnRangedEnemyAttacks -= AddHealth;
        GameManager.OnCheckpointReached -= SetSpawnPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.GameIsPaused) Move();

        if (Input.GetKeyDown(KeyCode.Alpha1) && _hasMeleeAttack)
        {
            _currentAttack = AttackType.Melee;
            Debug.Log("Melee attack selected");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && _hasRangedAttack)
        {
            _currentAttack = AttackType.Ranged;
            Debug.Log("Ranged attack selected");
        }

        if (Input.GetMouseButtonDown(0) && _currentAttack==AttackType.Ranged) Attack(_currentAttack);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("godModePowerup"))
        {
            GodMode(GodModeCooldown);
            Destroy(other.GameObject());
        }
        if (other.CompareTag("speedHackPowerup"))
        {
            SpeedHack(SpeedHackCooldown);
            Destroy(other.GameObject());
        }
        if (other.CompareTag("fireFistsPowerup"))
        {
            FireFists(FireFistsCooldown);
            Destroy(other.GameObject());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        if (Input.GetMouseButtonDown(0) && _currentAttack == AttackType.Melee)
        {
            Attack(_currentAttack);
            int amount = 30;
            GameManager.PlayerAttack(amount, other.GetComponent<Enemy>());
        }
    }
}
