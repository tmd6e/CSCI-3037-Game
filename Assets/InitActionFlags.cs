using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitActionFlags : StateMachineBehaviour
{
    CharacterManager character;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (character == null)
        {
            character = animator.GetComponent<CharacterManager>();
        }
        // Called when action starts
        character.isPerformingAction = true;

        character.canMove = false;
        character.canRotate = false;
        character.applyRootMotion = true;
        character.isJumping = false;
    }
}
