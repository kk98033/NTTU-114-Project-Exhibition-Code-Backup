using UnityEngine;

public class ShowUIOnPress : MonoBehaviour
{
    public GameObject uiElement;  // 這是你想要顯示/隱藏的 UI 元素

    void Start()
    {
        // 在遊戲開始時隱藏 UI 元素
        if (uiElement != null)
        {
            uiElement.SetActive(false);
        }
    }

    // 這個方法將在按下按鈕時被呼叫，會切換 UI 的顯示/隱藏狀態
    public void ToggleUI()
    {
        if (uiElement != null)
        {
            // 切換 UI 的顯示/隱藏狀態
            uiElement.SetActive(!uiElement.activeSelf);
        }
    }
}
