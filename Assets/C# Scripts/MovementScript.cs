using System.Collections;
using UnityEngine;
public class MovementScript : MonoBehaviour
{
    //These variables dictate certain attributes to consider when the player is moving.
    public float walkingSpeed = 5.0f;
    public float sprintSpeed = 10.0f;
    public float sensitivity = 2.0f;
    public int jumpHeight = 5;
    public bool isDead = false;
    //Reference game objects
    public GameObject playerCamera;
    public Animator animator;

    private CharacterController controller;
    private float rotationX = 0;
    int movementInt = 0;
    //Flags
    private bool isSprinting = false;
    private bool isJumping = false;
    //Player game object
    public GameObject characterObject;
    //Player's current speed
    private float currentSpeed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //As long as the player is not dead
        if (!isDead)
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

                if (Input.GetKeyDown(KeyCode.Space)){
                }

                // Determine the current movement speed
                if (isSprintKeyHeld && Input.GetKey(KeyCode.E))
                {
                    currentSpeed = sprintSpeed * 10;
                }
                else if (isSprintKeyHeld)
                {
                    currentSpeed = sprintSpeed;
                }
                else
                {
                    currentSpeed = walkingSpeed;
                }

                // Move the character
                controller.SimpleMove(moveDirection * currentSpeed);


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
    }
