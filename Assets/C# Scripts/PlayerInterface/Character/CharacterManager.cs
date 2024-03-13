using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterManager : NetworkBehaviour
{
    [Header("Status")]
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;
    
    [HideInInspector] public CharacterNetworkManager characterNetworkManager;
    [HideInInspector] public CharacterEffectsManager characterEffectsManager;
    [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;

    [Header("Action Flags")]
    public bool canRotate = true;
    public bool canMove = true;
    public bool applyRootMotion = false;
    public bool isPerformingAction = false;

    

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
        characterEffectsManager = GetComponent<CharacterEffectsManager>();
        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
    }

    protected virtual void Update() {
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

    public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false) {
        if (IsOwner) {
            characterNetworkManager.currentHealth.Value = 0;
            isDead.Value = true;

            // Reset flags

            // If not grounded, play aerial death

            if (!manuallySelectDeathAnimation) {
                characterAnimatorManager.PlayTargetActionAnimation("Die", true);
            }
        }

        // Play SFX
        yield return new WaitForSeconds(5);
        // Award players with currency that can buy powerups
    
    }

    public virtual void ReviveCharacter() { 
    }
}
