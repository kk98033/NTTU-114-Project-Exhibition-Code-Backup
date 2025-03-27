using UnityEngine;

public class ShowUIOnPress : MonoBehaviour
{
    public GameObject uiElement;  // �o�O�A�Q�n���/���ê� UI ����

    void Start()
    {
        // �b�C���}�l������ UI ����
        if (uiElement != null)
        {
            uiElement.SetActive(false);
        }
    }

    // �o�Ӥ�k�N�b���U���s�ɳQ�I�s�A�|���� UI �����/���ê��A
    public void ToggleUI()
    {
        if (uiElement != null)
        {
            // ���� UI �����/���ê��A
            uiElement.SetActive(!uiElement.activeSelf);
        }
    }
}
