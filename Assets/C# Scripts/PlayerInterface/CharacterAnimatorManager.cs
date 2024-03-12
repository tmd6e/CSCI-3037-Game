using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterAnimatorManager : MonoBehaviour
{
    CharacterManager character;

    int vertical;
    int horizontal;

    protected virtual void Awake() { 
        character = GetComponent<CharacterManager>();

        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }

    public void UpdateAnimatorMovementParameters(float horizontalMovement, float verticalMovement, bool isSprinting) {
        float horizontalAmount = horizontalMovement;
        float verticalAmount = verticalMovement;

        if (isSprinting) {
            verticalAmount = 2;
        }

        character.animator.SetFloat(horizontal, horizontalAmount, 0.1f, Time.deltaTime);
        character.animator.SetFloat(vertical, verticalAmount, 0.1f, Time.deltaTime);
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
