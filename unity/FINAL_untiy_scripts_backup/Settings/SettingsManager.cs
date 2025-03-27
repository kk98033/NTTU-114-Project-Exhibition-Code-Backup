using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; } // 單例實例

    // 設定參數
    public int Port { get; private set; } = 443; // 預設值為 443

    private void Awake()
    {
        // 單例模式設置
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保持跨場景存在
            LoadSettings(); // 加載保存的設定
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 設置 Port 並保存到 PlayerPrefs
    public void SetPort(int port)
    {
        Port = port;
        PlayerPrefs.SetInt("Port", Port); // 保存數據到本地
        PlayerPrefs.Save();
    }

    // 加載設定
    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("Port"))
        {
            Port = PlayerPrefs.GetInt("Port");
        }
    }
}
