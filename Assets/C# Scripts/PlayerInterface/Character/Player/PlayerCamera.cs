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

    [Header("Lock On")]
    [SerializeField] float lockOnRadius = 5;
    [SerializeField] float minViewableAngle = -50;
    [SerializeField] float maxViewableAngle = 50;
    private Coroutine cameraLockOnHeightRoutine;
    private List<CharacterManager> availableTargets = new List<CharacterManager>();
    public CharacterManager nearestLockOnTarget;
    public CharacterManager leftLockOnTarget;
    public CharacterManager rightLockOnTarget;
    [SerializeField] float lockOnTargetFollowSpeed = 0.2f;
    [SerializeField] float setCameraHeightSpeed = 1;
    [SerializeField] float unlockedCameraHeight = 1.65f;
    [SerializeField] float lockedCameraHeight = 2.0f;

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
        if (player.playerNetworkManager.isLockedOn.Value)
        {
            // This rotates this game object
            Vector3 rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - transform.position;
            rotationDirection.Normalize();
            rotationDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed);

            // This rotates the pivot object
            rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - camPivotTransform.position;
            rotationDirection.Normalize();

            targetRotation = Quaternion.LookRotation(rotationDirection);
            camPivotTransform.transform.rotation = Quaternion.Slerp(camPivotTransform.rotation, targetRotation, lockOnTargetFollowSpeed);

            // Save rotation to look angles to smooth unlocking process
            horizontalLookingAngle = transform.eulerAngles.y;
            verticalLookingAngle = transform.eulerAngles.x;
        }
        // Regular logic
        else
        {
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

    public void HandleLockOn() {
        float shortestDistance = Mathf.Infinity; // Used to choose closest target
        float shortestDistanceLeft = -Mathf.Infinity; // Closest left target
        float shortestDistanceRight = Mathf.Infinity; // Closest right target

        Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.instance.GetCharacterLayers());
        CharacterManager lockOnTarget;
        Vector3 lockOnTargetDirection;
        float distanceFromTarget;
        float viewableAngle;

        for (int i = 0; i < colliders.Length; i++) {
            lockOnTarget = colliders[i].GetComponent<CharacterManager>();
            if (lockOnTarget != null) { 
                // Check if within field of view
                lockOnTargetDirection = lockOnTarget.transform.position - player.transform.position;
                distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                viewableAngle = Vector3.Angle(lockOnTargetDirection, cameraObject.transform.forward);
                
                // Do not lock on to a dead target
                if (lockOnTarget.isDead.Value) {
                    continue;
                }
                // Do not lock on to self
                if (lockOnTarget.transform.root == player.transform.root) {
                    continue;
                }
                // Do not lock on to ally
                if (lockOnTarget.gameObject.layer == gameObject.layer)
                {
                    continue;
                }
                // Do not lock beyond maximum range
                if (distanceFromTarget > lockOnRadius) {
                    continue;
                }
                // Check view angle
                // If target is out of view or blocked by environment, check next potential target
                if (viewableAngle > minViewableAngle && viewableAngle < maxViewableAngle) {
                    RaycastHit hit;

                    // Only check environment layer
                    if (Physics.Linecast(player.playerCombatManager.lockOnTransform.position, 
                        lockOnTarget.characterCombatManager.lockOnTransform.position, 
                        out hit, WorldUtilityManager.instance.GetEnvironmentLayers()))
                    {
                        // We hit something, cannot lock on to this target
                        continue;
                    }
                    else {
                        availableTargets.Add(lockOnTarget);

                    }
                }  
            } 
        }

        // Sort through potential targets and check closest
        for (int k = 0; k < availableTargets.Count; k++)
        {
            if (availableTargets[k] != null)
            {
                distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[k].transform.position);
                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[k];
                }

                // If already locked on, search for nearest left/right targets
                if (player.playerNetworkManager.isLockedOn.Value) {
                    Vector3 relativeEnemyPosition = player.transform.InverseTransformPoint(availableTargets[k].transform.position);
                    var distanceFromLeftTarget = relativeEnemyPosition.x;
                    var distanceFromRightTarget = relativeEnemyPosition.x;

                    if (availableTargets[k] == player.playerCombatManager.currentTarget) {
                        continue;
                    }

                    // Check left side
                    if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceLeft)
                    {
                        shortestDistanceLeft = distanceFromLeftTarget;
                        leftLockOnTarget = availableTargets[k];
                    }
                    // Check right side
                    else if (relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortestDistanceRight)
                    {
                        shortestDistanceRight = distanceFromRightTarget;
                        rightLockOnTarget = availableTargets[k];
                    } 
                }
            }
            else {
                ClearLockOnTargets();
                player.playerNetworkManager.isLockedOn.Value = false;
            }
        }
    }

    public void SetLockCameraHeight() {
        if (cameraLockOnHeightRoutine != null)
        {
            StopCoroutine(cameraLockOnHeightRoutine);
        }

        cameraLockOnHeightRoutine = StartCoroutine(SetCameraHeight());
    }

    public void ClearLockOnTargets() {
        nearestLockOnTarget = null;
        leftLockOnTarget = null;
        rightLockOnTarget = null;
        availableTargets.Clear();
    }

    public IEnumerator WaitThenFindNewTarget() {
        while (player.isPerformingAction) {
            yield return null;
        }

        ClearLockOnTargets();
        HandleLockOn();

        if (nearestLockOnTarget != null) {
            player.playerCombatManager.SetTarget(nearestLockOnTarget);
            player.playerNetworkManager.isLockedOn.Value = true;
        }

        yield return null;
    }

    private IEnumerator SetCameraHeight() {
        float duration = 1;
        float timer = 0;

        Vector3 velocity = Vector3.zero;
        Vector3 newLockedCameraHeight = new Vector3(camPivotTransform.transform.localPosition.x, lockedCameraHeight);
        Vector3 newUnlockedCameraHeight = new Vector3(camPivotTransform.transform.localPosition.x, unlockedCameraHeight);

        while (timer < duration) {
            timer += Time.deltaTime;

            if (player != null) {
                if (player.playerCombatManager.currentTarget != null)
                {
                    camPivotTransform.transform.localPosition = Vector3.SmoothDamp(camPivotTransform.transform.localPosition, newLockedCameraHeight, ref velocity, setCameraHeightSpeed);
                    camPivotTransform.transform.localRotation = Quaternion.Slerp(camPivotTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), lockOnTargetFollowSpeed);
                }
                else {
                    camPivotTransform.transform.localPosition = Vector3.SmoothDamp(camPivotTransform.transform.localPosition, newUnlockedCameraHeight, ref velocity, setCameraHeightSpeed); 
                }
            }

            yield return null;
        
        }

        if (player != null) {
            if (player.playerCombatManager.currentTarget != null)
            {
                camPivotTransform.transform.localPosition = newLockedCameraHeight;
                camPivotTransform.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else {
                camPivotTransform.transform.localPosition = newUnlockedCameraHeight;
            }
        }

        yield return null;
    }
}
