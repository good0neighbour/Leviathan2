using UnityEngine;

public delegate void D_PlayDelegate();

public class C_PlayManager : MonoBehaviour, I_StateMachine<E_PlayState>
{
    /* ========== Fields ========== */

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

    public E_PlayState currentState
    {
        get;
        private set;
    }



    /* ========== Public Methods ========== */

    public void SetState(E_PlayState t_state)
    {
        // ���� ����
        currentState = t_state;
        C_CameraMove.instance.SetState(currentState);

        // ���� ����
        m_states[(int)currentState].Execute();
    }



    /* ========== Private Methods ========== */

    private void Awake()
    {
        // ����Ƽ�� �̱�������
        instance = this;
    }


    private void Start()
    {
        // ���� ����
        I_State<E_PlayState> tp_state = FindAnyObjectByType<C_AirPlane>();
        m_states[(int)E_PlayState.AIRPLANE] = tp_state;
        C_CameraMove.instance.SetTargetTransform(E_PlayState.AIRPLANE, ((C_AirPlane)tp_state).transform);

        tp_state = FindAnyObjectByType<C_GuidedMissle>();
        m_states[(int)E_PlayState.GUIDEDMISSLE] = tp_state;
        C_CameraMove.instance.SetTargetTransform(E_PlayState.GUIDEDMISSLE, ((C_GuidedMissle)tp_state).transform);

        tp_state = FindAnyObjectByType<C_Actor>();
        m_states[(int)E_PlayState.ACTOR] = tp_state;
        C_CameraMove.instance.SetTargetTransform(E_PlayState.ACTOR, ((C_Actor)tp_state).transform);

        // ���� ����
        m_states[(int)currentState].Execute();

        // ó�� ����
        currentState = E_PlayState.AIRPLANE;
    }

    private void Update()
    {
        m_states[(int)currentState].StateUpdate();
    }

    private void FixedUpdate()
    {
        // ���� FixedUpdate
        earlyFixedUpdate?.Invoke();

        // ������
        m_states[(int)currentState].StateFixedUpdate();

        // ���� FixedUpdate
        lateFixedUpdate?.Invoke();
    }
}
