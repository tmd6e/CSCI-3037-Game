using System.Collections;
using UnityEngine;
public class MovementScript : MonoBehaviour
{
    //These variables dictate certain attributes to consider when the player is moving.
    public float walkingSpeed = 5.0f;
    public float sprintSpeed = 10.0f;
    public float sensitivity = 2.0f;
    public float gravityForce = 9.81f;
    //Reference game objects
    public GameObject playerCamera;
    public Animator animator;

    private CharacterController controller;
    private float rotationX = 0;
    int movementInt = 0;
    //Flags
    private bool isSprinting = false;
    private bool isJumping = false;

    //Checks ground for jumping
    private GroundCheck groundCheck;
    //Player game object
    public GameObject characterObject;
    //Player's current speed
    private float currentSpeed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        groundCheck = characterObject.GetComponent<GroundCheck>();
    }

    void Update()
    {
        // Get input for movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool isMoving = (horizontalInput != 0 || verticalInput != 0);

        // Calculate the movement direction based on the input
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
        moveDirection = transform.TransformDirection(moveDirection);

        // Check if the Shift key is held down
        bool isSprintKeyHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Determine the current movement speed
        if (isSprintKeyHeld && Input.GetKey(KeyCode.E))
        {
            currentSpeed = sprintSpeed * 10;
        }
        else if (isSprintKeyHeld)
        {
            currentSpeed = sprintSpeed;
        }
        else {
            currentSpeed = walkingSpeed;
        }

        // Move the character
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);


        // Handle mouse rotation for the camera view
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotationX -= mouseY * .1f;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.Rotate(Vector3.up * mouseX);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            // Update the sprint state
            isSprinting = isSprintKeyHeld;
        }
    }
