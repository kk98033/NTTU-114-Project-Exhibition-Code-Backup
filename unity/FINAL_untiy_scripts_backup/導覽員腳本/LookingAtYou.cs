using UnityEngine;

public class LookingAtYou : MonoBehaviour
{
    public Transform target;  // �ؼЪ���
    public Transform characterUpperBody;
    public Transform characterShoulder;
    public Transform characterHead;
    public Transform characterSpine;

    public float rotationMultiplier = 1f; // �W�[���୿�v
    public float maxNeckRotation = 50f;   // �V���̤j���ਤ��
    public float maxSpineRotation = 30f;  // ��ճ̤j���ਤ��
    public float rotationSpeed = 5f;      // ���Ʊ���t��

    public float minDistanceThreshold = 0.5f;  // ��ؼоa�񦹶Z���ɰ������
    private float previousYRotation = 0.0f;
    private float previousXRotation = 0.0f;

    void LateUpdate()
    {
        if (target != null)
        {
            float distance = Vector3.Distance(target.position, characterHead.position);

            // ��Z���L��ɩ�������
            if (distance < minDistanceThreshold)
            {
                return; // ���i������s�A�קK�ݰ�
            }

            Vector3 direction = target.position - characterHead.position;
            Vector3 normalizedDirection = direction.normalized;

            // �p����ਤ��
            float xRotation = Mathf.Asin(normalizedDirection.y) * Mathf.Rad2Deg;
            float yRotation = Mathf.Atan2(normalizedDirection.x, normalizedDirection.z) * Mathf.Rad2Deg;

            // ���ƤƱ��ਤ��
            float smoothXRotation = Mathf.Lerp(previousXRotation, -xRotation, Time.deltaTime * rotationSpeed);
            float smoothYRotation = Mathf.Lerp(previousYRotation, yRotation, Time.deltaTime * rotationSpeed);

            // ��s�e�����ਤ��
            previousXRotation = smoothXRotation;
            previousYRotation = smoothYRotation;

            // ��s�������
            UpdateBodyRotation(smoothYRotation, smoothXRotation);
        }
    }

    void UpdateBodyRotation(float yRotation, float xRotation)
    {
        float rotationThreshold = 0.1f; // �����p����H�Ȫ������ܤ�

        float spineRotation = Mathf.Clamp(yRotation * 0.3f, -maxSpineRotation, maxSpineRotation);
        float shoulderRotation = Mathf.Clamp((yRotation - spineRotation) * 0.5f, -maxNeckRotation, maxNeckRotation);
        float neckRotation = yRotation - spineRotation - shoulderRotation;

        if (characterSpine != null && Mathf.Abs(spineRotation) > rotationThreshold)
        {
            Vector3 spineEuler = characterSpine.localEulerAngles;
            spineEuler.y = Mathf.LerpAngle(spineEuler.y, spineRotation, Time.deltaTime * rotationSpeed);
            characterSpine.localEulerAngles = spineEuler;
        }

        if (characterShoulder != null && Mathf.Abs(shoulderRotation) > rotationThreshold)
        {
            Vector3 shoulderEuler = characterShoulder.localEulerAngles;
            shoulderEuler.y = Mathf.LerpAngle(shoulderEuler.y, shoulderRotation, Time.deltaTime * rotationSpeed);
            characterShoulder.localEulerAngles = shoulderEuler;
        }

        if (characterHead != null && Mathf.Abs(neckRotation) > rotationThreshold)
        {
            float neckYRotation = Mathf.Clamp(neckRotation, -maxNeckRotation, maxNeckRotation);
            float neckXRotation = Mathf.Clamp(xRotation, -maxNeckRotation, maxNeckRotation);
            Vector3 headEuler = characterHead.localEulerAngles;
            headEuler.y = Mathf.LerpAngle(headEuler.y, neckYRotation, Time.deltaTime * rotationSpeed);
            headEuler.x = Mathf.LerpAngle(headEuler.x, neckXRotation, Time.deltaTime * rotationSpeed);
            characterHead.localEulerAngles = headEuler;
        }
    }

    float smoothAngle(float angle)
    {
        if (angle > 180) angle -= 360;
        if (angle < -180) angle += 360;
        return angle;
    }

    void OnApplicationQuit()
    {
        // �M�z�귽
    }
}
