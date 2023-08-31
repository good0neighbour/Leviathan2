using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void D_PlayDelegate();

public class C_PlayManager : MonoBehaviour, I_StateMachine<E_PlayStates>
{
    /* ========== Fields ========== */

    [Header("�Ϲ�")]
    [SerializeField] private GameObject mp_actor = null;
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
            C_GameManager.instance.gameWin = true;
            SceneManager.LoadScene("Scene_End");
        }
    }


    public Vector3 RandomAllyDestination()
    {
        if (0 == m_numOfLandBase)
        {
            return m_oceanForceBasePosition;
        }
        else if (0 == m_numOfOceanBase)
        {
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



    /* ========== Private Methods ========== */

    private void Awake()
    {
        // ����Ƽ�� �̱�������
        instance = this;

        // �÷��̾� ���� ��ġ
        playerBasePosition = transform.Find("PlayerBase").localPosition;

        // ������� ���� ��ġ
        m_landForceBasePosition = transform.Find("LandForceBase").localPosition;

        // �ؾ缼�� ���� ��ġ
        m_oceanForceBasePosition = transform.Find("OceanForceBase").localPosition;
    }


    private void Start()
    {
        // Actor ���� �����´�.
        C_ActorSettings tp_actSet = Resources.Load<C_ActorSettings>("ActorSettings");

        // ���� ����
        I_State<E_PlayStates> tp_state = FindFirstObjectByType<C_AirPlane>(FindObjectsInactive.Include);
        mp_states[(int)E_PlayStates.AIRPLANE] = tp_state;
        C_CameraMove.instance.SetTargetTransform(E_PlayStates.AIRPLANE, ((C_AirPlane)tp_state).transform);

        tp_state = FindFirstObjectByType<C_GuidedMissle>(FindObjectsInactive.Include);
        mp_states[(int)E_PlayStates.GUIDEDMISSILE] = tp_state;
        C_CameraMove.instance.SetTargetTransform(E_PlayStates.GUIDEDMISSILE, ((C_GuidedMissle)tp_state).transform);

        C_Actor tp_actor = Instantiate(mp_actor).GetComponent<C_Actor>();
        tp_actor.ActorInitialize(tp_actSet);
        C_GuidedMissle.instance.SetActor(tp_actor.transform);
        mp_states[(int)E_PlayStates.ACTOR] = tp_actor;
        C_CameraMove.instance.SetTargetTransform(E_PlayStates.ACTOR, tp_actor.transform);

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
            0.0f,
            Mathf.Sin(Time.realtimeSinceStartup * m_waterWaveSpeed) * m_waterWaveScale + m_waterPositionOffset,
            0.0f
        );

        // ���� FixedUpdate
        lateFixedUpdate?.Invoke();
    }
}
