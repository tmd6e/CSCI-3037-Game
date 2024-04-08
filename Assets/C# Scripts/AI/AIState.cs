using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : ScriptableObject
{
    
    public virtual AIState Tick(AICharacterManager aICharacter)
    {
        Debug.Log("WE ARE RUNNING THIS STATE");

        //DO SOME LOGIC TO FIND THE PLAYER

        // IF WE HAVE FOUND THE PLAYER, RETURN THE PURSUE TARGET STATE INSTEAD

        // IF WE HAVE NOT FOUND THE PLAYER, CONTINUE TO RETURN IDLE STATE
        return this;
    }

    protected virtual AIState SwitchState(AICharacterManager aICharacter, AIState newState)
    {
        ResetStateFlags(aICharacter);
        return newState;
    }

    protected virtual void ResetStateFlags(AICharacterManager aICharacter)
    {
        // RESET ANY STATE FLAGS HERE SO WHEN YOU RETURN TO THE STATE, THEY ARE BLANK ONCE AGAIN        
    }
}
