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
    [Header("�޼��� ����")]
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
    /// �ѱ��� Ű�� �μ��� �����ϸ� C_Langiage���� ������ ���ڿ��� �޼����� ǥ��
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
    /// �޼��� �ڽ� �ִϸ��̼�
    /// </summary>
    private IEnumerator MessageBoxAnimation(string tp_message)
    {
        C_AudioManager.instance.PlayAuido(E_AudioType.ALERT);
        float t_timer = 0.0f;
        mp_text.text = tp_message;
        mp_messageBoxObject.SetActive(true);

        // �޼��� �ڽ� ���� Ŀ����.
        while (C_Constants.MESSAGEBOX_APPEARING_TIME > t_timer)
        {
            t_timer += Time.deltaTime;
            mp_messageBox.localScale = new Vector3(1.0f, t_timer * C_Constants.MESSAGEBOX_SCALEMULT_Y, 1.0f);
            yield return null;
        }
        mp_messageBox.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        // ���� �ð����� ���.
        t_timer = 0.0f;
        while (C_Constants.MESSAGEBOX_DELAY > t_timer)
        {
            t_timer += Time.deltaTime;
            yield return null;
        }

        // ��Ȱ��ȭ
        mp_messageBoxObject.SetActive(false);
        m_messageBoxStatus = C_Constants.MESSAGE_DONE;
    }


    private void Awake()
    {
        // ����Ƽ�� �̱�������
        instance = this;

        // �÷��̾� ���� ���� ǥ�� �ʱ�ȭ
        mp_playerBaseHitPointImage.fillAmount = 1.0f;

        // ����
        mp_messageBoxObject = mp_messageBox.gameObject;
    }


    private void Update()
    {
        // ����Ű ����, �����쿡�� ESC, �ȵ���̵忡�� �ڷΰ���
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ButtonMenu();
        }

        // ���� �޼��� ǥ��
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
