using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : CharacterCombatManager
{
    PlayerManager player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }

    //[Header("Flags")]
    // public bool canComboWithMainHandWeapon = false;
    // public bool canComboWithOffHandWeapon = false;
    
    public override void SetTarget(CharacterManager newTarget)
    {
        base.SetTarget(newTarget);

        if (player.IsOwner) { 
            PlayerCamera.instance.SetLockCameraHeight(); ;
        }
    }

    /*public void EnableCanDoCombo() 
    {
        if (player.playerNetworkManager.isUsingRightHand.Value)
        {
            canComboWithMainHandWeapon = true;
        } 
        else
        {
            // Enable off hand combo
        }
            
    }

    public void DisableCanDoCombo() 
    {
        canComboWithMainHandWeapon = false;
        // canComboWithOffHandWeapon = false;
    }*/
}
