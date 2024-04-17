using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : ScriptableObject
{
    
    public virtual AIState Tick(AICharacterManager aICharacter)
    {
        Debug.Log("WE ARE RUNNING THIS STATE");

        return this;
    }

    protected virtual AIState SwitchState(AICharacterManager aICharacter, AIState newState)
    {
        ResetStateFlags(aICharacter);
        return newState;
    }

    protected virtual void ResetStateFlags(AICharacterManager aICharacter)
    {   
    }
}
