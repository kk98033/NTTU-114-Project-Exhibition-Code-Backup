using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CircleInteraction : MonoBehaviour
{
    public CanvasGroup buttonGroup; // �������s�� Canvas Group
    public Text[] buttonTexts; // ���s�W����r
    public float fadeDuration = 0.5f; // �H�J�H�X���ɶ�

    private bool isPlayerInside = false;

    void Start()
    {
        // �T�O���s�@�}�l�O���ê�
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
            StartCoroutine(FadeCanvasGroup(buttonGroup, 1)); // �H�J
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            StopAllCoroutines();
            StartCoroutine(FadeCanvasGroup(buttonGroup, 0)); // �H�X
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

        // ����椬�M�g�u
        canvasGroup.interactable = targetAlpha > 0;
        canvasGroup.blocksRaycasts = targetAlpha > 0;
    }
}
