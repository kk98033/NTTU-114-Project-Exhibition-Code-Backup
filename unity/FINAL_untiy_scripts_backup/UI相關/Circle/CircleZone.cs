using UnityEngine;

public class CircleZone : MonoBehaviour
{
    public SharedButtonManager sharedButtonManager; // �@�ɫ��s�޲z��

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter called. Collider: {other.name}");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the zone.");

            // �N�@�ɫ��s���ʨ��e��骺�W��
            sharedButtonManager.ShowButtons(transform.position);

            // ��s���s��r
            sharedButtonManager.UpdateButtonText(0, "���s1");
            sharedButtonManager.UpdateButtonText(1, "���s2");
            sharedButtonManager.UpdateButtonText(2, "���s3");
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

            // ���ë��s
            sharedButtonManager.HideButtons();
        }
    }
}
