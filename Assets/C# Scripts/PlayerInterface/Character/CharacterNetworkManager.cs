using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterNetworkManager : NetworkBehaviour
{
    CharacterManager character;

    [Header("Active")]
    public NetworkVariable<bool> isActive = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Transform")]
    public NetworkVariable<Vector3> networkPos = new NetworkVariable<Vector3>(Vector3.zero,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<Quaternion> networkRot = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public Vector3 networkPosVelocity;
    public float networkPositionSmoothTime = 0.1f;
    public float networkRotationSmoothTime = 0.1f;

    [Header("Animations")]
    public NetworkVariable<float> horizontal = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> vertical = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> networkMoveAmount = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Target")]
    public NetworkVariable<ulong> currentTargetNetworkObjectID = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Flags")]
    public NetworkVariable<bool> isLockedOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isMoving = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isInvulnerable = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Stats")]
    public NetworkVariable<float> staminaMultiplier = new NetworkVariable<float>(1.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> healthMultiplier = new NetworkVariable<float>(1.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> attackMultiplier = new NetworkVariable<float>(1.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> breakMultiplier = new NetworkVariable<float>(1.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> attackSpeed = new NetworkVariable<float>(1.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Resources")]
    public NetworkVariable<float> currentHealth = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> maxHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> currentStamina = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> maxStamina = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    [Header("AI Exclusive")]
    public NetworkVariable<float> currentToughness = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> maxToughness = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> canBeBroken = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public void CheckToughness(float oldValue, float newValue) {
        if (currentToughness.Value <= 0 && !character.isBroken.Value) { 
            StartCoroutine(character.ProcessBreakEvent());
        }
    }

    public void CheckHP(float oldValue, float newValue) {
        if (currentHealth.Value <= 0) {
            StartCoroutine(character.ProcessDeathEvent());
        }

        // Prevents overhealing
        if (character.IsOwner) {
            if (currentHealth.Value > maxHealth.Value) { 
                currentHealth.Value = maxHealth.Value;
            }
        }
    }

    public void OnLockOnTargetIDChange(ulong oldID, ulong newID) {
        if (!IsOwner) {
            character.characterCombatManager.currentTarget = NetworkManager.Singleton.SpawnManager.SpawnedObjects[newID].gameObject.GetComponent<CharacterManager>();

        }
    }

    public void OnIsLockedOnChanged(bool oldValue, bool isLockedOn) {
        if (!isLockedOn) {
            character.characterCombatManager.currentTarget = null;
        }
    }

    public void OnIsMovingChanged(bool oldStatus, bool newStatus)
    {
        character.animator.SetBool("isMoving", isMoving.Value);
    }

    public virtual void OnIsActiveChanged(bool oldValue, bool newValue)
    {
        gameObject.SetActive(isActive.Value);
    }
    [ServerRpc]
    public void NotifyServerOfActionServerRpc(ulong clientID, string animationID, bool applyRootMotion) {
        // If this instance is the host, play the action for all clients
        if (IsServer) { 
            PlayActionForClientsClientRpc(clientID, animationID, applyRootMotion);
        }
    }

    [ClientRpc]
    public void PlayActionForClientsClientRpc(ulong clientID, string animationID, bool applyRootMotion) {
        // This prevents repeat animations over the network
        if (clientID != NetworkManager.Singleton.LocalClientId) {
            PerformActionFromServer(animationID, applyRootMotion);
        }
    }

    private void PerformActionFromServer(string animationID, bool applyRootMotion) {
        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(animationID, 0.2f);
    }
}
