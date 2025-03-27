using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CircleInteraction : MonoBehaviour
{
    public CanvasGroup buttonGroup; // 對應按鈕的 Canvas Group
    public Text[] buttonTexts; // 按鈕上的文字
    public float fadeDuration = 0.5f; // 淡入淡出的時間

    private bool isPlayerInside = false;

    void Start()
    {
        // 確保按鈕一開始是隱藏的
        buttonGroup.alpha = 0;
        buttonGroup.interactable = false;
        buttonGroup.blocksRaycasts = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            StopAllCoroutines();
            StartCoroutine(FadeCanvasGroup(buttonGroup, 1)); // 淡入
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            StopAllCoroutines();
            StartCoroutine(FadeCanvasGroup(buttonGroup, 0)); // 淡出
        }
    }

    public void UpdateButtonText(int buttonIndex, string newText)
    {
        if (buttonIndex >= 0 && buttonIndex < buttonTexts.Length)
        {
            buttonTexts[buttonIndex].text = newText;
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        // 控制交互和射線
        canvasGroup.interactable = targetAlpha > 0;
        canvasGroup.blocksRaycasts = targetAlpha > 0;
    }
}
