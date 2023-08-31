using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void D_PlayDelegate();

public class C_PlayManager : MonoBehaviour, I_StateMachine<E_PlayStates>
{
    /* ========== Fields ========== */

    [Header("일반")]
    [SerializeField] private GameObject mp_actor = null;
    [SerializeField] private C_MinionSettings mp_minionSettings = null;
    [SerializeField] private Material mp_playerFlagMaterial = null;
    [SerializeField] private Sprite mp_playerFlagSprite = null;
    [SerializeField] private byte m_numOfLandBase = 6;
    [SerializeField] private byte m_numOfOceanBase = 6;
    [Header("물")]
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
        // 참조 변경
        currentState = t_state;
        C_CameraMove.instance.SetState(currentState);

        // 상태 실행
        mp_states[(int)currentState].Execute();
    }


    public C_MinionSettings GetEnemySettings()
    {
        return mp_minionSettings;
    }


    /// <summary>
    /// 남은 적 기지 수 갱신
    /// </summary>
    public void EnemyBaseConquested(E_ObjectPool t_enemyForce)
    {
        // 적 기지 수 감소
        switch (t_enemyForce)
        {
            case E_ObjectPool.ATTACKENEMY_LANDFORCE:
                --m_numOfLandBase;
                break;
            case E_ObjectPool.ATTACKENEMY_OCEANFORCE:
                --m_numOfOceanBase;
                break;
        }

        // HUD 업데이트
        C_CanvasAlwaysShow.instance.SetEnemyBaseNum(m_numOfLandBase, m_numOfOceanBase);

        // 게임 종료 확인
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
        // 유니티식 싱글턴패턴
        instance = this;

        // 플레이어 기지 위치
        playerBasePosition = transform.Find("PlayerBase").localPosition;

        // 대륙세력 기지 위치
        m_landForceBasePosition = transform.Find("LandForceBase").localPosition;

        // 해양세력 기지 위치
        m_oceanForceBasePosition = transform.Find("OceanForceBase").localPosition;
    }


    private void Start()
    {
        // Actor 설정 가져온다.
        C_ActorSettings tp_actSet = Resources.Load<C_ActorSettings>("ActorSettings");

        // 상태 생성
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

        // HUD 업데이트
        C_CanvasAlwaysShow.instance.SetEnemyBaseNum(m_numOfLandBase, m_numOfOceanBase);

        // 처음 상태
        currentState = E_PlayStates.AIRPLANE;

        // 상태 실행
        SetState(currentState);
    }


    private void Update()
    {
        mp_states[(int)currentState].StateUpdate();
    }


    private void FixedUpdate()
    {
        // 빠른 FixedUpdate
        earlyFixedUpdate?.Invoke();

        // 다형성
        mp_states[(int)currentState].StateFixedUpdate();

        // 수면 움직임
        mp_waterTransform.localPosition = new Vector3(
            0.0f,
            Mathf.Sin(Time.realtimeSinceStartup * m_waterWaveSpeed) * m_waterWaveScale + m_waterPositionOffset,
            0.0f
        );

        // 늦은 FixedUpdate
        lateFixedUpdate?.Invoke();
    }
}
