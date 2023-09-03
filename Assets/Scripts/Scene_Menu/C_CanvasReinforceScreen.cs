using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class C_CanvasReinforceScreen : MonoBehaviour
{
    /* ========== Fields ========== */

    [Header("일반")]
    [SerializeField] private C_ActorInfomation mp_actorInformation = null;
    [SerializeField] private GameObject mp_actorListSlot = null;
    [SerializeField] private Transform mp_actorListArea = null;
    [SerializeField] private Image mp_actorFullImage = null;
    [SerializeField] private TextMeshProUGUI mp_informationText = null;
    [SerializeField] private TextMeshProUGUI mp_increasementText = null;
    [SerializeField] private TextMeshProUGUI mp_faithRemaining = null;
    [SerializeField] private GameObject mp_reinforceButton = null;
    [Header("메뉴화면")]
    [SerializeField] private GameObject mp_previouseScreen = null;
    private byte m_currentIndex = 0;
    private bool m_isDisabled = true;



    /* ========== Public Methods ========== */

    public void ButtonActorList()
    {
        // 선택한 Actor 인덱스
        m_currentIndex = byte.Parse(EventSystem.current.currentSelectedGameObject.name);

        // Actor 정보 표시 업데이트
        ActorInfoUpdate();

        // 비활성화 상태면 활성화
        if (m_isDisabled)
        {
            m_isDisabled = false;
            mp_actorFullImage.gameObject.SetActive(true);
            switch (C_GameManager.instance.GetFaithRemaining())
            {
                case 0:
                    return;

                default:
                    mp_reinforceButton.gameObject.SetActive(true);
                    return;
            }
        }
    }


    public void ButtonReinforce()
    {
        // 강화
        C_GameManager.instance.IncreaseActorLevel(m_currentIndex);

        // Actor 정보 표시 업데이트
        ActorInfoUpdate();

        // 남은 신앙 수
        uint t_remain = C_GameManager.instance.GetFaithRemaining();
        mp_faithRemaining.text = $"{C_Language.instance["신앙"]} {t_remain}";

        // 남은 신앙이 없으면 강화 버튼 비활성화
        switch (t_remain)
        {
            case 0:
                mp_reinforceButton.gameObject.SetActive(false);
                return;
        }
    }


    public void ButtonBack()
    {
        m_isDisabled = true;
        mp_actorFullImage.gameObject.SetActive(false);
        mp_reinforceButton.gameObject.SetActive(false);
        gameObject.SetActive(false);
        mp_previouseScreen.SetActive(true);
    }



    /* ========== Private Methods ========== */

    /// <summary>
    /// Actor 정보 표시 업데이트
    /// </summary>
    private void ActorInfoUpdate()
    {
        // 현재 Actor 정보
        C_ActorInfomation.S_Info tp_actInfo = mp_actorInformation.mp_actorInformation[m_currentIndex];
        byte t_actLv = C_GameManager.instance.GetActorLevel(m_currentIndex);

        // 이미지 변경
        mp_actorFullImage.sprite = tp_actInfo.mp_actorImageFull;

        // Actor 정보 표시
        mp_informationText.text = $"{tp_actInfo.mp_name}\n{C_Language.instance["강화"]} {t_actLv}\n{C_Language.instance["방어"]} {tp_actInfo.m_hitPoint + tp_actInfo.m_hitPointUp * t_actLv}\n{C_Language.instance["이동 속력"]} {tp_actInfo.m_maxSpeed + tp_actInfo.m_maxSpeedUp * t_actLv}\n{C_Language.instance["점령 속도"]} {tp_actInfo.m_conquestSpeed + tp_actInfo.m_conquestSpeedUp * t_actLv}\n{C_Language.instance["상호작용 범위"]} {tp_actInfo.m_interactRange + tp_actInfo.m_interactRangeUp * t_actLv}";

        // 증가량 표시
        mp_increasementText.text = $"{C_Language.instance["강화 시 증가량"]}\n\n{C_Language.instance["방어"]} +{tp_actInfo.m_hitPointUp}\n{C_Language.instance["이동 속력"]} +{tp_actInfo.m_maxSpeedUp}\n{C_Language.instance["점령 속도"]} +{tp_actInfo.m_conquestSpeedUp}\n{C_Language.instance["상호작용 범위"]} +{tp_actInfo.m_interactRangeUp}";
    }


    private void Awake()
    {
        // Actor 목록에 슬롯 생성 밑 Actor 이미지 추가
        C_ActorInfomation.S_Info[] tp_actList = mp_actorInformation.mp_actorInformation;
        for (byte t_i = 0; t_i < tp_actList.Length; ++t_i)
        {
            GameObject tp_actSlot = Instantiate(mp_actorListSlot, mp_actorListArea);
            tp_actSlot.GetComponent<Image>().sprite = tp_actList[t_i].mp_actorPortrait;
            tp_actSlot.name = t_i.ToString();
            tp_actSlot.SetActive(true);
        }

        // 남은 신앙 수
        mp_faithRemaining.text = $"{C_Language.instance["신앙"]} {C_GameManager.instance.GetFaithRemaining()}";
    }
}
