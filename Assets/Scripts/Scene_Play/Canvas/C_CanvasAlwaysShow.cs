using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class C_CanvasAlwaysShow : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private Image mp_playerBaseHitPointImage = null;
    [SerializeField] private TextMeshProUGUI mp_landForceNum = null;
    [SerializeField] private TextMeshProUGUI mp_oceanForceNum = null;
    [SerializeField] private GameObject mp_canvasMenu = null;
    [Header("메세지 상자")]
    [SerializeField] private RectTransform mp_messageBox = null;
    [SerializeField] private TextMeshProUGUI mp_text = null;
    private Queue<string> mp_messageQueue = new Queue<string>();
    private GameObject mp_messageBoxObject = null;
    private byte m_messageBoxStatus = 0;

    public static C_CanvasAlwaysShow instance
    {
        get;
        private set;
    }



    /* ========== Public Methodes ========== */

    public void SetPlayerBaseHitPointImage(float t_amount)
    {
        mp_playerBaseHitPointImage.fillAmount = t_amount;
    }


    public void SetEnemyBaseNum(byte t_land, byte t_ocean)
    {
        mp_landForceNum.text = t_land.ToString();
        mp_oceanForceNum.text = t_ocean.ToString();
    }


    public void ButtonMenu()
    {
        C_AudioManager.instance.PlayAuido(E_AudioType.TOUCH);
        Time.timeScale = 0;
        mp_canvasMenu.SetActive(true);
    }


    /// <summary>
    /// 한국어 키를 인수로 전달하면 C_Langiage에서 번역된 문자열을 메세지로 표시
    /// </summary>
    public void DisplayMessage(string tp_message)
    {
        switch (m_messageBoxStatus)
        {
            case C_Constants.MESSAGE_STANDBY:
                mp_messageBoxObject.SetActive(true);
                StartCoroutine(MessageBoxAnimation(C_Language.instance[tp_message]));
                m_messageBoxStatus = C_Constants.MESSAGE_SHOWING;
                return;

            case C_Constants.MESSAGE_SHOWING:
                mp_messageQueue.Enqueue(C_Language.instance[tp_message]);
                return;
        }
    }



    /* ========== Private Methodes ========== */

    /// <summary>
    /// 메세지 박스 애니메이션
    /// </summary>
    private IEnumerator MessageBoxAnimation(string tp_message)
    {
        C_AudioManager.instance.PlayAuido(E_AudioType.ALERT);
        float t_timer = 0.0f;
        mp_text.text = tp_message;
        mp_messageBoxObject.SetActive(true);

        // 메세지 박스 점점 커진다.
        while (C_Constants.MESSAGEBOX_APPEARING_TIME > t_timer)
        {
            t_timer += Time.deltaTime;
            mp_messageBox.localScale = new Vector3(1.0f, t_timer * C_Constants.MESSAGEBOX_SCALEMULT_Y, 1.0f);
            yield return null;
        }
        mp_messageBox.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        // 일정 시간동안 대기.
        t_timer = 0.0f;
        while (C_Constants.MESSAGEBOX_DELAY > t_timer)
        {
            t_timer += Time.deltaTime;
            yield return null;
        }

        // 비활성화
        mp_messageBoxObject.SetActive(false);
        m_messageBoxStatus = C_Constants.MESSAGE_DONE;
    }


    private void Awake()
    {
        // 유니티식 싱글턴패턴
        instance = this;

        // 플레이어 기지 방어력 표시 초기화
        mp_playerBaseHitPointImage.fillAmount = 1.0f;

        // 참조
        mp_messageBoxObject = mp_messageBox.gameObject;
    }


    private void Update()
    {
        // 단축키 동작, 윈도우에서 ESC, 안드로이드에서 뒤로가기
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ButtonMenu();
        }

        // 다음 메세지 표시
        switch (m_messageBoxStatus)
        {
            case C_Constants.MESSAGE_DONE:
                if (0 < mp_messageQueue.Count)
                {
                    StartCoroutine(MessageBoxAnimation(mp_messageQueue.Dequeue()));
                    m_messageBoxStatus = C_Constants.MESSAGE_SHOWING;
                }
                else
                {
                    m_messageBoxStatus = C_Constants.MESSAGE_STANDBY;
                }
                return;
        }
    }
}
