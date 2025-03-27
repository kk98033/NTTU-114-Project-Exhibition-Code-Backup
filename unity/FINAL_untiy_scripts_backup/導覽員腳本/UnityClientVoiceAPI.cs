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

    //�۰ʷưʤ�r
    public ScrollRect scrollRect; // �s����A�� Scroll View �W�� ScrollRect
    public float scrollSpeed = 4f; // �ưʳt��
    private float targetPosition = 0f; // �ؼЦ�m
    public float delayBeforeScroll = 3f; // �ưʫe������ɶ��]��^




    // �s�W blendshape ���
    public SkinnedMeshRenderer characterMesh; // �ҫ��� SkinnedMeshRenderer
    private int blendShapeA = 39; // "a" ������ BlendShape ����
    private int blendShapeI = 40; // "i" ������ BlendShape ����
    private int blendShapeU = 41; // "u" ������ BlendShape ����
    private int blendShapeE = 42; // "e" ������ BlendShape ����
    private int blendShapeO = 43; // "o" ������ BlendShape ����

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
            AnimateMouth();  // �����W�ɶi��f���ʵe
        }
        else
        {
            //Debug.Log("Audio is NOT playing");
        }
    }

    private void ProcessRecordingInput()
    {
        // �ˬd startTalk ���� �� ��L "K" ��
        if ((startTalk.action.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.K)) && !isRecording)
        {
            StartRecording();
            TalkingInterface.SetActive(!TalkingInterface.activeSelf);
        }
        else if ((startTalk.action.WasReleasedThisFrame() || Input.GetKeyUp(KeyCode.K)) && isRecording)
        {
            StartCoroutine(DelayStopRecording(1f)); // ���� 1 �������
            TalkingInterface.SetActive(!TalkingInterface.activeSelf);
            LoadingInterface.SetActive(!LoadingInterface.activeSelf);

        }
    }
    IEnumerator AutoScroll()
    {
        while (true)
        {
            // �w�C�N verticalNormalizedPosition �վ��ؼЦ�m
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(scrollRect.verticalNormalizedPosition, targetPosition, scrollSpeed * Time.deltaTime);

            // �ˬd�p�G�ؼЦ�m�w�g�ܱ���A�N�����ư�
            if (Mathf.Abs(scrollRect.verticalNormalizedPosition - targetPosition) < 0.01f)
            {
                scrollRect.verticalNormalizedPosition = targetPosition;
            }

            yield return null;
        }
    }
    // �����}�l�u��
    IEnumerator DelayedScroll()
    {
        yield return new WaitForSeconds(delayBeforeScroll); // ���ݫ��w�����
        scrollRect.verticalNormalizedPosition = 1f;
        targetPosition = 0f;
        StartCoroutine(AutoScroll()); // �}�l�۰ʺu��

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

                // �ѪR JSON �M�����W
                if (www.downloadHandler != null)
                {
                    // ������
                    string boundary = GetBoundaryFromContentType(www.GetResponseHeader("Content-Type"));

                    byte[] responseData = www.downloadHandler.data;
                    Debug.Log($"Response data length: {responseData.Length} bytes");

                    // �W�[�ոիH���A���L�����^�����e
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

                    // �ѪR JSON �ñq����� 'response' ���
                    ActionResponseWithText actionResponse = JsonUtility.FromJson<ActionResponseWithText>(jsonString);
                    Debug.Log("Action: " + actionResponse.action);
                    Debug.Log("Response Text: " + actionResponse.response);

                    // ��s UI ���
                    ChatText.text = $"{actionResponse.response}";

                    // �I�s TeacherActions �� ExecuteAction ��k
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

                    // ��ܦb TextMeshPro ��
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

                    // �ˬd Base64 �r��O�_����
                    if (base64AudioData.Length > 0)
                    {
                        try
                        {
                            byte[] audioData = Convert.FromBase64String(base64AudioData);
                            Debug.Log($"Decoded audio data length: {audioData.Length} bytes");
                            StartCoroutine(PlayAudio(audioData));  // �ϥ� PlayAudio ���񭵰T
                            LoadingInterface.SetActive(!LoadingInterface.activeSelf);//��loading
                            ChatInterface.SetActive(!ChatInterface.activeSelf);//�}��ܮ�
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
        // �ק��{���ɮ׸��|�� .wav
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

                    // ����ʵe�]�N����ʵe�h���v���]�� 1�^
                    animator.SetFloat("talkingBlend", Mathf.Lerp(animator.GetFloat("talkingBlend"), 1, Time.deltaTime * 5));  // �ҥλ��ܰʵe
                    animator.SetLayerWeight(leftHandLayerIndex, 1);

                    // ���ݭ��W���񧹲�
                    yield return new WaitWhile(() => audioSource.isPlaying);

                    //����ܮ�
                    yield return new WaitForSeconds(delayBeforeScroll);
                    ChatInterface.SetActive(!ChatInterface.activeSelf);

                    // ����ʵe�]�N����ʵe�h���v���]�� 0�^
                    animator.SetLayerWeight(leftHandLayerIndex, 0);
                    animator.SetFloat("talkingBlend", Mathf.Lerp(animator.GetFloat("talkingBlend"), 0, Time.deltaTime * 5));  // �^��Idle�ʵe
                }
                Debug.Log($"Temporary file deleted: {tempFilePath}");
            }
        }
    }

    // �Ω�f���ʵe�����
    private void AnimateMouth()
    {
        float[] spectrum = new float[256];  // �Ω��x�s���W�W�мƾ�
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        // �վ��j���v�A�Ϥf���ܤƧ�[�۵M
        float aStrength = Mathf.Clamp(spectrum[1] * 2500, 0, 45);  // �N�̤j�ȱ���b 100 �H��
        float iStrength = Mathf.Clamp(spectrum[3] * 7000, 0, 70);
        float uStrength = Mathf.Clamp(spectrum[5] * 5000, 0, 80);
        float eStrength = Mathf.Clamp(spectrum[7] * 6000, 0, 60);
        float oStrength = Mathf.Clamp(spectrum[9] * 4000, 0, 45);

        // �]�w blendshape ��쪺�ȡA�����f���ܤ�
        characterMesh.SetBlendShapeWeight(blendShapeA, aStrength);
        characterMesh.SetBlendShapeWeight(blendShapeI, iStrength);
        characterMesh.SetBlendShapeWeight(blendShapeU, uStrength);
        characterMesh.SetBlendShapeWeight(blendShapeE, eStrength);
        characterMesh.SetBlendShapeWeight(blendShapeO, oStrength);

        // Debug �Ψ��ˬd�C�Ӥf�����v��
        Debug.Log($"BlendShape A: {aStrength}, I: {iStrength}, U: {uStrength}, E: {eStrength}, O: {oStrength}");
    }


    [System.Serializable]
    public class ActionResponseWithText
    {
        public int action;
        public string response;
    }

}
