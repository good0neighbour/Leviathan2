using UnityEngine;

public class C_StateFlight : C_AirPlaneStateBase
{
    /* ========== Fields ========== */

    private float m_rotationUpDown = 0.0f;
    private float m_rotationLeftRight = 0.0f;
    private float m_rotationY = 0.0f;
    private float m_rotateSpeedmult = 0.0f;
    private Vector3 m_rotatePower = Vector3.zero;



    /* ========== Public Methods ========== */

    public C_StateFlight(C_AirPlane t_machine, C_AirplaneSettings t_settings)
    {
        m_machine = t_machine;
        m_transform = t_machine.transform;
        m_airResist = t_settings.m_airResist;
        m_liftPower = t_settings.m_liftPower;
        m_rotateSpeedmult = t_settings.m_flightRotateSpeedmult;
        m_rotatePower = t_settings.m_flightRotatePower;
    }


    public override void ChangeState()
    {
        C_AirPlaneStateBase t_state = m_machine.GetState(E_FlightStates.eHover);
        t_state.power = power;
        t_state.velocity = velocity;
        t_state.Execute();
    }


    public override void Execute()
    {
        m_machine.SetState(E_FlightStates.eFlight);
    }


    public override void FixedUpdate()
    {
        // �������� ����
        Vector3 t_acceleration = new Vector3(0.0f, 0.0f, power);

        // ��Į ��ǥ�� ���� ���� ���� �ӷ�
        float t_velocityZ;

        // ��ġ �̵�
        m_transform.localPosition += SetVelocity(t_acceleration, out t_velocityZ) * Time.fixedDeltaTime;

        // ��ü ȸ��
        m_transform.localRotation *= Quaternion.Euler(
            new Vector3(
                m_rotationUpDown * m_rotatePower.x,
                m_rotationY * m_rotatePower.y,
                m_rotationLeftRight * m_rotatePower.z
            ) * Time.fixedDeltaTime * t_velocityZ
        );
    }


    public override void Update()
    {
        #region ����
        // ��, �Ʒ� ����
        if (Input.GetKey(KeyCode.S))
        {
            if (-1.0f > m_rotationUpDown)
            {
                m_rotationUpDown = -1.0f;
            }
            else
            {
                m_rotationUpDown -= Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else if (Input.GetKey(KeyCode.W))
        {
            if (1.0f < m_rotationUpDown)
            {
                m_rotationUpDown = 1.0f;
            }
            else
            {
                m_rotationUpDown += Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else
        {
            // ��, �� ���� �ӵ� ����
            if (0.0f < m_rotationUpDown)
            {
                m_rotationUpDown -= Time.deltaTime * m_rotateSpeedmult;
            }
            else if (0.0f > m_rotationUpDown)
            {
                m_rotationUpDown += Time.deltaTime * m_rotateSpeedmult;
            }
        }

        // ��, �� ����
        if (Input.GetKey(KeyCode.A))
        {
            if (1.0f < m_rotationLeftRight)
            {
                m_rotationLeftRight = 1.0f;
            }
            else
            {
                m_rotationLeftRight += Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (-1.0f > m_rotationLeftRight)
            {
                m_rotationLeftRight = -1.0f;
            }
            else
            {
                m_rotationLeftRight -= Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else
        {
            // ��, �� ���� �ӵ� ����
            if (0.0f < m_rotationLeftRight)
            {
                m_rotationLeftRight -= Time.deltaTime * m_rotateSpeedmult;
            }
            else if (0.0f > m_rotationLeftRight)
            {
                m_rotationLeftRight += Time.deltaTime * m_rotateSpeedmult;
            }
        }

        // ��ü ȸ�� �ӵ�
        if (Input.GetKey(KeyCode.E))
        {
            if (1.0f < m_rotationY)
            {
                m_rotationY = 1.0f;
            }
            else
            {
                m_rotationY += Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            if (-1.0f > m_rotationY)
            {
                m_rotationY = -1.0f;
            }
            else
            {
                m_rotationY -= Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else
        {
            // ��ü ȸ�� �ӵ� ����
            if (0.0f < m_rotationY)
            {
                m_rotationY -= Time.deltaTime * m_rotateSpeedmult;
            }
            else if (0.0f > m_rotationY)
            {
                m_rotationY += Time.deltaTime * m_rotateSpeedmult;
            }
        }
        #endregion

        // ���� ���� ����
        if (Input.GetKeyDown(KeyCode.F))
        {
            ChangeState();
        }
    }
}
