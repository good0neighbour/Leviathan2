using UnityEngine;

public class C_AirPlane : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField][Range(0.0f, 20.0f)] private float m_power = 9.8f;
    [SerializeField][Range(0.0f, 1.0f)] private float m_stealth = 0.0f;
    [SerializeField] private MeshRenderer m_renderer = null;
    private C_AirPlaneStateBase[] m_state = null;
    private Material m_material = null;
    private E_FlightStates m_currentState = E_FlightStates.eFlight;
    private float m_currentPower = 0.0f;



    /* ========== Public Methods ========== */

    private void Awake()
    {
        // ���� Ŭ���� ����
        m_state = new C_AirPlaneStateBase[(int)E_FlightStates.end];
        m_state[(int)E_FlightStates.eHover] = new C_StateHover(transform);
        m_state[(int)E_FlightStates.eFlight] = new C_StateFlight(transform);

        // �װ��� ��Ÿ���� ����
        m_material = new Material(m_renderer.material);
        m_renderer.material = m_material;
    }


    private void Update()
    {
        // ���� ��� ����
        if (Input.GetKey(KeyCode.LeftShift))
        {
            m_power += Time.deltaTime * 5.0f;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            m_power -= Time.deltaTime * 5.0f;
        }

        // ������
        m_state[(int)m_currentState].Update();

        // ����
        m_material.SetFloat("_DissolveAmount", m_stealth);
    }


    private void FixedUpdate()
    {
        // ���� ��� ����
        if (m_currentPower != m_power)
        {
            m_currentPower = m_power;
            m_state[(int)m_currentState].power = m_power;
        }

        // ������
        m_state[(int)m_currentState].FixedUpdate();

        // ���� �ձ� ����
        if (0.0f > transform.localPosition.y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, 0.0f, transform.localPosition.z);
        }
    }
}
