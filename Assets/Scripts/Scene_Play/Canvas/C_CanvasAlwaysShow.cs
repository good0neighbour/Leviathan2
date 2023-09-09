using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class C_CanvasAlwaysShow : MonoBehaviour
{
    /* ========== Fields ========== */

    [Header("상단")]
    [SerializeField] private Image mp_playerBaseHitPointImage = null;
    [SerializeField] private TextMeshProUGUI mp_landForceNum = null;
    [SerializeField] private TextMeshProUGUI mp_oceanForceNum = null;
    [SerializeField] private GameObject mp_canvasMenu = null;
    [Header("메세지 상자")]
    [SerializeField] private RectTransform mp_messageBox = null;
    [SerializeField] private Image mp_portrait = null;
    [SerializeField] private TextMeshProUGUI mp_name = null;
    [SerializeField] private TextMeshProUGUI mp_text = null;
    [Header("발표자 초상화")]
    [SerializeField] private Sprite mp_aidePortrait = null;
    [SerializeField] private Sprite mp_landForcePortrait = null;
    [SerializeField] private Sprite mp_oceanForcePortrait = null;
    private Queue<S_MessageContent> mp_messageQueue = new Queue<S_MessageContent>();
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
    public void DisplayMessage(string tp_message, C_ActorInfomation.S_Info t_actorInfo)
    {
        DisplayMessage(new S_MessageContent(
            t_actorInfo.mp_actorPortrait,
            t_actorInfo.mp_name,
            C_Language.instance[tp_message]
        ));
    }


    /// <summary>
    /// 한국어 키를 인수로 전달하면 C_Langiage에서 번역된 문자열을 메세지로 표시
    /// </summary>
    public void DisplayMessage(string tp_message, E_MessageAnnouncer t_announcer)
    {
        switch (t_announcer)
        {
            case E_MessageAnnouncer.AIDE:
                DisplayMessage(new S_MessageContent(
                    mp_aidePortrait,
                    C_Language.instance["부관"],
                    C_Language.instance[tp_message]
                ));
                break;

            case E_MessageAnnouncer.LANDFORCE:
                DisplayMessage(new S_MessageContent(
                    mp_landForcePortrait,
                    C_Language.instance["대륙세력"],
                    C_Language.instance[tp_message]
                ));
                break;

            case E_MessageAnnouncer.OCEANFORCE:
                DisplayMessage(new S_MessageContent(
                    mp_oceanForcePortrait,
                    C_Language.instance["해양세력"],
                    C_Language.instance[tp_message]
                ));
                break;

#if UNITY_EDITOR
            default:
                Debug.LogError("C_CanvasAlwaysShow.DisplayMessage : 잘못된 발표자");
                break;
#endif
        }
    }



    /* ========== Private Methodes ========== */

    private void DisplayMessage(S_MessageContent t_content)
    {
        switch (m_messageBoxStatus)
        {
            case C_Constants.MESSAGE_STANDBY:
                StartCoroutine(MessageBoxAnimation(t_content));
                m_messageBoxStatus = C_Constants.MESSAGE_SHOWING;
                return;

            default:
                mp_messageQueue.Enqueue(t_content);
                return;
        }
    }


    /// <summary>
    /// 메세지 박스 애니메이션
    /// </summary>
    private IEnumerator MessageBoxAnimation(S_MessageContent t_content)
    {
        C_AudioManager.instance.PlayAuido(E_AudioType.ALERT);
        float t_timer = 0.0f;
        mp_portrait.sprite = t_content.mp_portrait;
        mp_name.text = t_content.mp_name;
        mp_text.text = t_content.mp_text;
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

            default:
                return;
        }
    }



    /* ========== Structure ========== */

    private struct S_MessageContent
    {
        public Sprite mp_portrait;
        public string mp_name;
        public string mp_text;

        public S_MessageContent(Sprite tp_portrait, string tp_name, string tp_text)
        {
            mp_portrait = tp_portrait;
            mp_name = tp_name;
            mp_text = tp_text;
        }
    }
}
