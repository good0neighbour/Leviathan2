using UnityEngine;

public delegate void D_PlayDelegate();

public class C_PlayManager : MonoBehaviour, I_StateMachine<E_PlayState>
{
    /* ========== Fields ========== */

    private I_State<E_PlayState>[] m_states = new I_State<E_PlayState>[(int)E_PlayState.END];
    private E_PlayState m_currentState = E_PlayState.AIRPLANE;

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
        // ���� ����
        m_currentState = t_state;
        C_CameraMove.instance.SetState(m_currentState);

        // ���� ����
        m_states[(int)m_currentState].Execute();
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
        C_AirPlane tp_airPlane = FindAnyObjectByType<C_AirPlane>();
        m_states[(int)E_PlayState.AIRPLANE] = tp_airPlane;
        C_CameraMove.instance.SetTargetTransform(E_PlayState.AIRPLANE, tp_airPlane.transform);

        C_GuidedMissle tp_guidedMissle = FindAnyObjectByType<C_GuidedMissle>();
        m_states[(int)E_PlayState.GUIDEDMISSLE] = tp_guidedMissle;
        C_CameraMove.instance.SetTargetTransform(E_PlayState.GUIDEDMISSLE, tp_guidedMissle.transform);

        // ���� ����
        m_states[(int)m_currentState].Execute();
    }

    private void Update()
    {
        m_states[(int)m_currentState].StateUpdate();
    }

    private void FixedUpdate()
    {
        // ���� FixedUpdate
        earlyFixedUpdate?.Invoke();

        // ������
        m_states[(int)m_currentState].StateFixedUpdate();

        // ���� FixedUpdate
        lateFixedUpdate?.Invoke();
    }
}
