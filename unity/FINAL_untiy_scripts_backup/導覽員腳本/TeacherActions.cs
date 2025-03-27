using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeacherActions : MonoBehaviour
{
    private Animator hutaoAnimator;

    void Start()
    {
        // ����W�٬� "�D�n����������" �����⪺ Animator �ե�
        GameObject hutao = GameObject.Find("�D�n����������");
        if (hutao != null)
        {
            hutaoAnimator = hutao.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("Cannot find GameObject named '�D�n����������'");
        }

        // �}�l��{�A���ݤ������� ActionZero ��k
        //StartCoroutine(ExecuteActionZeroAfterDelay(5f));
    }

    IEnumerator ExecuteActionZeroAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ActionZero();
    }

    // �w�q�U�� action ���\��
    public void ActionOne()
    {
        Debug.Log("Action One Executed!");
        // ��ڥ\��N�X
    }

    public void ActionTwo()
    {
        Debug.Log("Action Two Executed!");
        // ��ڥ\��N�X
    }

    public void ActionThree()
    {
        Debug.Log("Action Three Executed!");
        // ��ڥ\��N�X
    }

    public void ActionZero()
    {
        Debug.Log("Action Zero Executed!");

        // �]�m isDancing �� true ��Ĳ�o�ʵe���A����
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
        // �ھ� action �Ȱ���������\��
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
