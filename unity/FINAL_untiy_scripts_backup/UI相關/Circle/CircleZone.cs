using UnityEngine;

public class CircleZone : MonoBehaviour
{
    public SharedButtonManager sharedButtonManager; // 共享按鈕管理器

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter called. Collider: {other.name}");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the zone.");

            // 將共享按鈕移動到當前圓圈的上方
            sharedButtonManager.ShowButtons(transform.position);

            // 更新按鈕文字
            sharedButtonManager.UpdateButtonText(0, "按鈕1");
            sharedButtonManager.UpdateButtonText(1, "按鈕2");
            sharedButtonManager.UpdateButtonText(2, "按鈕3");
        }
        else
        {
            Debug.Log("Non-player object entered the zone.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log($"OnTriggerExit called. Collider: {other.name}");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited the zone.");

            // 隱藏按鈕
            sharedButtonManager.HideButtons();
        }
    }
}
