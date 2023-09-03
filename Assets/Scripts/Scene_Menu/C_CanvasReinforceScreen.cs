using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class C_CanvasReinforceScreen : MonoBehaviour
{
    /* ========== Fields ========== */

    [Header("�Ϲ�")]
    [SerializeField] private C_ActorInfomation mp_actorInformation = null;
    [SerializeField] private GameObject mp_actorListSlot = null;
    [SerializeField] private Transform mp_actorListArea = null;
    [SerializeField] private Image mp_actorFullImage = null;
    [SerializeField] private TextMeshProUGUI mp_informationText = null;
    [SerializeField] private TextMeshProUGUI mp_increasementText = null;
    [SerializeField] private TextMeshProUGUI mp_faithRemaining = null;
    [SerializeField] private GameObject mp_reinforceButton = null;
    [Header("�޴�ȭ��")]
    [SerializeField] private GameObject mp_previouseScreen = null;
    private byte m_currentIndex = 0;
    private bool m_isDisabled = true;



    /* ========== Public Methods ========== */

    public void ButtonActorList()
    {
        // ������ Actor �ε���
        m_currentIndex = byte.Parse(EventSystem.current.currentSelectedGameObject.name);

        // Actor ���� ǥ�� ������Ʈ
        ActorInfoUpdate();

        // ��Ȱ��ȭ ���¸� Ȱ��ȭ
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
        // ��ȭ
        C_GameManager.instance.IncreaseActorLevel(m_currentIndex);

        // Actor ���� ǥ�� ������Ʈ
        ActorInfoUpdate();

        // ���� �ž� ��
        uint t_remain = C_GameManager.instance.GetFaithRemaining();
        mp_faithRemaining.text = $"{C_Language.instance["�ž�"]} {t_remain}";

        // ���� �ž��� ������ ��ȭ ��ư ��Ȱ��ȭ
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
    /// Actor ���� ǥ�� ������Ʈ
    /// </summary>
    private void ActorInfoUpdate()
    {
        // ���� Actor ����
        C_ActorInfomation.S_Info tp_actInfo = mp_actorInformation.mp_actorInformation[m_currentIndex];
        byte t_actLv = C_GameManager.instance.GetActorLevel(m_currentIndex);

        // �̹��� ����
        mp_actorFullImage.sprite = tp_actInfo.mp_actorImageFull;

        // Actor ���� ǥ��
        mp_informationText.text = $"{tp_actInfo.mp_name}\n{C_Language.instance["��ȭ"]} {t_actLv}\n{C_Language.instance["���"]} {tp_actInfo.m_hitPoint + tp_actInfo.m_hitPointUp * t_actLv}\n{C_Language.instance["�̵� �ӷ�"]} {tp_actInfo.m_maxSpeed + tp_actInfo.m_maxSpeedUp * t_actLv}\n{C_Language.instance["���� �ӵ�"]} {tp_actInfo.m_conquestSpeed + tp_actInfo.m_conquestSpeedUp * t_actLv}\n{C_Language.instance["��ȣ�ۿ� ����"]} {tp_actInfo.m_interactRange + tp_actInfo.m_interactRangeUp * t_actLv}";

        // ������ ǥ��
        mp_increasementText.text = $"{C_Language.instance["��ȭ �� ������"]}\n\n{C_Language.instance["���"]} +{tp_actInfo.m_hitPointUp}\n{C_Language.instance["�̵� �ӷ�"]} +{tp_actInfo.m_maxSpeedUp}\n{C_Language.instance["���� �ӵ�"]} +{tp_actInfo.m_conquestSpeedUp}\n{C_Language.instance["��ȣ�ۿ� ����"]} +{tp_actInfo.m_interactRangeUp}";
    }


    private void Awake()
    {
        // Actor ��Ͽ� ���� ���� �� Actor �̹��� �߰�
        C_ActorInfomation.S_Info[] tp_actList = mp_actorInformation.mp_actorInformation;
        for (byte t_i = 0; t_i < tp_actList.Length; ++t_i)
        {
            GameObject tp_actSlot = Instantiate(mp_actorListSlot, mp_actorListArea);
            tp_actSlot.GetComponent<Image>().sprite = tp_actList[t_i].mp_actorPortrait;
            tp_actSlot.name = t_i.ToString();
            tp_actSlot.SetActive(true);
        }

        // ���� �ž� ��
        mp_faithRemaining.text = $"{C_Language.instance["�ž�"]} {C_GameManager.instance.GetFaithRemaining()}";
    }
}
