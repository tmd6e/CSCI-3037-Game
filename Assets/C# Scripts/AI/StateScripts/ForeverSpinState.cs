using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "A.I/States/Forever Spin")]
public class ForeverSpinState : AIState
{
    public override AIState Tick(AICharacterManager aICharacter)
    {
        // Simple AI to have an enemy spin in place. This is for a stationary mage.
        aICharacter.aiCharacterLocomotionManager.RotateStationary(aICharacter);
        return this;
    }
}
