using UnityEngine;

public class LookingAtYou : MonoBehaviour
{
    public Transform target;  // 目標物件
    public Transform characterUpperBody;
    public Transform characterShoulder;
    public Transform characterHead;
    public Transform characterSpine;

    public float rotationMultiplier = 1f; // 增加旋轉倍率
    public float maxNeckRotation = 50f;   // 頸部最大旋轉角度
    public float maxSpineRotation = 30f;  // 脊椎最大旋轉角度
    public float rotationSpeed = 5f;      // 平滑旋轉速度

    public float minDistanceThreshold = 0.5f;  // 當目標靠近此距離時停止旋轉
    private float previousYRotation = 0.0f;
    private float previousXRotation = 0.0f;

    void LateUpdate()
    {
        if (target != null)
        {
            float distance = Vector3.Distance(target.position, characterHead.position);

            // 當距離過近時忽略旋轉
            if (distance < minDistanceThreshold)
            {
                return; // 不進行旋轉更新，避免抖動
            }

            Vector3 direction = target.position - characterHead.position;
            Vector3 normalizedDirection = direction.normalized;

            // 計算旋轉角度
            float xRotation = Mathf.Asin(normalizedDirection.y) * Mathf.Rad2Deg;
            float yRotation = Mathf.Atan2(normalizedDirection.x, normalizedDirection.z) * Mathf.Rad2Deg;

            // 平滑化旋轉角度
            float smoothXRotation = Mathf.Lerp(previousXRotation, -xRotation, Time.deltaTime * rotationSpeed);
            float smoothYRotation = Mathf.Lerp(previousYRotation, yRotation, Time.deltaTime * rotationSpeed);

            // 更新前次旋轉角度
            previousXRotation = smoothXRotation;
            previousYRotation = smoothYRotation;

            // 更新身體旋轉
            UpdateBodyRotation(smoothYRotation, smoothXRotation);
        }
    }

    void UpdateBodyRotation(float yRotation, float xRotation)
    {
        float rotationThreshold = 0.1f; // 忽略小於該閾值的旋轉變化

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
        // 清理資源
    }
}
