using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "A.I/States/Toughness Broken")]

public class ToughnessBrokenState : AIState
{
    public override AIState Tick(AICharacterManager aICharacter)
    {
        if (aICharacter.aiCharacterNetworkManager.currentToughness.Value <= 0)
        {
            return this;
        }
        else
        {
            return SwitchState(aICharacter, aICharacter.idle);
        }
    }
}
