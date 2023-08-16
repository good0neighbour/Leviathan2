using UnityEngine;

public class C_GuidedMissle : MonoBehaviour, I_StateBase
{
    /* ========== Fields ========== */

    [SerializeField] private Transform m_attachedTarget = null;
    [SerializeField] private Vector3 m_attachingOffset = Vector3.zero;
    private Quaternion m_currentRotationX = Quaternion.Euler(45.0f, 0.0f, 0.0f);
    private Quaternion m_currentRotationY = Quaternion.identity;



    /* ========== Public Methods ========== */

    public void ChangeState()
    {
        
    }

    public void Execute()
    {
        
    }

    public void StateFixedUpdate()
    {
        transform.localPosition = m_attachedTarget.localPosition + m_attachingOffset;

        transform.localRotation = m_attachedTarget.localRotation * m_currentRotationY * m_currentRotationX;
    }

    public void StateUpdate()
    {
#if PLATFORM_STANDALONE_WIN

        #region 조작
        if (Input.GetKey(KeyCode.W))
        {
            if (0.0f < m_currentRotationX.eulerAngles.x && 270.0f > m_currentRotationX.eulerAngles.x)
            {
                m_currentRotationX *= Quaternion.Euler(-30.0f * Time.deltaTime, 0.0f, 0.0f);
            }
            else
            {
                m_currentRotationX = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (90.0f > m_currentRotationX.eulerAngles.x && 180.0f > m_currentRotationX.eulerAngles.z)
            {
                m_currentRotationX *= Quaternion.Euler(30.0f * Time.deltaTime, 0.0f, 0.0f);
            }
            else
            {
                m_currentRotationX = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            m_currentRotationY *= Quaternion.Euler(0.0f, -30.0f * Time.deltaTime, 0.0f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            m_currentRotationY *= Quaternion.Euler(0.0f, 30.0f * Time.deltaTime, 0.0f);
        }
        #endregion
        #region 상태 변경
        // 비행기로 상태 변경
        if (Input.GetKeyDown(KeyCode.V))
        {
            C_PlayManager.instance.ChangeState(E_PlayState.AIRPLANE);
        }
        #endregion
#endif
    }



    /* ========== Private Methods ========== */
}
