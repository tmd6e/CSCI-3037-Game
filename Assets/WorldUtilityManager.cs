using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUtilityManager : MonoBehaviour
{
    public static WorldUtilityManager instance;

    [Header("Layers")]
    [SerializeField] LayerMask characterLayers;
    [SerializeField] LayerMask environmentLayers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { 
            Destroy(gameObject);
        }
    }

    public LayerMask GetCharacterLayers() {
        return characterLayers;
    }
    public LayerMask GetEnvironmentLayers()
    {
        return environmentLayers;
    }

    public bool CanIDamageThisTarget(CharacterGroup attacker, CharacterGroup target)
    {
        if (attacker == CharacterGroup.Team01)
        {
            switch (target)
            {
                case CharacterGroup.Team01: return false;
                case CharacterGroup.Team02: return true;
                default:
                    break;
            }
        }
        else if (attacker == CharacterGroup.Team02)
        {
            switch (target)
            {
                case CharacterGroup.Team01: return true;
                case CharacterGroup.Team02: return false;
                default:
                    break;
            }
        }

        return false;
    }
}
