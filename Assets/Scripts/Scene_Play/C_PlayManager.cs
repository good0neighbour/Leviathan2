using UnityEngine;

public delegate void D_PlayDelegate();

public class C_PlayManager : MonoBehaviour, I_StateMachine<E_PlayState>
{
    /* ========== Fields ========== */

    private I_State<E_PlayState>[] m_states = new I_State<E_PlayState>[(int)E_PlayState.END];
    private E_PlayState m_currentState = E_PlayState.ACTOR;

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



    /* ========== Public Methods ========== */

    public void SetState(E_PlayState t_state)
    {
        // 참조 변경
        m_currentState = t_state;
        C_CameraMove.instance.SetState(m_currentState);

        // 상태 실행
        m_states[(int)m_currentState].Execute();
    }



    /* ========== Private Methods ========== */

    private void Awake()
    {
        // 유니티식 싱글턴패턴
        instance = this;
    }


    private void Start()
    {
        // 상태 생성
        I_State<E_PlayState> tp_state = FindAnyObjectByType<C_AirPlane>();
        m_states[(int)E_PlayState.AIRPLANE] = tp_state;
        C_CameraMove.instance.SetTargetTransform(E_PlayState.AIRPLANE, ((C_AirPlane)tp_state).transform);

        tp_state = FindAnyObjectByType<C_GuidedMissle>();
        m_states[(int)E_PlayState.GUIDEDMISSLE] = tp_state;
        C_CameraMove.instance.SetTargetTransform(E_PlayState.GUIDEDMISSLE, ((C_GuidedMissle)tp_state).transform);

        tp_state = FindAnyObjectByType<C_Actor>();
        m_states[(int)E_PlayState.ACTOR] = tp_state;
        C_CameraMove.instance.SetTargetTransform(E_PlayState.ACTOR, ((C_Actor)tp_state).transform);

        // 상태 실행
        m_states[(int)m_currentState].Execute();
    }

    private void Update()
    {
        m_states[(int)m_currentState].StateUpdate();
    }

    private void FixedUpdate()
    {
        // 빠른 FixedUpdate
        earlyFixedUpdate?.Invoke();

        // 다형성
        m_states[(int)m_currentState].StateFixedUpdate();

        // 늦은 FixedUpdate
        lateFixedUpdate?.Invoke();
    }
}
