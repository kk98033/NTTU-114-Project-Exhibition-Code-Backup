using UnityEngine;
using TMPro; // �K�[�R�W�Ŷ�
using System.Collections;

public class SharedButtonManager : MonoBehaviour
{
    public CanvasGroup buttonGroup; // Canvas Group �Ω�H�J�H�X
    public RectTransform buttonTransform; // ���s�ժ�������
    public TMP_Text[] buttonTexts; // �ϥ� TextMeshPro �����s��r
    public float buttonHeightOffset = 2f; // ���s���װ���
    public float fadeDuration = 0.5f; // �H�J�H�X���ɶ�

    // ��ܫ��s��
    public void ShowButtons(Vector3 position)
    {
        Debug.Log($"ShowButtons called. Position: {position}");

        // ��s���s�ժ���m
        buttonTransform.position = position + Vector3.up * buttonHeightOffset;
        Debug.Log($"ButtonTransform new position: {buttonTransform.position}");

        // �����e�Ҧ��� Coroutine
        StopAllCoroutines();

        // �ҰʲH�J�ʵe
        StartCoroutine(FadeCanvasGroup(buttonGroup, 1)); // �H�J
    }

    // ���ë��s��
    public void HideButtons()
    {
        Debug.Log("HideButtons called.");

        // �����e�Ҧ��� Coroutine
        StopAllCoroutines();

        // �ҰʲH�X�ʵe
        StartCoroutine(FadeCanvasGroup(buttonGroup, 0)); // �H�X
    }

    // ��s���s��r
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

    // ���� Canvas Group ���H�J�H�X
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
