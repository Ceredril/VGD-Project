using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    //Object references
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform characterCamera;
    private Animator animator;
    
    //Stamina parameters
    private bool canSprint = true;
    [SerializeField] private float StaminaUseMultiplier = 4;
    [SerializeField] private float TimeBeforeStaminaRegenStarts = 1.5f;
    private float StaminaValueIncrement = 2;
    private float StaminaTimeIncrement = 0.1f;
    //public static float CurrentStamina;
    private Coroutine regeneratingStamina;
    public static Action<float> OnStaminaChange;
    public static bool normalMode = true;
    
    //Movement variables
    public static float walkingSpeed = 2f;
    public static float sprintSpeed = 4f;
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float JumpMultiplier = 10;
    private bool isJumping = false;
    private float _speedOffset;
    //Gravity variables
    private readonly float _gravity = 9.81f;
    private float _verticalSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        PlayerManager.CurrentStamina = PlayerManager.MaxStamina;
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        characterCamera = GameObject.Find("Main Camera").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.GameIsPaused)Move();   
    }
    
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
        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded && PlayerManager.CurrentStamina >= JumpMultiplier)
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
            if (isJumping && normalMode)   // that means it's running and jumping 
                PlayerManager.CurrentStamina -= (JumpMultiplier + StaminaUseMultiplier) * Time.deltaTime;
            else if (normalMode)           // here is just running 
                PlayerManager.CurrentStamina -= StaminaUseMultiplier * Time.deltaTime;
            if (PlayerManager.CurrentStamina < 0)
                PlayerManager.CurrentStamina = 0;
            OnStaminaChange?.Invoke(PlayerManager.CurrentStamina);
            if (PlayerManager.CurrentStamina <= 0)
            {
                canSprint = false;
            }
        }
        else if (_speedOffset != sprintSpeed && isJumping && normalMode)  // this second if, manage the stamina when the character is walking or standing still but is jumping
        {
            if (regeneratingStamina != null)
            {
                StopCoroutine(regeneratingStamina);
                regeneratingStamina = null;
            }
            PlayerManager.CurrentStamina -= JumpMultiplier * Time.deltaTime;
            if (PlayerManager.CurrentStamina < 0)
                PlayerManager.CurrentStamina = 0;
            OnStaminaChange?.Invoke(PlayerManager.CurrentStamina);
            if (PlayerManager.CurrentStamina <= 0)
            {
                canSprint = false;
            }
        }
        else if (_speedOffset != sprintSpeed && PlayerManager.CurrentStamina < PlayerManager.MaxStamina && regeneratingStamina == null)// when the character is still,it begin to regenerate stamina
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
        while (PlayerManager.CurrentStamina < PlayerManager.MaxStamina)
        {
            if (PlayerManager.CurrentStamina > 0)
            {
                canSprint = true;
                //canJump = true;
            }
            PlayerManager.CurrentStamina += StaminaValueIncrement;
            if (PlayerManager.CurrentStamina > PlayerManager.MaxStamina)
                PlayerManager.CurrentStamina = PlayerManager.MaxStamina;
            PlayerManager.CurrentStamina -= StaminaUseMultiplier * Time.deltaTime;
            yield return timeToWait;
        }
        regeneratingStamina = null;
    }
}
