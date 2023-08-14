using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class C_StateFlight : C_AirPlaneStateBase
{
    /* ========== Fields ========== */

    private Vector3 m_rotatePower = Vector3.zero;
    private float m_rotationUpDown = 0.0f;
    private float m_rotationLeftRight = 0.0f;
    private float m_rotationY = 0.0f;
    private float m_rotateSpeedmult = 0.0f;
    private float m_flightPowerMultiply = 0.0f;
    private float m_flightFalldownForce = 0.0f;
    private float m_liftPower = 0.0f;



    /* ========== Public Methods ========== */

    public C_StateFlight(C_AirPlane t_machine, C_AirplaneSettings t_settings, Animator t_animator) : base(t_machine, t_settings)
    {
        m_rotateSpeedmult = t_settings.m_flightRotateSpeedmult;
        m_rotatePower = t_settings.m_flightRotatePower;
        m_flightPowerMultiply = t_settings.m_flightPowerMultiply;
        m_flightFalldownForce = t_settings.m_flightFalldownForce;
        m_liftPower = t_settings.m_liftPower;
        m_animator = t_animator;
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


    public override void StateFixedUpdate()
    {
        // �������� ����
        Vector3 t_acceleration = new Vector3(0.0f, 0.0f, power * m_flightPowerMultiply);

        // ��Į ��ǥ�� ���� ���� ���� �ӷ�
        float t_velocityZ;

        // ��ġ �̵�
        m_transform.localPosition += SetVelocity(t_acceleration, out t_velocityZ) * Time.fixedDeltaTime;

        // Z�� ȸ��
        float t_rotationZ = m_transform.localRotation.eulerAngles.z * Mathf.Deg2Rad;

        // ��ü ȸ��
        m_transform.localRotation *= Quaternion.Euler(
            // ��ü ���ۿ� ���� ȸ��
            new Vector3(
                m_rotationUpDown * m_rotatePower.x,
                m_rotationY * m_rotatePower.y * 0.5f,
                m_rotationLeftRight * m_rotatePower.z
            ) * Time.fixedDeltaTime * t_velocityZ
            // ��¿� ���� ȸ��
            + new Vector3(
                (m_flightFalldownForce * Mathf.Cos(t_rotationZ) - (m_flightFalldownForce - 1.0f / ((t_velocityZ + 1.0f) * m_liftPower))) * Time.fixedDeltaTime,
                -m_flightFalldownForce * Mathf.Sin(t_rotationZ) * Time.fixedDeltaTime,
                0.0f
            )
        );
    }


    public override void StateUpdate()
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
            m_animator.SetBool("FlightMode", false);
            ChangeState();
        }

        HUDUpdate();
    }
}
