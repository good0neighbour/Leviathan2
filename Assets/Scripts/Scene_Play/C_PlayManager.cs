using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void D_PlayDelegate();

public class C_PlayManager : MonoBehaviour, I_StateMachine<E_PlayStates>
{
    /* ========== Fields ========== */

    [Header("�Ϲ�")]
    [SerializeField] private C_MinionSettings mp_minionSettings = null;
    [SerializeField] private Material mp_playerFlagMaterial = null;
    [SerializeField] private Sprite mp_playerFlagSprite = null;
    [SerializeField] private byte m_numOfLandBase = 6;
    [SerializeField] private byte m_numOfOceanBase = 6;
    [Header("��")]
    [SerializeField] private Transform mp_waterTransform = null;
    [SerializeField] private float m_waterPositionOffset = -2.0f;
    [SerializeField] private float m_waterWaveScale = 0.5f;
    [SerializeField] private float m_waterWaveSpeed = 0.5f;
#if UNITY_EDITOR
    [Header("����� �뵵")]
    [SerializeField] private GameObject mp_audioManager = null;
#endif
    private C_Actor[] mp_actors = new C_Actor[C_Constants.NUM_OF_ACTOR_LIMIT];
    private Vector3 m_landForceBasePosition = Vector3.zero;
    private Vector3 m_oceanForceBasePosition = Vector3.zero;
    private I_State<E_PlayStates>[] mp_states = new I_State<E_PlayStates>[(int)E_PlayStates.END];

    public static C_PlayManager instance
    {
        get;
        set;
    }

    public D_PlayDelegate earlyFixedUpdate
    {
        get;
        set;
    }

    public D_PlayDelegate lateFixedUpdate
    {
        get;
        set;
    }

    public Vector3 playerBasePosition
    {
        get;
        private set;
    }

    public E_PlayStates currentState
    {
        get;
        private set;
    }



    /* ========== Public Methods ========== */

    public void SetState(E_PlayStates t_state)
    {
        // ���� ����
        currentState = t_state;
        C_CameraMove.instance.SetState(currentState);

        // ���� ����
        mp_states[(int)currentState].Execute();
    }


    public C_MinionSettings GetEnemySettings()
    {
        return mp_minionSettings;
    }


    /// <summary>
    /// ���� �� ���� �� ����
    /// </summary>
    public void EnemyBaseConquested(E_ObjectPool t_enemyForce)
    {
        // �� ���� �� ����
        switch (t_enemyForce)
        {
            case E_ObjectPool.ATTACKENEMY_LANDFORCE:
                --m_numOfLandBase;
                break;
            case E_ObjectPool.ATTACKENEMY_OCEANFORCE:
                --m_numOfOceanBase;
                break;
        }

        // HUD ������Ʈ
        C_CanvasAlwaysShow.instance.SetEnemyBaseNum(m_numOfLandBase, m_numOfOceanBase);

        // ���� ���� Ȯ��
        if (0 == m_numOfLandBase && 0 == m_numOfOceanBase)
        {
            GameEnd(true);
        }
    }


    public Vector3 RandomAllyDestination()
    {
        if (0 == m_numOfLandBase)
        {
            C_CanvasAlwaysShow.instance.DisplayMessage("��������� ��� ������ �����߽��ϴ�.");
            return m_oceanForceBasePosition;
        }
        else if (0 == m_numOfOceanBase)
        {
            C_CanvasAlwaysShow.instance.DisplayMessage("�ؾ缼���� ��� ������ �����߽��ϴ�.");
            return m_landForceBasePosition;
        }
        else
        {
            if (0 < Random.Range(0, 2))
            {
                return m_landForceBasePosition;
            }
            else
            {
                return m_oceanForceBasePosition;
            }
        }
    }


    public Material GetPlayerFlagMaterial()
    {
        return mp_playerFlagMaterial;
    }


    public Sprite GetPlayerFlagSprite()
    {
        return mp_playerFlagSprite;
    }


    public  Transform SetCurrentActor(byte t_index)
    {
        switch (mp_actors[t_index])
        {
            case null:
                return null;

            default:
                mp_states[(int)E_PlayStates.ACTOR] = mp_actors[t_index];
                Transform tp_actTrans = mp_actors[t_index].transform;
                C_CameraMove.instance.SetTargetTransform(E_PlayStates.ACTOR, tp_actTrans);
                return tp_actTrans;
        }
    }


    public void GameEnd(bool m_win)
    {
        C_GameManager.instance.gameWin = m_win;
        SceneManager.LoadScene("Scene_End");
    }    



    /* ========== Private Methods ========== */

    private void Awake()
    {
#if UNITY_EDITOR
        // AudioManager ������ ����
        switch (C_AudioManager.instance)
        {
            case null:
                GameObject tp_audMgr = Instantiate(mp_audioManager);
                DontDestroyOnLoad(tp_audMgr);
                C_AudioManager.instance = tp_audMgr.GetComponent<C_AudioManager>();
                break;

            default:
                break;
        }
#endif

        // ����Ƽ�� �̱�������
        instance = this;

        // �÷��̾� ���� ��ġ
        playerBasePosition = transform.Find("PlayerBase").localPosition;

        // ������� ���� ��ġ
        m_landForceBasePosition = transform.Find("LandForceBase").localPosition;

        // �ؾ缼�� ���� ��ġ
        m_oceanForceBasePosition = transform.Find("OceanForceBase").localPosition;

        // �ڵ� ���� �۵�
        foreach (C_AutoTranslation tp_auTra in FindObjectsOfType<C_AutoTranslation>(true))
        {
            tp_auTra.TranslationReady();
        }

        // ��� �ҷ��´�
        C_Language.instance.LoadLangeage(C_GameManager.instance.currentLanguage);
    }


    private void Start()
    {
        // Actor ���� �����´�.
        C_ActorSettings tp_actSet = Resources.Load<C_ActorSettings>("ActorSettings");

        // ����� ���� ����
        I_State<E_PlayStates> tp_state = FindFirstObjectByType<C_AirPlane>(FindObjectsInactive.Include);
        mp_states[(int)E_PlayStates.AIRPLANE] = tp_state;
        C_CameraMove.instance.SetTargetTransform(E_PlayStates.AIRPLANE, ((C_AirPlane)tp_state).transform);

        // ���̵�̻��� ���� ����
        tp_state = FindFirstObjectByType<C_GuidedMissle>(FindObjectsInactive.Include);
        mp_states[(int)E_PlayStates.GUIDEDMISSILE] = tp_state;
        C_CameraMove.instance.SetTargetTransform(E_PlayStates.GUIDEDMISSILE, ((C_GuidedMissle)tp_state).transform);

        // Actor ���� ����
        byte[] mp_actsLv;
        C_ActorInfomation.S_Info[] tp_actInfoList = C_GameManager.instance.GetActorList(out mp_actsLv);
        for (byte t_i = 0; t_i < tp_actInfoList.Length; ++t_i)
        {
            switch (tp_actInfoList[t_i])
            {
                case null:
                    mp_actors[t_i] = null;
                    break;

                default:
                    C_Actor tp_actor = Instantiate(tp_actInfoList[t_i].mp_prefab).GetComponent<C_Actor>();
                    tp_actor.ActorInitialize(tp_actSet, tp_actInfoList[t_i], t_i, mp_actsLv[t_i]);
                    mp_actors[t_i] = tp_actor;
                    break;
            }
        }

        // ���̵�̻��Ͽ� Actor ���� ����
        C_GuidedMissle.instance.SetActorSlot(tp_actInfoList);

        // HUD ������Ʈ
        C_CanvasAlwaysShow.instance.SetEnemyBaseNum(m_numOfLandBase, m_numOfOceanBase);

        // ó�� ����
        currentState = E_PlayStates.AIRPLANE;

        // ���� ����
        SetState(currentState);
    }


    private void Update()
    {
        mp_states[(int)currentState].StateUpdate();
    }


    private void FixedUpdate()
    {
        // ���� FixedUpdate
        earlyFixedUpdate?.Invoke();

        // ������
        mp_states[(int)currentState].StateFixedUpdate();

        // ���� ������
        mp_waterTransform.localPosition = new Vector3(
            1000.0f,
            Mathf.Sin(Time.realtimeSinceStartup * m_waterWaveSpeed) * m_waterWaveScale + m_waterPositionOffset,
            750.0f
        );

        // ���� FixedUpdate
        lateFixedUpdate?.Invoke();
    }
}
