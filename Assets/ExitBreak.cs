using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBreak : StateMachineBehaviour
{
    CharacterManager character;
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (character == null)
        {
            character = animator.GetComponent<CharacterManager>();
        }
        character.characterNetworkManager.currentToughness.Value = character.characterNetworkManager.maxToughness.Value;
        character.isBroken.Value = false;
    }
}
