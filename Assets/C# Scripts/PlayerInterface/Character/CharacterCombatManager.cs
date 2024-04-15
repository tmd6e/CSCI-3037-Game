using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterCombatManager : NetworkBehaviour
{
    protected CharacterManager character;

    // [Header("Last Attack Animation Performed")]
    // public string lastAttackAnimationPerformed;

    [Header("Attack Target")]
    public CharacterManager currentTarget;

    [Header("Lock On Transform")]
    public Transform lockOnTransform;

    protected virtual void Awake() { 
        character = GetComponent<CharacterManager>();
    }

    public virtual void SetTarget(CharacterManager newTarget)
    {
        if (character.IsOwner) {
            if (newTarget != null && !newTarget.isDead.Value)
            {
                currentTarget = newTarget;
                // Tell network there is a new target and identify them
                character.characterNetworkManager.currentTargetNetworkObjectID.Value = newTarget.GetComponent<NetworkObject>().NetworkObjectId;
            }
            else { 
                currentTarget = null;
            }
        }
    }

    public void EnableIsInvulnerable() {
        if (character.IsOwner)
        {
            character.characterNetworkManager.isInvulnerable.Value = true;
        
        }
    }
    public void DisableIsInvulnerable()
    {
        if (character.IsOwner)
        {
            character.characterNetworkManager.isInvulnerable.Value = false;
        }
    }
}
