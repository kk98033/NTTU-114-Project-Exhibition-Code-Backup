using UnityEngine;
using UnityEngine.UI;

public class ButtonPressUIController : MonoBehaviour
{
    public GameObject uiElement;  // �A�Q�n��ܪ� UI ����

    // �o�Ӥ�k�N�b���s�Q���U�ɩI�s
    public void OnPress()
    {
        uiElement.SetActive(true);  // ��� UI
    }
}
