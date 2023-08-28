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
    private float m_altitudeLimitMult = 0.0f;
    private bool m_stateChangeAvailable = true;



    /* ========== Public Methods ========== */

    public C_StateHover(C_AirPlane tp_machine, C_AirplaneSettings tp_settings, Animator tp_animator) : base(tp_machine, tp_settings)
    {
        m_rotateSpeedmult = tp_settings.m_hoverRotateSpeedmult;
        m_rotatePower = tp_settings.m_hoverRotatePower;
        m_rotateRestorePower = tp_settings.m_hoverRotateRestorePower;
        m_altitudeLimitMult = 1.0f / tp_settings.m_altitudeLimit;
        mp_animator = tp_animator;
        m_reverseRotateSpeedmult = m_rotateSpeedmult * 0.5f;
    }


    public override void ChangeState(E_FlightStates t_state)
    {
        C_AirPlaneStateBase tp_state = mp_machine.GetState(t_state);
        tp_state.power = power;
        tp_state.velocity = velocity;
        tp_state.Execute();
    }


    public override void Execute()
    {
        mp_machine.SetState(E_FlightStates.HOVER);
    }


    public override void StateFixedUpdate()
    {
        switch (C_PlayManager.instance.currentState)
        {
            case E_PlayStates.GUIDEDMISSLE:
                // GuidedMissle 상태에서는 회전하지 않는다.
                m_rotationFront = 0.0f;
                m_rotationSide = 0.0f;
                MovePosition();
                break;

            case E_PlayStates.ACTOR:
                // Actor 상태에서는 회전, 이동하지 않는다.
                m_rotationFront = 0.0f;
                m_rotationSide = 0.0f;
                velocity = Vector3.zero;
                break;

            default:
                //기체 이동
                MovePosition();
                // 기체 회전
                mp_transform.localRotation *= Quaternion.Euler(new Vector3(m_rotationFront, m_rotationY, m_rotationSide) * Time.fixedDeltaTime * m_rotatePower);
                break;
        }
        
        // 기체 회전 회복
        float t_rotationX = mp_transform.localRotation.eulerAngles.x;
        float t_rotationZ = mp_transform.localRotation.eulerAngles.z;
        if (180.0f < t_rotationX)
        {
            t_rotationX -= 360.0f;
        }
        if (180.0f < t_rotationZ)
        {
            t_rotationZ -= 360.0f;
        }
        mp_transform.localRotation *= Quaternion.Euler(
            (-t_rotationX) * Time.fixedDeltaTime * m_rotateRestorePower,
            0.0f,
            (-t_rotationZ) * Time.fixedDeltaTime * m_rotateRestorePower
        );
    }


    public override void StateUpdate()
    {

        if (!m_stateChangeAvailable && 1.0f <= mp_animator.GetCurrentAnimatorStateInfo(0).normalizedTime)
        {
            m_stateChangeAvailable = true;
            ChangeState(E_FlightStates.FLIGHT);
        }

        HUDUpdate(false);

#if PLATFORM_STANDALONE_WIN

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
        #region 상태 변경
        // 비행 상태 변경
        if (Input.GetKeyDown(KeyCode.F) && m_stateChangeAvailable)
        {
            mp_animator.SetBool("FlightMode", true);
            m_stateChangeAvailable = false;
        }
        #endregion
#endif
    }



    /* ========== Private Methods ========== */

    /// <summary>
    /// 기체 이동
    /// </summary>
    private void MovePosition()
    {
        // 기체의 위 방향으로 가속
        Vector3 t_acceleration = new Vector3(
            0.0f,
            power * (1.0f - mp_transform.localPosition.y * m_altitudeLimitMult),
            0.0f
        );
        // 위치 이동
        SetVelocity(t_acceleration);
    }
}
