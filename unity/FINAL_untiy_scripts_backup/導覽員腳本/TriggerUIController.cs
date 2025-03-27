using UnityEngine;
using UnityEngine.UI;

public class TriggerUIController : MonoBehaviour
{
    public GameObject uiElement;  // �o�ӬO�A�Q�n��ܪ� UI ����
    public GameObject player;     // �o�ӬO�A�n���w�� Player ����

    // ���a�i�J�㦳 isTrigger �Ŀ諸�ϰ�ɰ���
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player) // �ϥ� public player �ӧP�_�O�_�O���a
        {
            uiElement.SetActive(true); // ��� UI
        }
    }

    // ���a���}�Ӱϰ�ɰ���
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            uiElement.SetActive(false); // ���� UI
        }
    }
}
