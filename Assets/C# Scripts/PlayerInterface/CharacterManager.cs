using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterManager : NetworkBehaviour
{
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;
    
    [HideInInspector] public CharacterNetworkManager characterNetworkManager;

    [Header("Action Flags")]
    public bool isPerformingAction = false;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
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
}
