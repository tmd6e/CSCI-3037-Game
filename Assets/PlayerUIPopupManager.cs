using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUIPopupManager : MonoBehaviour
{
    [Header("YOU DIED Pop Up")]
    [SerializeField] GameObject youDiedPopUpGameObject;
    [SerializeField] TextMeshProUGUI youDiedPopUpBackgroundText;
    [SerializeField] TextMeshProUGUI youDiedPopupText;
    [SerializeField] CanvasGroup youDiedPopupCanvasGroup;

    public void SendYouDiedPopup() { 
        // Activate post processing effects

        youDiedPopUpGameObject.SetActive(true);
        youDiedPopUpBackgroundText.characterSpacing = 0;
        // Stretch the pop up
        StartCoroutine(StretchPopUpTextOverTime(youDiedPopUpBackgroundText, 8, 8.32f));
        // Fade in the pop up
        StartCoroutine(FadeInPopUpOverTime(youDiedPopupCanvasGroup, 5));
        // Wait, then fade out
        StartCoroutine(WaitThenFadeOutPopUpOverTime(youDiedPopupCanvasGroup, 2, 5));
    }

    private IEnumerator StretchPopUpTextOverTime(TextMeshProUGUI text, float duration, float stretchAmount) {
        if (duration > 0) {
            text.characterSpacing = 0; // Resets character spacing
            float timer = 0;

            yield return null;

            while (timer < duration) {
                timer = timer + Time.deltaTime;
                text.characterSpacing = Mathf.Lerp(text.characterSpacing, stretchAmount, duration * (Time.deltaTime / 20));
                yield return null;
            }
        }
    }

    private IEnumerator FadeInPopUpOverTime(CanvasGroup canvas, float duration) {
        if (duration > 0) {
            canvas.alpha = 0;
            float timer = 0;

            yield return null;

            while (timer < duration) {
                timer = timer + Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 1, duration * Time.deltaTime);
                yield return null;
            }

            canvas.alpha = 1;

            yield return null;
        }
    }

    private IEnumerator WaitThenFadeOutPopUpOverTime(CanvasGroup canvas, float duration, float delay) {
        if (duration > 0)
        {
            while (delay > 0) {
                delay = delay - Time.deltaTime;
                yield return null;
            }

            canvas.alpha = 1;
            float timer = 0;

            yield return null;

            while (timer < duration)
            {
                timer = timer + Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 0, duration * Time.deltaTime);
                yield return null;
            }

            canvas.alpha = 0;

            yield return null;
        }
    }
}
