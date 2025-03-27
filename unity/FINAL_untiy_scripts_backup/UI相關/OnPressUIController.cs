using UnityEngine;
using UnityEngine.UI;

public class ButtonPressUIController : MonoBehaviour
{
    public GameObject uiElement;  // 你想要顯示的 UI 元素

    // 這個方法將在按鈕被按下時呼叫
    public void OnPress()
    {
        uiElement.SetActive(true);  // 顯示 UI
    }
}
