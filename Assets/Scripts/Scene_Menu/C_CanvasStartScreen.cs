using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class C_CanvasStartScreen : MonoBehaviour
{
    /* ========== Fields ========== */

    [Header("일반")]
    [SerializeField] private Image[] mp_selectedActors = null;
    [SerializeField] private GameObject[] mp_bottomButtons = null;
    [SerializeField] private RectTransform mp_listArea = null;
    [SerializeField] private Sprite mp_unselectedImage = null;
    [SerializeField] private C_ActorInfomation mp_actorInformation = null;
    [SerializeField] private GameObject mp_actorListSlot = null;
    [SerializeField] private Transform mp_actorListArea = null;
    [Header("메뉴 화면")]
    [SerializeField] private GameObject mp_previousScreen = null;
    [SerializeField] private GameObject mp_nextScreen = null;
    private C_ActorInfomation.S_Info[] mp_currentActorList = new C_ActorInfomation.S_Info[C_Constants.NUM_OF_ACTOR_LIMIT];
    private byte[] mp_currentActorIndexs = new byte[C_Constants.NUM_OF_ACTOR_LIMIT];
    private float m_timer = 0.0f;
    private bool m_buttonAvailable = false;



    /* ========== Public Methods ========== */

    public void ButtonActorList()
    {
        if (!m_buttonAvailable)
        {
            return;
        }

        // Actor 인덱스
        byte actInx = byte.Parse(EventSystem.current.currentSelectedGameObject.name);

        // 해당 Actor 정보
        C_ActorInfomation.S_Info tp_actInfo = mp_actorInformation.mp_actorInformation[actInx];
        byte t_slot = C_Constants.NUM_OF_ACTOR_LIMIT;

        // 빈 자리 혹은 이미 선택됐는지 확인
        for (short t_i = C_Constants.NUM_OF_ACTOR_LIMIT - 1; t_i >= 0; --t_i)
        {
            if (tp_actInfo == mp_currentActorList[t_i])
            {
                // 이미 선택된 것이면 선택 해제
                ButtonSelectedActor(t_i);
                return;
            }
            else if (null == mp_currentActorList[t_i])
            {
                // 빈 자리
                t_slot = (byte)t_i;
            }
        }

        // 빈 자리 있으면 추가
        switch (t_slot)
        {
            case C_Constants.NUM_OF_ACTOR_LIMIT:
                return;

            default:
                mp_currentActorList[t_slot] = tp_actInfo;
                mp_currentActorIndexs[t_slot] = actInx;
                mp_selectedActors[t_slot].sprite = tp_actInfo.mp_actorPortrait;
                return;
        }
    }


    public void ButtonSelectedActor(int t_index)
    {
        if (!m_buttonAvailable)
        {
            return;
        }

        // 선택 해제
        mp_currentActorList[t_index] = null;
        mp_selectedActors[t_index].sprite = mp_unselectedImage;
    }


    public void ButtonStart()
    {
        if (!m_buttonAvailable)
        {
            return;
        }

        // 다음 화면
        C_GameManager.instance.SetActorList(mp_currentActorList, mp_currentActorIndexs);
        gameObject.SetActive(false);
        mp_nextScreen.SetActive(true);
    }


    public void ButtonBack()
    {
        if (!m_buttonAvailable)
        {
            return;
        }

        // 화면 전환
        gameObject.SetActive(false);
        mp_previousScreen.SetActive(true);
        m_buttonAvailable = false;
        m_timer = 0.0f;

        // 화면 초기화
        for (byte t_i = 0; t_i < C_Constants.NUM_OF_ACTOR_LIMIT; ++t_i)
        {
            mp_currentActorList[t_i] = null;
            mp_selectedActors[t_i].sprite = mp_unselectedImage;
        }
        foreach (Image tp_img in mp_selectedActors)
        {
            tp_img.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }
        foreach (GameObject tp_button in mp_bottomButtons)
        {
            tp_button.SetActive(false);
        }
        mp_listArea.anchorMax = new Vector2(mp_listArea.anchorMax.x, 0.1f);
    }




    /* ========== Private Methods ========== */

    private void Awake()
    {
        // Actor 목록에 슬롯 생성 밑 Actor 이미지 추가
        C_ActorInfomation.S_Info[] tp_actList = mp_actorInformation.mp_actorInformation;
        for (byte t_i = 0; t_i < tp_actList.Length; ++t_i)
        {
            GameObject tp_actSlot = Instantiate(mp_actorListSlot, mp_actorListArea);
            tp_actSlot.GetComponent<Image>().sprite = tp_actList[t_i].mp_actorImageThin;
            tp_actSlot.name = t_i.ToString();
            tp_actSlot.SetActive(true);
        }
    }


    private void Update()
    {
        // 처음 화면 애니메이션
        if (!m_buttonAvailable)
        {
            if (1.0f > m_timer)
            {
                m_timer += Time.deltaTime;
                if (1.0f < m_timer)
                {
                    m_timer = 1.0f;
                }
                foreach (Image tp_img in mp_selectedActors)
                {
                    tp_img.color = new Color(1.0f, 1.0f, 1.0f, m_timer);
                }
            }
            else
            {
                m_timer += Time.deltaTime;
                if (2.0f < m_timer)
                {
                    m_timer = 2.0f;
                    m_buttonAvailable = true;
                    foreach (GameObject tp_button in mp_bottomButtons)
                    {
                        tp_button.SetActive(true);
                    }
                }
                mp_listArea.anchorMax = new Vector2(mp_listArea.anchorMax.x, Mathf.Cos(m_timer * Mathf.PI) * 0.2f + 0.3f);
            }
        }

        // 단축키 동작, 윈도우에서 ESC, 안드로이드에서 뒤로가기
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ButtonBack();
        }
    }
}
