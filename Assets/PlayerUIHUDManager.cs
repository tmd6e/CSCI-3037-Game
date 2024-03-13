using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIHUDManager : MonoBehaviour
{
    [SerializeField] UI_StatBar healthBar;
    [SerializeField] UI_StatBar staminaBar;

    public void RefreshHUD() { 
        healthBar.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(true);
        staminaBar.gameObject.SetActive(false);
        staminaBar.gameObject.SetActive(true);
    }

    public void SetNewHealth(float oldValue, float newValue)
    {
        healthBar.SetStat(Mathf.RoundToInt(newValue));
    }

    public void SetMaxHealth(int maxHealth)
    {
        healthBar.SetMaxStat(maxHealth);
    }

    public void SetNewStamina(float oldValue, float newValue) {
        staminaBar.SetStat(Mathf.RoundToInt(newValue));
    }

    public void SetMaxStamina(int maxStamina) { 
        staminaBar.SetMaxStat(maxStamina);
    }
}
