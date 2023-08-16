using UnityEngine;

public class C_CameraMove : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private Vector3 m_cameraPosition = Vector3.zero;
    [SerializeField] float m_lerpWeight = 0.1f;
    private Transform[] mp_targets = new Transform[(int)E_PlayState.END];
    private E_PlayState m_currentState = E_PlayState.AIRPLANE;

    public static C_CameraMove instance
    {
        get;
        set;
    }



    /* ========== Public Methods ========== */

    public void ChangeState(E_PlayState t_state)
    {
        m_currentState = t_state;
    }


    /// <summary>
    /// 카메라 대상 전달
    /// </summary>
    public void SetTargetTransform(E_PlayState t_index, Transform tp_target)
    {
        mp_targets[(int)t_index] = tp_target;
    }



    /* ========== Private Methods ========== */

    private void Awake()
    {
        // 유니티식 싱글턴패턴
        instance = this;
    }


    private void FixedUpdate()
    {
        Transform tp_target = mp_targets[(int)m_currentState];

        switch (m_currentState)
        {
            case E_PlayState.AIRPLANE:
                transform.localPosition = Vector3.Lerp(
                    tp_target.localPosition + tp_target.localRotation * m_cameraPosition,
                    transform.localPosition,
                    m_lerpWeight
                );
                transform.localRotation = Quaternion.Lerp(
                    tp_target.localRotation,
                    transform.localRotation,
                    m_lerpWeight
                );
                break;

            case E_PlayState.GUIDEDMISSLE:
                transform.localPosition = tp_target.localPosition;
                transform.localRotation = tp_target.localRotation;
                break;

            default:
#if UNITY_EDITOR
                Debug.Log("잘못된 카메라 상태");
#endif
                break;
        }
    }
}
