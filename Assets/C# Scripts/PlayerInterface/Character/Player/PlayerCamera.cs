using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    public PlayerManager player;
    public Camera cameraObject;
    [SerializeField] Transform camPivotTransform;

    // Camera performance settings
    [Header("Camera Settings")]    
    private float cameraSmoothSpeed = 1;
    [SerializeField] float horizontalRotationSpeed = 220;
    [SerializeField] float verticalRotationSpeed = 220;
    [SerializeField] float minPivot = -30; // Lowest angle the player can look down
    [SerializeField] float maxPivot = 60; // Highest angle the player can look up
    [SerializeField] float collisionRadius = 0.2f;
    [SerializeField] LayerMask collideWithLayers;

    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    private Vector3 cameraObjectPos;
    [SerializeField] float horizontalLookingAngle;
    [SerializeField] float verticalLookingAngle;
    private float defaultCameraPos; // For camera collisions
    private float targetCameraPos;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { 
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        defaultCameraPos = cameraObject.transform.localPosition.z;
    }

    public void HandleAllCameraActions() {
        if (player != null)
        {
            // Follow the player
            FollowTarget();
            // Rotate around the player
            HandleRotations();
            // Collide with environment to prevent weird clipping
            CollisionControl();
        }
        
    }
    private void FollowTarget() {
        Vector3 targetCameraPos = Vector3.SmoothDamp(
            transform.position, 
            player.transform.position, 
            ref cameraVelocity, 
            cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPos;
    }
    private void HandleRotations() { 
        // Lock on logic
        // Regular logic
        // Horizontal rotation
        horizontalLookingAngle += (PlayerInputManager.instance.camHorizontalInput * horizontalRotationSpeed) * Time.deltaTime;
        // Vertical rotation
        verticalLookingAngle -= (PlayerInputManager.instance.camVerticalInput * verticalRotationSpeed) * Time.deltaTime;
        // Limit viewing angle based on defined constraints
        verticalLookingAngle = Mathf.Clamp(verticalLookingAngle, minPivot, maxPivot);

        Vector3 camRotation = Vector3.zero;
        Quaternion targetRotation;

        // Rotates the gameObject left and right
        camRotation.y = horizontalLookingAngle;
        targetRotation = Quaternion.Euler(camRotation);
        transform.rotation = targetRotation;

        // Rotates the gameObject up and down
        camRotation = Vector3.zero;
        camRotation.x = verticalLookingAngle;
        targetRotation = Quaternion.Euler(camRotation);
        camPivotTransform.localRotation = targetRotation;

    }

    private void CollisionControl() {
        targetCameraPos = defaultCameraPos;
        RaycastHit hit;
        // Direction for collision check
        Vector3 direction = cameraObject.transform.position - camPivotTransform.position;
        direction.Normalize();

        if (Physics.SphereCast(camPivotTransform.position, collisionRadius, direction, out hit, Mathf.Abs(targetCameraPos), collideWithLayers)){
            float distanceFromCollider = Vector3.Distance(camPivotTransform.position, hit.point);
            targetCameraPos = -(distanceFromCollider - collisionRadius);
        }

        if (Mathf.Abs(targetCameraPos) < collisionRadius)
        {
            targetCameraPos = -collisionRadius;
        }

        cameraObjectPos.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraPos, 0.2f);
        cameraObject.transform.localPosition = cameraObjectPos;
    }
}
