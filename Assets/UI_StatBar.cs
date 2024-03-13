using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatBar : MonoBehaviour
{
    private Slider slider;
    private RectTransform rectTransform;
    // Scales bar size depending on stat
    [Header("Bar Options")]
    [SerializeField] protected bool scaleBarLengthWithMultipliers = true;
    [SerializeField] protected float widthScaleMultiplier = 1;
    // Secondary flash bar for polish

    protected virtual void Awake() {
        if (slider == null) {
            slider = GetComponent<Slider>();
            rectTransform = GetComponent<RectTransform>();
        }
    }

    public virtual void SetStat(int newValue) {
        slider.value = newValue;
    }

    public virtual void SetMaxStat(int maxValue) {
        slider.maxValue = maxValue;
        slider.value = maxValue;

        if (scaleBarLengthWithMultipliers) {
            rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, rectTransform.sizeDelta.y);
            PlayerUIManager.instance.playerUIHUDManager.RefreshHUD();
        }
    }
}
