using UnityEngine;

public class C_CameraMove : MonoBehaviour, I_StateMachine<E_PlayState>
{
    /* ========== Fields ========== */

    [SerializeField] private Vector3 m_airPlaneCameraPosition = Vector3.zero;
    [SerializeField] private Vector3 m_actorCameraPosition = Vector3.zero;
    [SerializeField] float m_lerpWeight = 0.1f;
    private Transform[] mp_targets = new Transform[(int)E_PlayState.END];
    private E_PlayState m_currentState = E_PlayState.AIRPLANE;

    public static C_CameraMove instance
    {
        get;
        set;
    }



    /* ========== Public Methods ========== */

    public void SetState(E_PlayState t_state)
    {
        // 상태 변경
        m_currentState = t_state;

        // 대상
        Transform tp_target = mp_targets[(int)m_currentState];

        // 대상 전달
        C_MinimapCameraMove.instance.target = tp_target;

        // 카메라 즉시 이동
        switch (m_currentState)
        {
            case E_PlayState.AIRPLANE:
                transform.localPosition = tp_target.localPosition + tp_target.localRotation * m_airPlaneCameraPosition;
                transform.localRotation = tp_target.localRotation;
                return;

            case E_PlayState.ACTOR:
                transform.localRotation = Quaternion.Euler(
                    0.0f,
                    transform.localRotation.eulerAngles.y,
                    0.0f
                );
                return;
        }
    }


    /// <summary>
    /// 카메라 대상 전달
    /// </summary>
    public void SetTargetTransform(E_PlayState t_index, Transform tp_target)
    {
        mp_targets[(int)t_index] = tp_target;
    }



    /* ========== Private Methods ========== */

    private void LateFixedUpdate()
    {
        Transform tp_target = mp_targets[(int)m_currentState];

        switch (m_currentState)
        {
            case E_PlayState.AIRPLANE:
                transform.localPosition = Vector3.Lerp(
                    tp_target.localPosition + tp_target.localRotation * m_airPlaneCameraPosition,
                    transform.localPosition,
                    m_lerpWeight
                );
                transform.localRotation = Quaternion.Lerp(
                    tp_target.localRotation,
                    transform.localRotation,
                    m_lerpWeight
                );
                return;

            case E_PlayState.GUIDEDMISSLE:
                transform.localPosition = tp_target.localPosition;
                transform.localRotation = tp_target.localRotation;
                return;

            case E_PlayState.ACTOR:
                transform.localPosition = tp_target.localPosition + transform.localRotation * m_actorCameraPosition;
                return;

            default:
#if UNITY_EDITOR
                Debug.LogError("잘못된 카메라 상태");
#endif
                return;
        }
    }


    private void Awake()
    {
        // 유니티식 싱글턴패턴
        instance = this;
    }


    private void Start()
    {
        // 대리자 등록
        C_PlayManager.instance.lateFixedUpdate += LateFixedUpdate;
    }
}
