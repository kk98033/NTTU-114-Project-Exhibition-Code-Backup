using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; } // ��ҹ��

    // �]�w�Ѽ�
    public int Port { get; private set; } = 443; // �w�]�Ȭ� 443

    private void Awake()
    {
        // ��ҼҦ��]�m
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �O��������s�b
            LoadSettings(); // �[���O�s���]�w
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // �]�m Port �ëO�s�� PlayerPrefs
    public void SetPort(int port)
    {
        Port = port;
        PlayerPrefs.SetInt("Port", Port); // �O�s�ƾڨ쥻�a
        PlayerPrefs.Save();
    }

    // �[���]�w
    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("Port"))
        {
            Port = PlayerPrefs.GetInt("Port");
        }
    }
}
