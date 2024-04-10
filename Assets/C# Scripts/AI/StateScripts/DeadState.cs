using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "A.I/States/Dead")]

public class DeadState : AIState
{
    public override AIState Tick(AICharacterManager aICharacter) {
        if (aICharacter.isDead.Value)
        {
            return this;
        }
        else {
            return SwitchState(aICharacter, aICharacter.idle);
        }
    }
}
