using UnityEngine;

public delegate void D_PlayDelegate();

public class C_PlayManager : MonoBehaviour, I_StateMachine<E_PlayState>
{
    /* ========== Fields ========== */

    [SerializeField] private GameObject mp_actorHUDCanvas = null;
    private I_State<E_PlayState>[] m_states = new I_State<E_PlayState>[(int)E_PlayState.END];

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

    public D_PlayDelegate onStateChange
    {
        get;
        set;
    }

    public E_PlayState currentState
    {
        get;
        private set;
    }



    /* ========== Public Methods ========== */

    public void SetState(E_PlayState t_state)
    {
        // 참조 변경
        currentState = t_state;
        C_CameraMove.instance.SetState(currentState);

        // 상태 실행
        m_states[(int)currentState].Execute();

        // 대리자 호출
        onStateChange?.Invoke();
    }



    /* ========== Private Methods ========== */

    private void Awake()
    {
        // 유니티식 싱글턴패턴
        instance = this;
    }


    private void Start()
    {
        // Actor 설정 가져온다.
        C_ActorSettings tp_actSet = Resources.Load<C_ActorSettings>("ActorSettings");

        // 상태 생성
        I_State<E_PlayState> tp_state = FindFirstObjectByType<C_AirPlane>(FindObjectsInactive.Include);
        m_states[(int)E_PlayState.AIRPLANE] = tp_state;
        C_CameraMove.instance.SetTargetTransform(E_PlayState.AIRPLANE, ((C_AirPlane)tp_state).transform);

        tp_state = FindFirstObjectByType<C_GuidedMissle>(FindObjectsInactive.Include);
        m_states[(int)E_PlayState.GUIDEDMISSLE] = tp_state;
        C_CameraMove.instance.SetTargetTransform(E_PlayState.GUIDEDMISSLE, ((C_GuidedMissle)tp_state).transform);

        C_Actor t_actor = FindFirstObjectByType<C_Actor>(FindObjectsInactive.Include);
        t_actor.ActorInitialize(tp_actSet, mp_actorHUDCanvas);
        m_states[(int)E_PlayState.ACTOR] = t_actor;
        C_CameraMove.instance.SetTargetTransform(E_PlayState.ACTOR, t_actor.transform);

        // 처음 상태
        currentState = E_PlayState.AIRPLANE;

        // 상태 실행
        SetState(currentState);
    }


    private void Update()
    {
        m_states[(int)currentState].StateUpdate();
    }


    private void FixedUpdate()
    {
        // 빠른 FixedUpdate
        earlyFixedUpdate?.Invoke();

        // 다형성
        m_states[(int)currentState].StateFixedUpdate();

        // 늦은 FixedUpdate
        lateFixedUpdate?.Invoke();
    }
}
