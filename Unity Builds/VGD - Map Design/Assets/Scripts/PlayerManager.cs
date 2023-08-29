using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //Object references
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform characterCamera;
    public static Animator Animator;

    //Movement variables
    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float sprintSpeed = 4f;
    [SerializeField] private float jumpForce = 4f;
    private float _speedOffset;
    //Gravity variables
    private readonly float _gravity = 9.81f;
    private float _verticalSpeed;


    
    //Movement functions
    private void Move()
    {
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
            Animator.SetBool("jump", true);
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
        Animator.SetFloat("hInput", keyboardInputHorizontal);
        Animator.SetFloat("vInput", keyboardInputVertical);


        // Setting the jump and hit states based on keystroke
        if (Input.GetKeyUp(KeyCode.Space)) Animator.SetBool("jump", false);

        // Switch between states based on whether you are running or walking
        if (_speedOffset == sprintSpeed) Animator.SetBool("running", true);
        else Animator.SetBool("running", false);

    }

    // Start is called before the first frame update
    private void Awake()
    {
        //Components
        characterController = GetComponentInChildren<CharacterController>();
        characterCamera = GetComponentInChildren<Camera>().transform;
        Animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    { 
        Move();
    }

}
