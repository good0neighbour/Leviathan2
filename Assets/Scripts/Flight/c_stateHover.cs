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

    public C_StateHover(C_AirPlane t_machine, C_AirplaneSettings t_settings) : base(t_machine, t_settings)
    {
        m_rotateSpeedmult = t_settings.m_hoverRotateSpeedmult;
        m_rotatePower = t_settings.m_hoverRotatePower;
        m_rotateRestorePower = t_settings.m_hoverRotateRestorePower;
        m_reverseRotateSpeedmult = m_rotateSpeedmult * 0.5f;
    }


    public override void ChangeState()
    {
        C_AirPlaneStateBase t_state = m_machine.GetState(E_FlightStates.eFlight);
        t_state.power = power;
        t_state.velocity = velocity;
        t_state.Execute();
    }


    public override void Execute()
    {
        m_machine.SetState(E_FlightStates.eHover);
    }


    public override void FixedUpdate()
    {
        // 기체의 위 방향으로 가속
        Vector3 t_acceleration = new Vector3(0.0f, power, 0.0f);

        // 위치 이동
        m_transform.localPosition += SetVelocity(t_acceleration) * Time.fixedDeltaTime;

        // 기체 회전
        m_transform.localRotation *= Quaternion.Euler(new Vector3(m_rotationFront, m_rotationY, m_rotationSide) * Time.fixedDeltaTime * m_rotatePower);

        // 기체 회전 회복
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
            (-t_rotationX) * Time.fixedDeltaTime * m_rotateRestorePower,
            0.0f,
            (-t_rotationZ) * Time.fixedDeltaTime * m_rotateRestorePower
        );
    }


    public override void Update()
    {
        #region 조작
        // 전, 후 기울기
        if (Input.GetKey(KeyCode.W))
        {
            if (1.0f < m_rotationFront)
            {
                m_rotationFront = 1.0f;
            }
            else
            {
                m_rotationFront += Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (-1.0f > m_rotationFront)
            {
                m_rotationFront = -1.0f;
            }
            else
            {
                m_rotationFront -= Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else
        {
            // 전, 후 기울기 속도 감소
            if (0.0f < m_rotationFront)
            {
                m_rotationFront -= Time.deltaTime * m_reverseRotateSpeedmult;
            }
            else if (0.0f > m_rotationFront)
            {
                m_rotationFront += Time.deltaTime * m_reverseRotateSpeedmult;
            }
        }

        // 좌, 우 기울기
        if (Input.GetKey(KeyCode.A))
        {
            if (1 < m_rotationSide)
            {
                m_rotationSide = 1.0f;
            }
            else
            {
                m_rotationSide += Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (-1.0f > m_rotationSide)
            {
                m_rotationSide = -1.0f;
            }
            else
            {
                m_rotationSide -= Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else
        {
            // 좌, 우 기울기 속도 감소
            if (0.0f < m_rotationSide)
            {
                m_rotationSide -= Time.deltaTime * m_reverseRotateSpeedmult;
            }
            else if (0.0f > m_rotationSide)
            {
                m_rotationSide += Time.deltaTime * m_reverseRotateSpeedmult;
            }
        }

        // 기체 회전 속도
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
            // 기체 회전 속도 감소
            if (0.0f < m_rotationY)
            {
                m_rotationY -= Time.deltaTime * m_reverseRotateSpeedmult;
            }
            else if (0.0f > m_rotationY)
            {
                m_rotationY += Time.deltaTime * m_reverseRotateSpeedmult;
            }
        }
        #endregion

        // 비행 상태 변경
        if (Input.GetKeyDown(KeyCode.F))
        {
            ChangeState();
        }

        HUDUpdate();
    }
}
