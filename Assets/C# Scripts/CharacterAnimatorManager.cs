using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterAnimatorManager : MonoBehaviour
{
    CharacterManager character;

    float vertical;
    float horizontal;

    protected virtual void Awake() { 
        character = GetComponent<CharacterManager>();
    }
    public void UpdateAnimatorMovementParameters(float horizontal, float vertical) {
        character.animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
        character.animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);
    }

    public virtual void PlayTargetActionAnimation(
        string target, 
        bool isPerformingAction, 
        bool applyRootMotion = true, 
        bool canRotate = false, 
        bool canMove = false) {
        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(target, 0.2f);
        // Controls character action
        character.isPerformingAction = isPerformingAction;
        character.canRotate = canRotate;
        character.canMove = canMove;

        // Network the animation and sync for other players
        character.characterNetworkManager.NotifyServerOfActionServerRpc(NetworkManager.Singleton.LocalClientId, target, applyRootMotion);

    }
}
