using System;
using UnityEngine;

public class BodyMovement : MonoBehaviour
{
    //Object references
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform characterCamera;
    
    //Movement variables
    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float sprintSpeed = 4f;
    [SerializeField] private float jumpForce = 4f;
    private float _speedOffset;

    //Gravity variables
    private readonly float _gravity = 9.81f;
    private float _verticalSpeed;

    //Camera variables
    private float _turnSmoothTime;
    private readonly float _turnSmoothVelocity = 0.001f;
    
    //Public instance
    public static BodyMovement instance;

    private void Awake() => instance = this;

    private void Start()
    {
        instance.characterController = FindObjectOfType<CharacterController>();
        instance.characterCamera = FindObjectOfType<Camera>().transform;
    }

    private void Update()
    {
        instance.Move();
        if(GameManager.GameIsOver)this.gameObject.SetActive(false);
    }


    public void Move()
    {
        if (GameManager.GameIsOver) return;
        //Get keyboard inputs
        float keyboardInputHorizontal = Input.GetAxis("Horizontal");
        float keyboardInputVertical = Input.GetAxis("Vertical"); 
        Vector3 movementDirection = new Vector3(keyboardInputHorizontal, 0f, keyboardInputVertical).normalized;

        //While the character's moving, match the moving direction to the direction at which the camera is aiming
        if (movementDirection.magnitude >= 0.1f)
        { 
            float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg + characterCamera.eulerAngles.y; 
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothTime, _turnSmoothVelocity);
            instance.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            movementDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }
        //If user presses Spacebar and the character is grounded, apply jump force. Otherwise let it fall :D
        if (Input.GetKeyDown(KeyCode.Space) && instance.characterController.isGrounded) _verticalSpeed = jumpForce;
        else _verticalSpeed -= _gravity * Time.deltaTime;
        
        //If the user is holding Shift, assign sprintSpeed to the speedOffset. 
        if (Input.GetKey(KeyCode.LeftShift)) _speedOffset = sprintSpeed;
        else _speedOffset = walkingSpeed;

        //Update the movement vector with the new values
        movementDirection.y = _verticalSpeed; movementDirection.x *= _speedOffset;
        movementDirection.z *= _speedOffset;
        
        //Apply the movement to the character
        instance.characterController.Move(movementDirection * Time.deltaTime);
    }
}