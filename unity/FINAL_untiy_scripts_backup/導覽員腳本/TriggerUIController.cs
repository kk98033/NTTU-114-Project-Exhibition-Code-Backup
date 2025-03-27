using UnityEngine;
using UnityEngine.UI;

public class TriggerUIController : MonoBehaviour
{
    public GameObject uiElement;  // 這個是你想要顯示的 UI 元素
    public GameObject player;     // 這個是你要指定的 Player 物件

    // 當玩家進入具有 isTrigger 勾選的區域時執行
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player) // 使用 public player 來判斷是否是玩家
        {
            uiElement.SetActive(true); // 顯示 UI
        }
    }

    // 當玩家離開該區域時執行
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            uiElement.SetActive(false); // 隱藏 UI
        }
    }
}
