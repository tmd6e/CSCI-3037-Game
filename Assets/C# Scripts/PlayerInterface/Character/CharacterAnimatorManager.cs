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
        float snappedHorizontalAmount = horizontalMovement;
        float snappedVerticalAmount = verticalMovement;

        if (horizontalMovement > 0 && horizontalMovement <= 0.5f)
        {
            snappedHorizontalAmount = 0.5f;
        }
        else if (horizontalMovement > 0.5f && horizontalMovement <= 1)
        {
            snappedHorizontalAmount = 1;
        }
        else if (horizontalMovement < 0 && horizontalMovement >= -0.5f)
        {
            snappedHorizontalAmount = -0.5f;
        }
        else if (horizontalMovement < -0.5f && horizontalMovement >= -1)
        {
            snappedHorizontalAmount = -1;
        }
        else {
            snappedHorizontalAmount = 0;
        }

        if (verticalMovement > 0 && verticalMovement <= 0.5f) {
            snappedVerticalAmount = 0.5f;
        }
        else if (verticalMovement > 0.5f && verticalMovement <= 1) {
            snappedVerticalAmount = 1;
        }
        else if (verticalMovement < 0 && verticalMovement >= -0.5f) {
            snappedVerticalAmount = -0.5f;
        }
        else if (verticalMovement < -0.5f && verticalMovement >= -1) {
  snappedVerticalAmount = -1;
        }
        else {          
            snappedVerticalAmount = 0;
        }
        if (isSprinting) {
            snappedVerticalAmount = 2;
        }
        
        character.animator.SetFloat(horizontal, snappedHorizontalAmount, 0.1f, Time.deltaTime);
        character.animator.SetFloat(vertical, snappedVerticalAmount, 0.1f, Time.deltaTime);
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
