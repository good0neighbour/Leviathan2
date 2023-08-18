using UnityEngine;

public class C_CameraMove : MonoBehaviour, I_StateMachine<E_PlayState>
{
    /* ========== Fields ========== */

    [SerializeField] private Vector3 m_cameraPosition = Vector3.zero;
    [SerializeField] float m_lerpWeight = 0.1f;
    private Transform[] mp_targets = new Transform[(int)E_PlayState.END];
    private E_PlayState m_currentState = E_PlayState.ACTOR;

    public static C_CameraMove instance
    {
        get;
        set;
    }



    /* ========== Public Methods ========== */

    public void SetState(E_PlayState t_state)
    {
        m_currentState = t_state;
    }


    /// <summary>
    /// ī�޶� ��� ����
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
                    tp_target.localPosition + tp_target.localRotation * m_cameraPosition,
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
                transform.localPosition = tp_target.localPosition + transform.localRotation * new Vector3(0.0f, 1.0f, -5.0f);
                return;

            default:
#if UNITY_EDITOR
                Debug.Log("�߸��� ī�޶� ����");
#endif
                return;
        }
    }


    private void Awake()
    {
        // ����Ƽ�� �̱�������
        instance = this;
    }


    private void Start()
    {
        // �븮�� ���
        C_PlayManager.instance.lateFixedUpdate += LateFixedUpdate;
    }
}
