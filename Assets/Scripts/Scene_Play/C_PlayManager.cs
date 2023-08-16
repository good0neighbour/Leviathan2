using UnityEngine;

public class C_PlayManager : MonoBehaviour
{
    /* ========== Fields ========== */

    private I_StateBase[] m_states = new I_StateBase[(int)E_PlayState.END];
    private E_PlayState m_currentState = E_PlayState.AIRPLANE;

    public static C_PlayManager instance
    {
        get;
        set;
    }



    /* ========== Public Methods ========== */

    public void ChangeState(E_PlayState t_state)
    {
        m_currentState = t_state;
        C_CameraMove.instance.ChangeState(m_currentState);
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
        C_AirPlane tp_airPlane = FindAnyObjectByType<C_AirPlane>();
        m_states[(int)E_PlayState.AIRPLANE] = tp_airPlane;
        C_CameraMove.instance.SetTargetTransform(E_PlayState.AIRPLANE, tp_airPlane.transform);

        C_GuidedMissle tp_guidedMissle = FindAnyObjectByType<C_GuidedMissle>();
        m_states[(int)E_PlayState.GUIDEDMISSLE] = tp_guidedMissle;
        C_CameraMove.instance.SetTargetTransform(E_PlayState.GUIDEDMISSLE, tp_guidedMissle.transform);
    }

    private void Update()
    {
        m_states[(int)m_currentState].StateUpdate();
    }

    private void FixedUpdate()
    {
        m_states[(int)m_currentState].StateFixedUpdate();
    }
}
