using UnityEngine;
using TMPro; // 使用 TextMeshPro
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CircleZone : MonoBehaviour
{
    public SharedButtonManager sharedButtonManager; // 共享按鈕管理器
    public string artifactName = "儒艮"; // 文物名稱，可在 Inspector 修改
    private bool isRequestInProgress = false; // 防止多次請求

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isRequestInProgress)
        {
            Debug.Log($"Player entered zone: {artifactName}");
            StartCoroutine(SendApiRequest(artifactName));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player exited zone: {artifactName}");
            StartCoroutine(DelayedHideButtons());
        }
    }

    IEnumerator DelayedHideButtons()
    {
        float delay = 1.0f; // 延遲時間（秒）
        yield return new WaitForSeconds(delay);

        sharedButtonManager.HideButtons(); // 在延遲後隱藏按鈕並啟動淡出效果
        Debug.Log("Buttons are now hiding after delay.");
    }


    IEnumerator SendApiRequest(string artifact)
    {
        isRequestInProgress = true;

        // API 請求的 URL
        string apiUrl = "http://210.240.160.27:443/text_chat";

        // API 請求的內容
        string requestText = $"你要為博物館的這個文物: '{artifact}' 生成三個有關於他的簡短問題\\n你必須要查閱博物館資料\\n你必須以<intro>生成的問題</intro>的格式回答。例如:<intro>生成的問題1</intro><intro>生成的問題2</intro><intro>生成的問題3</intro>";
        string jsonBody = $"{{\"text\": \"{requestText}\", \"generate_audio\": false}}";

        // 禁用 SSL 驗證
        System.Net.ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

        // 發送 HTTP POST 請求
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(apiUrl, ""))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("Sending API request...");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("API request successful.");
                Debug.Log($"Response: {request.downloadHandler.text}");

                // 解碼回應並更新按鈕
                string decodedResponse = DecodeUnicode(request.downloadHandler.text);
                UpdateButtonsFromResponse(decodedResponse);
            }
            else
            {
                Debug.LogError($"API request failed: {request.error}");
            }
        }

        isRequestInProgress = false;
    }

    // 解碼 Unicode 字符串
    string DecodeUnicode(string input)
    {
        // 使用正則表達式查找 \uXXXX 格式並進行解碼
        return Regex.Replace(input, @"\\u([0-9A-Fa-f]{4})", match =>
        {
            return ((char)System.Convert.ToInt32(match.Groups[1].Value, 16)).ToString();
        });
    }

    void UpdateButtonsFromResponse(string response)
    {
        // 使用正則表達式提取 <intro> 標籤中的內容
        List<string> intros = new List<string>();
        MatchCollection matches = Regex.Matches(response, @"<intro>(.*?)</intro>");

        foreach (Match match in matches)
        {
            if (intros.Count < 3) // 最多提取三個
            {
                intros.Add(match.Groups[1].Value);
            }
        }

        // 將解析出的內容更新到按鈕
        for (int i = 0; i < intros.Count; i++)
        {
            sharedButtonManager.UpdateButtonText(i, intros[i]);
        }

        // 如果沒有提取到內容，清空按鈕文字
        for (int i = intros.Count; i < 3; i++)
        {
            sharedButtonManager.UpdateButtonText(i, ""); // 清空多餘按鈕的文字
        }

        // 顯示按鈕
        sharedButtonManager.ShowButtons(transform.position);
    }
}
