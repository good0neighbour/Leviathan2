using UnityEngine;
using TMPro;

public class C_GuidedMissle : MonoBehaviour, I_State<E_PlayState>
{
    /* ========== Fields ========== */
    
    [SerializeField] private Transform mp_attachedTarget = null;
    [SerializeField] private Vector3 m_attachingOffset = Vector3.zero;
    [Header("HUD 참조")]
    [SerializeField] private GameObject mp_HUDCanvas = null;
    [SerializeField] private TextMeshProUGUI mp_altitudeText = null;
    private Quaternion m_currentRotationX = Quaternion.Euler(45.0f, 0.0f, 0.0f);
    private Quaternion m_currentRotationY = Quaternion.identity;



    /* ========== Public Methods ========== */

    public void Execute()
    {
        mp_HUDCanvas.SetActive(true);
    }


    public void ChangeState(E_PlayState t_state)
    {
        mp_HUDCanvas.SetActive(false);
        C_PlayManager.instance.SetState(t_state);
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
            ChangeState(E_PlayState.AIRPLANE);
        }
        #endregion
#endif

        // 고도 표시
        mp_altitudeText.text = Mathf.RoundToInt(transform.localPosition.y).ToString();
    }


    public void StateFixedUpdate()
    {
        transform.localPosition = mp_attachedTarget.localPosition + m_attachingOffset;
        transform.localRotation = mp_attachedTarget.localRotation * m_currentRotationY * m_currentRotationX;
    }
}
