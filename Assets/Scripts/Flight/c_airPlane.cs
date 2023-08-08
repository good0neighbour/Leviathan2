using UnityEngine;

public class C_AirPlane : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField][Range(0.0f, 100.0f)] private float m_power = 9.8f;
    [SerializeField][Range(0.0f, 1.0f)] private float m_stealth = 0.0f;
    [SerializeField] private MeshRenderer m_renderer = null;
    private C_AirPlaneStateBase[] m_state = null;
    private Material m_material = null;
    private E_FlightStates m_currentState = E_FlightStates.eHover;
    private float m_currentPower = 0.0f;
    private byte m_stealthActive = 0;
    private float m_maxEnginePower = 0.0f;
    private float m_minEnginePower = 0.0f;



    /* ========== Public Methods ========== */

    /// <summary>
    /// ���� ����
    /// </summary>
    public void SetState(E_FlightStates t_state)
    {
        // ���� ����
        m_currentState = t_state;
    }


    /// <summary>
    /// ���� Ŭ���� ��ȯ
    /// </summary>
    public C_AirPlaneStateBase GetState(E_FlightStates t_state)
    {
        return m_state[(int)t_state];
    }



    /* ========== Private Methods ========== */

    private void Awake()
    {
        // �װ��� ���� ��������
        C_AirplaneSettings t_settings = Resources.Load<C_AirplaneSettings>("AirplaneSettings");
        m_maxEnginePower = t_settings.m_maxEnginePower;
        m_minEnginePower = t_settings.m_minEnginePower;

        // ���� Ŭ���� ����
        m_state = new C_AirPlaneStateBase[(int)E_FlightStates.end];
        m_state[(int)E_FlightStates.eHover] = new C_StateHover(this, t_settings);
        m_state[(int)E_FlightStates.eFlight] = new C_StateFlight(this, t_settings);

        // �װ��� ��Ÿ���� ����
        m_material = new Material(m_renderer.material);
        m_renderer.material = m_material;
    }


    private void Update()
    {
        // ���� ��� ����
        if (Input.GetKey(KeyCode.LeftShift) && m_maxEnginePower > m_power)
        {
            m_power += Time.deltaTime * 5.0f;
        }
        else if (Input.GetKey(KeyCode.LeftControl) && m_minEnginePower < m_power)
        {
            m_power -= Time.deltaTime * 5.0f;
        }

        // ���� (�ӽ�)
        if (Input.GetKeyDown(KeyCode.C) && 0 == (m_stealthActive & 0b10))
        {
            if (1 <= (1 & m_stealthActive))
            {
                m_stealthActive -= 0b01;
            }
            else
            {
                m_stealthActive += 0b01;
            }

            m_stealthActive += 0b10;
        }

        if (1 <= (m_stealthActive & 0b10))
        {
            if (1 <= (1 & m_stealthActive))
            {
                m_stealth += Time.deltaTime;
                if (1.0f < m_stealth)
                {
                    m_stealth = 1.0f;
                    m_stealthActive -= 0b10;
                }
            }
            else
            {
                m_stealth -= Time.deltaTime;
                if (0.0f > m_stealth)
                {
                    m_stealth = 0.0f;
                    m_stealthActive -= 0b10;
                }
            }

            m_material.SetFloat("_DissolveAmount", m_stealth);
        }

        // ������
        m_state[(int)m_currentState].Update();
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
