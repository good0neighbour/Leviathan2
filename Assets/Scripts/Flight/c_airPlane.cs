using UnityEngine;

public class c_airPlane : MonoBehaviour
{
    /* ========== Private Fields ========== */

    [SerializeField][Range(0.0f, 20.0f)] private float m_power = 9.8f;
    [SerializeField][Range(0.0f, 1.0f)] private float m_stealth = 0.0f;
    [SerializeField] private MeshRenderer m_renderer = null;
    private c_airPlaneStateBase[] m_state = null;
    private Material m_material = null;
    private e_flightStates m_currentState = e_flightStates.eHover;
    private float m_currentPower = 0.0f;


    /* ========== Public Methods ========== */

    private void Awake()
    {
        // 상태 클래스 생성
        m_state = new c_airPlaneStateBase[(int)e_flightStates.end];
        m_state[(int)e_flightStates.eHover] = new c_stateHover(transform);

        // 항공기 메타리얼 복사
        m_material = new Material(m_renderer.material);
        m_renderer.material = m_material;
    }

    private void Update()
    {
        m_material.SetFloat("_DissolveAmount", m_stealth);
    }

    private void FixedUpdate()
    {
        if (m_currentPower != m_power)
        {
            m_currentPower = m_power;
            m_state[(int)m_currentState].power = m_power;
        }
        m_state[(int)m_currentState].fixedUpdate();
    }
}
