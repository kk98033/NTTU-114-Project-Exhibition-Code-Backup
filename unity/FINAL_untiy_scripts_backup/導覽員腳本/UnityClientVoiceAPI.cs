using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Text;
using System;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using TMPro;
//using GLTF.Schema;
using Buffer = System.Buffer;
using UnityEngine.UI;



public class UnityClientVoiceAPI : MonoBehaviour
{
    public AudioSource audioSource;

    private AudioClip audioClip;
    private bool isRecording = false;

    private string apiUrl = "http://210.240.160.27:443/voice_chat";
    //private string apiUrl = "http://127.0.0.1:/voice_chat";

    public TeacherActions teacherActions;
    public Transform head;
    public float spawnDistance = 2;
    public GameObject TalkingInterface;
    public GameObject LoadingInterface;
    public GameObject ChatInterface;
    public TextMeshProUGUI ChatText;
    public InputActionProperty startTalk;

    //自動滑動文字
    public ScrollRect scrollRect; // 連結到你的 Scroll View 上的 ScrollRect
    public float scrollSpeed = 4f; // 滑動速度
    private float targetPosition = 0f; // 目標位置
    public float delayBeforeScroll = 3f; // 滑動前的延遲時間（秒）




    // 新增 blendshape 欄位
    public SkinnedMeshRenderer characterMesh; // 模型的 SkinnedMeshRenderer
    private int blendShapeA = 39; // "a" 對應的 BlendShape 索引
    private int blendShapeI = 40; // "i" 對應的 BlendShape 索引
    private int blendShapeU = 41; // "u" 對應的 BlendShape 索引
    private int blendShapeE = 42; // "e" 對應的 BlendShape 索引
    private int blendShapeO = 43; // "o" 對應的 BlendShape 索引

    public Animator animator;
    private int leftHandLayerIndex = 1;

    void Start()
    {
        animator.SetLayerWeight(leftHandLayerIndex, 0);
        
    }

    void Update()
    {
        

        ProcessRecordingInput();
    }

    void LateUpdate()
    {
        if (audioSource.isPlaying)
        {
            //Debug.Log("Audio is playing, triggering AnimateMouth()");
            AnimateMouth();  // 播放音頻時進行口型動畫
        }
        else
        {
            //Debug.Log("Audio is NOT playing");
        }
    }

    private void ProcessRecordingInput()
    {
        // 檢查 startTalk 按鍵 或 鍵盤 "K" 鍵
        if ((startTalk.action.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.K)) && !isRecording)
        {
            StartRecording();
            TalkingInterface.SetActive(!TalkingInterface.activeSelf);
        }
        else if ((startTalk.action.WasReleasedThisFrame() || Input.GetKeyUp(KeyCode.K)) && isRecording)
        {
            StartCoroutine(DelayStopRecording(1f)); // 延遲 1 秒停止錄音
            TalkingInterface.SetActive(!TalkingInterface.activeSelf);
            LoadingInterface.SetActive(!LoadingInterface.activeSelf);

        }
    }
    IEnumerator AutoScroll()
    {
        while (true)
        {
            // 緩慢將 verticalNormalizedPosition 調整到目標位置
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(scrollRect.verticalNormalizedPosition, targetPosition, scrollSpeed * Time.deltaTime);

            // 檢查如果目標位置已經很接近，就結束滑動
            if (Mathf.Abs(scrollRect.verticalNormalizedPosition - targetPosition) < 0.01f)
            {
                scrollRect.verticalNormalizedPosition = targetPosition;
            }

            yield return null;
        }
    }
    // 延遲後開始滾動
    IEnumerator DelayedScroll()
    {
        yield return new WaitForSeconds(delayBeforeScroll); // 等待指定的秒數
        scrollRect.verticalNormalizedPosition = 1f;
        targetPosition = 0f;
        StartCoroutine(AutoScroll()); // 開始自動滾動

    }
    IEnumerator DelayStopRecording(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay time
        StopRecordingAndSendAudio(); // Stop recording and send audio after delay
    }

    private void StartRecording()
    {
        isRecording = true;
        audioClip = Microphone.Start(null, true, 10, 44100);
        Debug.Log("Recording started!");
    }

    private void StopRecordingAndSendAudio()
    {
        int lastSample = Microphone.GetPosition(null);
        Microphone.End(null);
        isRecording = false;
        Debug.Log("Recording ended, sending audio...");

        // Convert audio to byte array and send
        if (lastSample > 0)
        {
            StartCoroutine(SendAudio(ConvertAudioClipToBytes(audioClip, lastSample)));
        }
        else
        {
            Debug.Log("No mic input detected.");
        }
    }

    private byte[] ConvertAudioClipToBytes(AudioClip clip, int lastSample)
    {
        float[] samples = new float[lastSample * clip.channels];
        clip.GetData(samples, 0);

        short[] intData = new short[samples.Length];
        byte[] bytes = new byte[samples.Length * 2];
        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * short.MaxValue);
        }
        Buffer.BlockCopy(intData, 0, bytes, 0, bytes.Length);
        return AddWavFileHeader(bytes, clip.channels, clip.frequency, 16);
    }

    private byte[] AddWavFileHeader(byte[] audioData, int channels, int sampleRate, int bitDepth)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                // Write header information according to the WAV file standard
                WriteWavHeader(writer, audioData.Length, channels, sampleRate, bitDepth);
                writer.Write(audioData);  // Add audio data
            }
            return memoryStream.ToArray();
        }
    }

    private void WriteWavHeader(BinaryWriter writer, int audioDataLength, int channels, int sampleRate, int bitDepth)
    {
        writer.Write("RIFF".ToCharArray());
        writer.Write(36 + audioDataLength);  // Total file length minus the first 8 bytes
        writer.Write("WAVE".ToCharArray());
        writer.Write("fmt ".ToCharArray());
        writer.Write(16);  // Length of the WAV format block
        writer.Write((short)1);  // Audio format, PCM
        writer.Write((short)channels);
        writer.Write(sampleRate);
        writer.Write(sampleRate * channels * (bitDepth / 8));  // Bytes per second
        writer.Write((short)(channels * (bitDepth / 8)));  // Data block alignment unit
        writer.Write((short)bitDepth);  // Bits per sample
        writer.Write("data".ToCharArray());
        writer.Write(audioDataLength);  // Length of the audio data
    }

    IEnumerator SendAudio(byte[] audioData)
    {
        Debug.Log("Sending audio to API...");

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", audioData, "recording.wav", "audio/wav");

        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Audio successfully sent!");

                // 解析 JSON 和播放音頻
                if (www.downloadHandler != null)
                {
                    // 獲取邊界
                    string boundary = GetBoundaryFromContentType(www.GetResponseHeader("Content-Type"));

                    byte[] responseData = www.downloadHandler.data;
                    Debug.Log($"Response data length: {responseData.Length} bytes");

                    // 增加調試信息，打印部分回應內容
                    string responsePreview = Encoding.UTF8.GetString(responseData, 0, Mathf.Min(responseData.Length, 500));
                    Debug.Log($"Response content preview (first 500 chars): {responsePreview}");

                    ParseMultipartResponse(responseData, boundary);
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

    private void ParseMultipartResponse(byte[] data, string boundary)
    {
        if (boundary == null)
        {
            Debug.LogError("Boundary not found in content type.");
            return;
        }

        string content = Encoding.UTF8.GetString(data);
        string[] sections = content.Split(new string[] { boundary }, StringSplitOptions.RemoveEmptyEntries);

        Debug.Log($"Number of sections in response: {sections.Length}");

        foreach (string section in sections)
        {
            Debug.Log($"Processing section:\n{section}");

            if (section.Contains("Content-Type: application/json"))
            {
                int startIndex = section.IndexOf("{");
                int endIndex = section.LastIndexOf("}");
                if (startIndex >= 0 && endIndex >= 0)
                {
                    string jsonString = section.Substring(startIndex, endIndex - startIndex + 1);
                    Debug.Log($"Parsed JSON string: {jsonString}");

                    // 解析 JSON 並從中獲取 'response' 欄位
                    ActionResponseWithText actionResponse = JsonUtility.FromJson<ActionResponseWithText>(jsonString);
                    Debug.Log("Action: " + actionResponse.action);
                    Debug.Log("Response Text: " + actionResponse.response);

                    // 更新 UI 顯示
                    ChatText.text = $"{actionResponse.response}";

                    // 呼叫 TeacherActions 的 ExecuteAction 方法
                    teacherActions.ExecuteAction(actionResponse.action);
                }
                else
                {
                    Debug.LogError("Failed to find JSON object in the section.");
                }
            }
            else if (section.Contains("Content-Type: text/plain"))
            {
                int startIndex = section.IndexOf("\r\n\r\n") + 4;
                if (startIndex < section.Length)
                {
                    string responseText = section.Substring(startIndex).Trim();
                    Debug.Log($"Extracted text/plain response: {responseText}");

                    // 顯示在 TextMeshPro 中
                    ChatText.text = $"{responseText}";
                }
            }
            else if (section.Contains("Content-Type: audio/wav"))  
            {
                int startIndex = section.IndexOf("\r\n\r\n") + 4;
                if (startIndex < section.Length)
                {
                    string base64AudioData = section.Substring(startIndex).Trim();
                    Debug.Log($"Base64 audio data length: {base64AudioData.Length}");

                    // 檢查 Base64 字串是否有效
                    if (base64AudioData.Length > 0)
                    {
                        try
                        {
                            byte[] audioData = Convert.FromBase64String(base64AudioData);
                            Debug.Log($"Decoded audio data length: {audioData.Length} bytes");
                            StartCoroutine(PlayAudio(audioData));  // 使用 PlayAudio 播放音訊
                            LoadingInterface.SetActive(!LoadingInterface.activeSelf);//關loading
                            ChatInterface.SetActive(!ChatInterface.activeSelf);//開對話框
                            StartCoroutine(DelayedScroll());
                        }
                        catch (FormatException e)
                        {
                            Debug.LogError("Base64 decode error: " + e.Message);
                        }
                    }
                    else
                    {
                        Debug.LogError("Base64 audio data is empty.");
                    }
                }
                else
                {
                    Debug.LogError("Failed to find audio data in the section.");
                }
            }
        }
    }

    IEnumerator PlayAudio(byte[] audioData)
    {
        // 修改臨時檔案路徑為 .wav
        string tempFilePath = Path.Combine(Application.persistentDataPath, "temp.wav");
        File.WriteAllBytes(tempFilePath, audioData);
        Debug.Log($"Audio data written to temp file: {tempFilePath}");

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + tempFilePath, AudioType.WAV)) 
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                if (clip == null)
                {
                    Debug.LogError("Failed to load AudioClip from downloaded data.");
                }
                else
                {
                    audioSource.clip = clip;
                    audioSource.Play();
                    Debug.Log("AudioClip played successfully.");

                    // 撥放動畫（將左手動畫層的權重設為 1）
                    animator.SetFloat("talkingBlend", Mathf.Lerp(animator.GetFloat("talkingBlend"), 1, Time.deltaTime * 5));  // 啟用說話動畫
                    animator.SetLayerWeight(leftHandLayerIndex, 1);

                    // 等待音頻播放完畢
                    yield return new WaitWhile(() => audioSource.isPlaying);

                    //關對話框
                    yield return new WaitForSeconds(delayBeforeScroll);
                    ChatInterface.SetActive(!ChatInterface.activeSelf);

                    // 停止動畫（將左手動畫層的權重設為 0）
                    animator.SetLayerWeight(leftHandLayerIndex, 0);
                    animator.SetFloat("talkingBlend", Mathf.Lerp(animator.GetFloat("talkingBlend"), 0, Time.deltaTime * 5));  // 回到Idle動畫
                }
                Debug.Log($"Temporary file deleted: {tempFilePath}");
            }
        }
    }

    // 用於口型動畫的函數
    private void AnimateMouth()
    {
        float[] spectrum = new float[256];  // 用於儲存音頻頻譜數據
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        // 調整放大倍率，使口型變化更加自然
        float aStrength = Mathf.Clamp(spectrum[1] * 2500, 0, 45);  // 將最大值控制在 100 以內
        float iStrength = Mathf.Clamp(spectrum[3] * 7000, 0, 70);
        float uStrength = Mathf.Clamp(spectrum[5] * 5000, 0, 80);
        float eStrength = Mathf.Clamp(spectrum[7] * 6000, 0, 60);
        float oStrength = Mathf.Clamp(spectrum[9] * 4000, 0, 45);

        // 設定 blendshape 欄位的值，模擬口型變化
        characterMesh.SetBlendShapeWeight(blendShapeA, aStrength);
        characterMesh.SetBlendShapeWeight(blendShapeI, iStrength);
        characterMesh.SetBlendShapeWeight(blendShapeU, uStrength);
        characterMesh.SetBlendShapeWeight(blendShapeE, eStrength);
        characterMesh.SetBlendShapeWeight(blendShapeO, oStrength);

        // Debug 用來檢查每個口型的權重
        Debug.Log($"BlendShape A: {aStrength}, I: {iStrength}, U: {uStrength}, E: {eStrength}, O: {oStrength}");
    }


    [System.Serializable]
    public class ActionResponseWithText
    {
        public int action;
        public string response;
    }

}
