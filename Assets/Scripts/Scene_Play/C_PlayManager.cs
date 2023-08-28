using UnityEngine;

public delegate void D_PlayDelegate();

public class C_PlayManager : MonoBehaviour, I_StateMachine<E_PlayStates>
{
    /* ========== Fields ========== */

    [SerializeField] private GameObject mp_actor = null;
    [SerializeField] private C_MinionSettings mp_minionSettings = null;
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

    public Vector3 landForceBasePosition
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



    /* ========== Private Methods ========== */

    private void Awake()
    {
        // 유니티식 싱글턴패턴
        instance = this;

        // 플레이어 기지 위치
        playerBasePosition = transform.Find("PlayerBase").localPosition;

        // 대륙세력 기지 위치
        landForceBasePosition = transform.Find("LandForceBase").localPosition;
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
        mp_states[(int)E_PlayStates.GUIDEDMISSLE] = tp_state;
        C_CameraMove.instance.SetTargetTransform(E_PlayStates.GUIDEDMISSLE, ((C_GuidedMissle)tp_state).transform);

        C_Actor tp_actor = Instantiate(mp_actor).GetComponent<C_Actor>();
        tp_actor.ActorInitialize(tp_actSet);
        C_GuidedMissle.instance.SetActor(tp_actor.transform);
        mp_states[(int)E_PlayStates.ACTOR] = tp_actor;
        C_CameraMove.instance.SetTargetTransform(E_PlayStates.ACTOR, tp_actor.transform);

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

        // 늦은 FixedUpdate
        lateFixedUpdate?.Invoke();
    }
}
