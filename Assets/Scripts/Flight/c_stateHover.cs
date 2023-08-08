using UnityEngine;

public class C_StateHover : C_AirPlaneStateBase
{
    /* ========== Fields ========== */

    private float m_rotationFront = 0.0f;
    private float m_rotationSide = 0.0f;
    private float m_rotationY = 0.0f;
    private float m_rotateSpeedmult = 0.0f;
    private float m_rotatePower = 0.0f;
    private float m_reverseRotateSpeedmult = 0.0f;
    private float m_rotateRestorePower = 0.0f;



    /* ========== Public Methods ========== */

    public C_StateHover(Transform t_transform, C_AirplaneSettings t_settings)
    {
        m_transform = t_transform;
        m_rotateSpeedmult = t_settings.m_hoverRotateSpeedmult;
        m_rotatePower = t_settings.m_hoverRotatePower;
        m_rotateRestorePower = t_settings.m_hoverRotateRestorePower;
        m_airResist = t_settings.m_airResist;
        m_reverseRotateSpeedmult = m_rotateSpeedmult * 0.5f;
    }


    public override void ChangeState()
    {
        
    }


    public override void Execute()
    {
        
    }


    public override void FixedUpdate()
    {
        // ��ü�� �� �������� ����
        Vector3 t_acellation = new Vector3(0.0f, power, 0.0f);

        // �������װ� ���
        t_acellation += AirResistAndLiftPower();

        // �߷� ���ӵ� �� ���� ����
        t_acellation = m_transform.localRotation * t_acellation + new Vector3(0, -9.8f, 0);

        // �ӷ� ����
        m_velocity += t_acellation * Time.deltaTime;

        // ��ġ �̵�
        m_transform.localPosition += m_velocity * Time.deltaTime;

        // ��ü ȸ��
        m_transform.localRotation *= Quaternion.Euler(new Vector3(m_rotationFront, m_rotationY, m_rotationSide) * Time.deltaTime * m_rotatePower);

        // ��ü ȸ�� ȸ��
        float t_rotationX = m_transform.localRotation.eulerAngles.x;
        float t_rotationZ = m_transform.localRotation.eulerAngles.z;
        if (180.0f < t_rotationX)
        {
            t_rotationX -= 360.0f;
        }
        if (180.0f < t_rotationZ)
        {
            t_rotationZ -= 360.0f;
        }
        m_transform.localRotation *= Quaternion.Euler(
            (-t_rotationX) * Time.deltaTime * m_rotateRestorePower,
            0.0f,
            (-t_rotationZ) * Time.deltaTime * m_rotateRestorePower
        );
    }


    public override void Update()
    {
        // ��, �� ����
        if (Input.GetKey(KeyCode.W))
        {
            if (1 < m_rotationFront)
            {
                m_rotationFront = 1;
            }
            else
            {
                m_rotationFront += Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (-1 > m_rotationFront)
            {
                m_rotationFront = -1;
            }
            else
            {
                m_rotationFront -= Time.deltaTime * m_rotateSpeedmult;
            }
        }

        // ��, �� ���� �ӵ� ����
        if (0 < m_rotationFront)
        {
            m_rotationFront -= Time.deltaTime * m_reverseRotateSpeedmult;
        }
        else if (0 > m_rotationFront)
        {
            m_rotationFront += Time.deltaTime * m_reverseRotateSpeedmult;
        }

        // ��, �� ����
        if (Input.GetKey(KeyCode.A))
        {
            if (1 < m_rotationSide)
            {
                m_rotationSide = 1;
            }
            else
            {
                m_rotationSide += Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (-1 > m_rotationSide)
            {
                m_rotationSide = -1;
            }
            else
            {
                m_rotationSide -= Time.deltaTime * m_rotateSpeedmult;
            }
        }

        // ��, �� ���� �ӵ� ���
        if (0 < m_rotationSide)
        {
            m_rotationSide -= Time.deltaTime * m_reverseRotateSpeedmult;
        }
        else if (0 > m_rotationSide)
        {
            m_rotationSide += Time.deltaTime * m_reverseRotateSpeedmult;
        }

        // ��ü ȸ�� �ӵ�
        if (Input.GetKey(KeyCode.E))
        {
            if (1 < m_rotationY)
            {
                m_rotationY = 1;
            }
            else
            {
                m_rotationY += Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            if (-1 > m_rotationY)
            {
                m_rotationY = -1;
            }
            else
            {
                m_rotationY -= Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else
        {
            // ��ü ȸ�� �ӵ� ����
            if (0 < m_rotationY)
            {
                m_rotationY -= Time.deltaTime * m_reverseRotateSpeedmult;
            }
            else if (0 > m_rotationY)
            {
                m_rotationY += Time.deltaTime * m_reverseRotateSpeedmult;
            }
        }
    }
}
