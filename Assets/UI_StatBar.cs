using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatBar : MonoBehaviour
{
    private Slider slider;
    // Scales bar size depending on stat

    // Secondary flash bar for polish

    protected virtual void Awake() {
        if (slider == null) {
            slider = GetComponent<Slider>();
        }
    }

    public virtual void SetStat(int newValue) {
        slider.value = newValue;
    }

    public virtual void SetMaxStat(int maxValue) {
        slider.maxValue = maxValue;
        slider.value = maxValue;
    }
}
