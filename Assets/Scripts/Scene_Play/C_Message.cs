using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class C_Message : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private RectTransform mp_messageBox = null;
    [SerializeField] private TextMeshProUGUI mp_text = null;
    private Queue<string> mp_messageQueue = new Queue<string>();
    private float m_timer = 0.0f;



    /* ========== Public Methods ========== */

    /// <summary>
    /// 한국어 키를 인수로 전달하면 C_Langiage에서 번역된 문자열을 메세지로 표시
    /// </summary>
    public void DisplayMessage(string tp_message)
    {
        switch (mp_messageQueue.Count)
        {
            case 0:
                StopAllCoroutines();
                StartCoroutine(MessageBoxAnimation(C_Language.instance[tp_message]));
                return;

            default:
                mp_messageQueue.Enqueue(C_Language.instance[tp_message]);
                return;
        }
    }



    /* ========== Private Methods ========== */

    private IEnumerator MessageBoxAnimation(string tp_message)
    {
        float t_timer = 0.0f;
        mp_text.text = tp_message;
        while (C_Constants.MESSAGEBOX_APPEARING_TIME > t_timer)
        {
            t_timer += Time.deltaTime;
            mp_messageBox.localScale = new Vector3(1.0f, t_timer * C_Constants.MESSAGEBOX_SCALEMULT_Y, 1.0f);
            yield return null;
        }
        mp_messageBox.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }


    private void Update()
    {
        if (C_Constants.MESSAGEBOX_DELAY < m_timer)
        {
            switch (mp_messageQueue.Count)
            {
                case 0:
                    return;

                default:
                    m_timer = 0.0f;
                    StopAllCoroutines();
                    StartCoroutine(MessageBoxAnimation(C_Language.instance[mp_messageQueue.Dequeue()]));
                    return;
            }
        }
        else
        {
            m_timer += Time.deltaTime;
        }
    }
}
