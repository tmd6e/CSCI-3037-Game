using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Stamina Regeneration")]
    [SerializeField] float staminaRegenRate = 5;
    private float staminaRegenTimer = 0;
    private float staminaTickTimer = 0;
    [SerializeField] float regenDelay = 2;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Start()
    {
    }

    public int CalculateHealth(float healthMultiplier)
    {
        float health = 0;

        health = 150 * healthMultiplier;

        return Mathf.RoundToInt(health);
    }
    public int CalculateStamina(float staminaMultiplier)
    {
        float stamina = 0;

        stamina = 150 * staminaMultiplier;

        return Mathf.RoundToInt(stamina);
    }
    public virtual void RegenerateStamina()
    {
        if (!character.IsOwner)
        {
            return;
        }
        if (character.characterNetworkManager.isSprinting.Value)
        {
            return;
        }
        if (character.isPerformingAction)
        {
            return;
        }

        staminaRegenTimer += Time.deltaTime;

        if (staminaRegenTimer > regenDelay)
        {
            if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
            {
                staminaTickTimer = staminaTickTimer + Time.deltaTime;

                if (staminaTickTimer >= 0.1)
                {
                    staminaTickTimer = 0;
                    character.characterNetworkManager.currentStamina.Value += staminaRegenRate;
                }
            }
        }
    }
    public virtual void ResetStaminaRegenTimer(float previousValue, float currentValue) {
        if (currentValue < previousValue)
        {
            staminaRegenTimer = 0;
        }
    }
}
