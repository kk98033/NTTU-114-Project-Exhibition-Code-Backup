using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeacherActions : MonoBehaviour
{
    private Animator hutaoAnimator;

    void Start()
    {
        // Àò¨ú¦WºÙ¬° "¥D­nµêÀÀ¾ÉÄý­û" ªº¨¤¦âªº Animator ²Õ¥ó
        GameObject hutao = GameObject.Find("主要虛擬導覽員");
        if (hutao != null)
        {
            hutaoAnimator = hutao.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("Cannot find GameObject named '主要虛擬導覽員'");
        }

        // ¶}©l¨óµ{¡Aµ¥«Ý¤­¬í«á°õ¦æ ActionZero ¤èªk
        //StartCoroutine(ExecuteActionZeroAfterDelay(5f));
    }

    IEnumerator ExecuteActionZeroAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ActionZero();
    }

    // ©w¸q¦UºØ action ªº¥\¯à
    public void ActionOne()
    {
        Debug.Log("Action One Executed!");
        // ¹ê»Ú¥\¯à¥N½X
    }

    public void ActionTwo()
    {
        Debug.Log("Action Two Executed!");
        // ¹ê»Ú¥\¯à¥N½X
    }

    public void ActionThree()
    {
        Debug.Log("Action Three Executed!");
        // ¹ê»Ú¥\¯à¥N½X
    }

    public void ActionZero()
    {
        Debug.Log("Action Zero Executed!");

        // ³]¸m isDancing ¬° true ¨ÓÄ²µo°Êµeª¬ºA¤Á´«
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
        // ®Ú¾Ú action ­È°õ¦æ¬ÛÀ³ªº¥\¯à
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
