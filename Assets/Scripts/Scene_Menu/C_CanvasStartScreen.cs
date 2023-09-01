using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class C_CanvasStartScreen : MonoBehaviour
{
    /* ========== Fields ========== */

    [Header("�Ϲ�")]
    [SerializeField] private Image[] mp_selectedActors = new Image[3];
    [SerializeField] private Image[] mp_actorList = null;
    [SerializeField] private Sprite mp_unselectedImage = null;
    [SerializeField] private C_ActorInfomation mp_actorInformation = null;
    [Header("�޴� ȭ��")]
    [SerializeField] private GameObject mp_previouseScreen = null;
    private C_ActorInfomation.S_Info[] mp_currentActorList = new C_ActorInfomation.S_Info[C_Constants.NUM_OF_ACTOR_LIMIT];



    /* ========== Public Methods ========== */

    public void ButtonActorList(int t_index)
    {
        C_ActorInfomation.S_Info tp_actInfo = mp_actorInformation.mp_actorInformation[t_index];
        byte t_slot = C_Constants.NUM_OF_ACTOR_LIMIT;

        // �� �ڸ� Ȥ�� �̹� ���õƴ��� Ȯ��
        for (short t_i = C_Constants.NUM_OF_ACTOR_LIMIT - 1; t_i >= 0; --t_i)
        {
            if (tp_actInfo == mp_currentActorList[t_i])
            {
                // �̹� ���õ� ���̸� ���� ����
                ButtonSelectedActor(t_i);
                return;
            }
            else if (null == mp_currentActorList[t_i])
            {
                // �� �ڸ�
                t_slot = (byte)t_i;
            }
        }

        // �� �ڸ� ������ �߰�
        switch (t_slot)
        {
            case C_Constants.NUM_OF_ACTOR_LIMIT:
                return;

            default:
                mp_currentActorList[t_slot] = tp_actInfo;
                mp_selectedActors[t_slot].sprite = tp_actInfo.mp_actorPortrait;
                return;
        }
    }


    public void ButtonSelectedActor(int t_index)
    {
        // ���� ����
        mp_currentActorList[t_index] = null;
        mp_selectedActors[t_index].sprite = mp_unselectedImage;
    }


    public void ButtonStart()
    {
        C_GameManager.instance.SetActorList(mp_currentActorList);
        SceneManager.LoadScene("Scene_Play");
    }


    public void ButtonBack()
    {
        gameObject.SetActive(false);
        mp_previouseScreen.SetActive(true);
    }




    /* ========== Private Methods ========== */

    private void Awake()
    {
        for (byte t_i = 0; t_i < mp_actorList.Length; ++t_i)
        {
            mp_actorList[t_i].sprite = mp_actorInformation.mp_actorInformation[t_i].mp_actorImageThin;
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ButtonBack();
        }
    }
}
