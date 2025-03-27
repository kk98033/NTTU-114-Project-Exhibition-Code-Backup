using UnityEngine;
using TMPro; // 添加命名空間
using System.Collections;

public class SharedButtonManager : MonoBehaviour
{
    public CanvasGroup buttonGroup; // Canvas Group 用於淡入淡出
    public RectTransform buttonTransform; // 按鈕組的父物件
    public TMP_Text[] buttonTexts; // 使用 TextMeshPro 的按鈕文字
    public float buttonHeightOffset = 2f; // 按鈕高度偏移
    public float fadeDuration = 0.5f; // 淡入淡出的時間

    // 顯示按鈕組
    public void ShowButtons(Vector3 position)
    {
        Debug.Log($"ShowButtons called. Position: {position}");

        // 更新按鈕組的位置
        buttonTransform.position = position + Vector3.up * buttonHeightOffset;
        Debug.Log($"ButtonTransform new position: {buttonTransform.position}");

        // 停止當前所有的 Coroutine
        StopAllCoroutines();

        // 啟動淡入動畫
        StartCoroutine(FadeCanvasGroup(buttonGroup, 1)); // 淡入
    }

    // 隱藏按鈕組
    public void HideButtons()
    {
        Debug.Log("HideButtons called.");

        // 停止當前所有的 Coroutine
        StopAllCoroutines();

        // 啟動淡出動畫
        StartCoroutine(FadeCanvasGroup(buttonGroup, 0)); // 淡出
    }

    // 更新按鈕文字
    public void UpdateButtonText(int index, string newText)
    {
        if (index >= 0 && index < buttonTexts.Length)
        {
            buttonTexts[index].text = newText;
            Debug.Log($"Button {index} text updated to: {newText}");
        }
        else
        {
            Debug.LogWarning($"Invalid button index: {index}");
        }
    }

    // 控制 Canvas Group 的淡入淡出
    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha)
    {
        Debug.Log($"FadeCanvasGroup called. Target Alpha: {targetAlpha}");

        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        Debug.Log($"Fade completed. CanvasGroup alpha is now: {canvasGroup.alpha}");
    }
}
