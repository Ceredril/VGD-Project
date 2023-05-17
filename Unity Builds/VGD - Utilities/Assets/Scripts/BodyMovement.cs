using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

public class BodyMovement : MonoBehaviour
{
    private bool _canHit = true;

    //Object references
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform characterCamera;
    [HideInInspector] public Animator animator;


    //Movement variables
    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float sprintSpeed = 4f;
    [SerializeField] private float jumpForce = 4f;
    private float _speedOffset;

    //Gravity variables
    private readonly float _gravity = 9.81f;
    private float _verticalSpeed;

    //Public instance
    public static BodyMovement instance;

    private void Awake() => instance = this;

    private void Start()
    {
        instance.characterController = FindObjectOfType<CharacterController>();
        instance.characterCamera = FindObjectOfType<Camera>().transform;
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        instance.Move();

        if (GameManager.GameIsOver) gameObject.SetActive(false);
    }


    public void Move()
    {
        if (GameManager.GameIsOver) return;
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
        if (Input.GetKeyDown(KeyCode.Space) && instance.characterController.isGrounded) {
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
        instance.characterController.Move(movementDirection * Time.deltaTime);

        //Passing the horizontal and vertical value to the animator
        animator.SetFloat("hInput", keyboardInputHorizontal);
        animator.SetFloat("vInput", keyboardInputVertical);


        // Setting the jump and hit states based on keystroke
        if (Input.GetKeyUp(KeyCode.Space)) animator.SetBool("jump", false);
        if (Input.GetMouseButtonDown(0) && _canHit) { animator.SetTrigger("hook");  StartCoroutine(Wait()); }

        // Switch between states based on whether you are running or walking
        if (_speedOffset == sprintSpeed) animator.SetBool("running", true);
        else animator.SetBool("running", false);

    }

    private IEnumerator Wait() {
        _canHit = false;
        yield return new WaitForSeconds(1);
        _canHit = true;

    }
}