using UnityEngine;
using UnityEngine.UI;
using TMPro; // 使用 TextMeshPro
using UnityEngine.Networking;
using System.Collections;
using System;

public class ButtonActionHandler : MonoBehaviour
{
    public AudioSource audioSource; // 用於播放音訊的 AudioSource
    public SkinnedMeshRenderer characterMesh; // 虛擬老師的 SkinnedMeshRenderer
    public int blendShapeA = 39, blendShapeI = 40, blendShapeU = 41, blendShapeE = 42, blendShapeO = 43; // BlendShape Index
    public Animator animator; // 虛擬老師的 Animator
    public TextMeshProUGUI chatText; // 顯示文字的 TextMeshPro UI
    public GameObject chatInterface; // 文字顯示介面
    public float delayBeforeIdle = 3f; // 播放完音訊後延遲 Idle 時間

    // 按鈕按下時觸發
    public void OnButtonClicked(GameObject button)
    {
        TMP_Text buttonTextComponent = button.GetComponentInChildren<TMP_Text>(); // 獲取按鈕的文字元件
        if (buttonTextComponent == null)
        {
            Debug.LogError("No TMP_Text component found in the button.");
            return;
        }

        string buttonText = buttonTextComponent.text; // 獲取按鈕文字
        Debug.Log($"Button clicked: {buttonText}");

        // 發送請求到伺服器
        StartCoroutine(SendTextChatRequest(buttonText));
    }

    IEnumerator SendTextChatRequest(string text)
    {
        string apiUrl = "http://210.240.160.27:443/text_chat_unity";

        // 構建 JSON 請求
        string prefixedText = $"幫我查詢博物館資料工具，解答這個問題:{text}";
        string jsonBody = $"{{\"text\": \"{prefixedText}\"}}";

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(apiUrl, ""))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("Sending request to /text_chat_unity...");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Request successful!");

                string boundary = GetBoundaryFromContentType(request.GetResponseHeader("Content-Type"));
                byte[] responseData = request.downloadHandler.data;

                if (boundary != null)
                {
                    ParseMultipartResponse(responseData, boundary); // 處理多部分回應
                }
                else
                {
                    Debug.LogError("Boundary not found in Content-Type.");
                }
            }
            else
            {
                Debug.LogError($"Request failed: {request.error}");
            }
        }
    }

    private void ParseMultipartResponse(byte[] data, string boundary)
    {
        if (boundary == null)
        {
            Debug.LogError("Boundary not found in content type.");
            return;
        }

        string content = System.Text.Encoding.UTF8.GetString(data);
        string[] sections = content.Split(new string[] { boundary }, StringSplitOptions.RemoveEmptyEntries);

        Debug.Log($"Number of sections in response: {sections.Length}");

        foreach (string section in sections)
        {
            if (section.Contains("Content-Type: application/json"))
            {
                int startIndex = section.IndexOf("{");
                int endIndex = section.LastIndexOf("}");
                if (startIndex >= 0 && endIndex >= 0)
                {
                    string jsonString = section.Substring(startIndex, endIndex - startIndex + 1);
                    Debug.Log($"Parsed JSON string: {jsonString}");

                    var jsonResponse = JsonUtility.FromJson<ServerResponse>(jsonString);
                    DisplayResponseText(jsonResponse.response); // 顯示文字
                    ExecuteAction(jsonResponse.action); // 執行動作
                }
            }
            else if (section.Contains("Content-Type: audio/wav"))
            {
                int startIndex = section.IndexOf("\r\n\r\n") + 4;
                if (startIndex < section.Length)
                {
                    string base64AudioData = section.Substring(startIndex).Trim();
                    if (!string.IsNullOrEmpty(base64AudioData))
                    {
                        try
                        {
                            byte[] audioData = Convert.FromBase64String(base64AudioData);
                            StartCoroutine(PlayAudio(audioData)); // 播放音訊
                        }
                        catch (FormatException e)
                        {
                            Debug.LogError("Failed to decode Base64 audio: " + e.Message);
                        }
                    }
                }
            }
        }
    }

    private string GetBoundaryFromContentType(string contentType)
    {
        if (string.IsNullOrEmpty(contentType)) return null;

        string[] parameters = contentType.Split(';');
        foreach (string param in parameters)
        {
            string trimmedParam = param.Trim();
            if (trimmedParam.StartsWith("boundary="))
            {
                return "--" + trimmedParam.Substring("boundary=".Length);
            }
        }
        return null;
    }

    void DisplayResponseText(string text)
    {
        chatText.text = text; // 更新 UI 文字
        chatInterface.SetActive(true); // 顯示介面
        Debug.Log($"Displaying response text: {text}");
    }

    void ExecuteAction(string action)
    {
        if (!string.IsNullOrEmpty(action))
        {
            Debug.Log($"Executing action: {action}");
            animator.SetTrigger(action);
        }
    }

    IEnumerator PlayAudio(byte[] audioData)
    {
        string tempFilePath = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "temp.wav");
        System.IO.File.WriteAllBytes(tempFilePath, audioData);
        Debug.Log($"Audio data written to temp file: {tempFilePath}");

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + tempFilePath, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;
                audioSource.Play();
                Debug.Log("Playing audio...");

                animator.SetFloat("talkingBlend", 1);
                while (audioSource.isPlaying)
                {
                    AnimateMouth();
                    yield return null;
                }

                yield return new WaitForSeconds(delayBeforeIdle);
                ResetTeacher();
            }
            else
            {
                Debug.LogError(www.error);
            }
        }
    }

    void AnimateMouth()
    {
        float[] spectrum = new float[256];
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        characterMesh.SetBlendShapeWeight(blendShapeA, Mathf.Clamp(spectrum[1] * 2000, 0, 100));
        characterMesh.SetBlendShapeWeight(blendShapeI, Mathf.Clamp(spectrum[2] * 2000, 0, 100));
        characterMesh.SetBlendShapeWeight(blendShapeU, Mathf.Clamp(spectrum[3] * 2000, 0, 100));
        characterMesh.SetBlendShapeWeight(blendShapeE, Mathf.Clamp(spectrum[4] * 2000, 0, 100));
        characterMesh.SetBlendShapeWeight(blendShapeO, Mathf.Clamp(spectrum[5] * 2000, 0, 100));
    }

    void ResetTeacher()
    {
        animator.SetFloat("talkingBlend", 0);
        characterMesh.SetBlendShapeWeight(blendShapeA, 0);
        characterMesh.SetBlendShapeWeight(blendShapeI, 0);
        characterMesh.SetBlendShapeWeight(blendShapeU, 0);
        characterMesh.SetBlendShapeWeight(blendShapeE, 0);
        characterMesh.SetBlendShapeWeight(blendShapeO, 0);
        chatInterface.SetActive(false);
        Debug.Log("Teacher reset to idle state.");
    }

    [System.Serializable]
    public class ServerResponse
    {
        public string action;
        public string response;
    }
}
