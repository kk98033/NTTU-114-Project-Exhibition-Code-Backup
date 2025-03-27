using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeacherActions : MonoBehaviour
{
    private Animator hutaoAnimator;

    void Start()
    {
        // 在場景中尋找名為「主要虛擬導覽員」的角色，並取得其 Animator
        GameObject hutao = GameObject.Find("主要虛擬導覽員");
        if (hutao != null)
        {
            hutaoAnimator = hutao.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("Cannot find GameObject named '主要虛擬導覽員'");
        }

        // test
        //StartCoroutine(ExecuteActionZeroAfterDelay(5f));
    }

    // 延遲一段時間後執行 ActionZero，用於測試或初始動畫觸發
    IEnumerator ExecuteActionZeroAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ActionZero();
    }

    // 執行自訂動作 ActionOne
    public void ActionOne()
    {
        Debug.Log("Action One Executed!");
        // 此處可加入角色特定動畫或動作邏輯
    }

    public void ActionTwo()
    {
        Debug.Log("Action Two Executed!");
        // 此處可加入角色特定動畫或動作邏輯
    }

    public void ActionThree()
    {
        Debug.Log("Action Three Executed!");
        // 此處可加入角色特定動畫或動作邏輯
    }

    public void ActionZero()
    {
        Debug.Log("Action Zero Executed!");

        // 設定參數 isDancing 為 true，讓 Animator 播放跳舞動畫
        if (hutaoAnimator != null)
        {
            hutaoAnimator.SetBool("isDancing", true);
        }
        else
        {
            Debug.LogError("Animator for 'hutao' is not found.");
        }
    }

    public void ExecuteAction(int action)
    {
        // 根據整數 action 編號執行對應的方法
        switch (action)
        {
            case 0:
                ActionZero();
                break;
            case 1:
                ActionOne();
                break;
            case 2:
                ActionTwo();
                break;
            case 3:
                ActionThree();
                break;
            default:
                Debug.Log("Unknown Action: " + action);
                break;
        }
    }
}
