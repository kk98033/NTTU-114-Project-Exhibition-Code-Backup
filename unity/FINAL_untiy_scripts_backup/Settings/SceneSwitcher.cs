using UnityEngine;
using UnityEngine.SceneManagement; // 場景管理需要的命名空間

public class SceneSwitcher : MonoBehaviour
{
    // 方法：根據場景名稱切換場景
    public void SwitchSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // 方法：根據場景索引切換場景
    public void SwitchSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
