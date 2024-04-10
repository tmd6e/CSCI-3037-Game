using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterManager : NetworkBehaviour
{
    [Header("Status")]
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isBroken = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;
    
    [HideInInspector] public CharacterNetworkManager characterNetworkManager;
    [HideInInspector] public CharacterEffectsManager characterEffectsManager;
    [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
    [HideInInspector] public CharacterCombatManager characterCombatManager;
    [HideInInspector] public CharacterLocomotionManager characterLocomotionManager;

    [Header("Character Group")]
    public CharacterGroup characterGroup;

    [Header("Action Flags")]
    public bool canRotate = true;
    public bool canMove = true;
    public bool applyRootMotion = false;
    public bool isPerformingAction = false;
    public bool isJumping = false;
    public bool isGrounded = true;


    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
        characterEffectsManager = GetComponent<CharacterEffectsManager>();
        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
        characterCombatManager = GetComponent<CharacterCombatManager>();
        characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
    }

    protected virtual void Update() {
        animator.SetBool("isGrounded", isGrounded);
        if (IsOwner)
        {
            characterNetworkManager.networkPos.Value = transform.position;
            characterNetworkManager.networkRot.Value = transform.rotation;
        }
        else {
            transform.position = Vector3.SmoothDamp
                (transform.position,
                characterNetworkManager.networkPos.Value, 
                ref characterNetworkManager.networkPosVelocity,
                characterNetworkManager.networkPositionSmoothTime);

            transform.rotation = Quaternion.Slerp
                (transform.rotation, 
                characterNetworkManager.networkRot.Value, 
                characterNetworkManager.networkRotationSmoothTime);
        }
    }

    protected virtual void LateUpdate() { 
    
    }

    protected virtual void FixedUpdate()
    {

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        animator.SetBool("isMoving", characterNetworkManager.isMoving.Value);
        characterNetworkManager.OnIsActiveChanged(false, characterNetworkManager.isActive.Value);
        
        characterNetworkManager.isMoving.OnValueChanged += characterNetworkManager.OnIsMovingChanged;
        characterNetworkManager.isActive.OnValueChanged += characterNetworkManager.OnIsActiveChanged;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        characterNetworkManager.isMoving.OnValueChanged -= characterNetworkManager.OnIsMovingChanged;
    }

    public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false) {
        if (IsOwner) {
            characterNetworkManager.currentHealth.Value = 0;
            // Reset flags
            isDead.Value = true;
            canMove = false;
            canRotate = false;

            

            // If not grounded, play aerial death

            if (!manuallySelectDeathAnimation) {
                characterAnimatorManager.PlayTargetActionAnimation("Die", true);
            }
        }

        // Play SFX
        yield return new WaitForSeconds(5);
        // Award players powerups
    
    }
    public virtual IEnumerator ProcessBreakEvent()
    {
        if (IsOwner)
        {
            characterNetworkManager.currentToughness.Value = 0;

            // Reset flags
            isBroken.Value = true;
            canMove = false;
            canRotate = false;

            characterAnimatorManager.PlayTargetActionAnimation("ToughnessBrokenEnter", true);
        }

        yield return new WaitForSeconds(5);
    }

    public virtual void ReviveCharacter() { 
    }
}
