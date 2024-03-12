using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterNetworkManager : NetworkBehaviour
{
    CharacterManager character;
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

    [Header("Flags")]
    public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    [Header("Stats")]
    public NetworkVariable<float> staminaMultiplier = new NetworkVariable<float>(1.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> healthMultiplier = new NetworkVariable<float>(1.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    [Header("Resources")]
    public NetworkVariable<float> currentHealth = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> maxHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> currentStamina = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> maxStamina = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
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
