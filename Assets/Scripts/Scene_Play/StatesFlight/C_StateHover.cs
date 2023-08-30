using UnityEngine;

public class C_StateHover : C_AirPlaneStateBase
{
    /* ========== Fields ========== */

    private float m_rotatePower = 0.0f;
    private float m_rotateRestorePower = 0.0f;
    private float m_altitudeLimitMult = 0.0f;
    private byte m_stateChangePhase = 0;



    /* ========== Public Methods ========== */

    public C_StateHover(C_AirPlane tp_machine, C_AirplaneSettings tp_settings, Animator tp_animator) : base(tp_machine, tp_settings)
    {
        m_rotatePower = tp_settings.m_hoverRotatePower;
        m_rotateRestorePower = tp_settings.m_hoverRotateRestorePower;
        m_altitudeLimitMult = 1.0f / tp_settings.m_altitudeLimit;
        mp_animator = tp_animator;
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
        mp_rotationY.value = 0.0f;
        mp_machine.SetState(E_FlightStates.HOVER);
    }


    public override void StateFixedUpdate()
    {
        switch (C_PlayManager.instance.currentState)
        {
            case E_PlayStates.GUIDEDMISSILE:
                MovePosition();
                break;

            case E_PlayStates.ACTOR:
                velocity = Vector3.zero;
                break;

            default:
                //기체 이동
                MovePosition();
                // 기체 회전
                mp_transform.localRotation *= Quaternion.Euler(new Vector3(
                    mp_joystick.value.y,
                    mp_rotationY.value,
                    -mp_joystick.value.x
                ) * Time.fixedDeltaTime * m_rotatePower);
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
        // 모드 변경 애니메이션 끝나는 시점에 상태 변경
        switch (m_stateChangePhase)
        {
            case C_Constants.HOVER_UNAVAILABLE:
                if (1.0f <= mp_animator.GetCurrentAnimatorStateInfo(0).normalizedTime)
                {
                    m_stateChangePhase = C_Constants.HOVER_STANDBY;
                    ChangeState(E_FlightStates.FLIGHT);
                }
                break;

            case C_Constants.HOVER_ACTIVATE:
                mp_animator.SetBool("FlightMode", true);
                m_stateChangePhase = C_Constants.HOVER_UNAVAILABLE;
                break;

            default:
                break;
        }

        // 전방에 고정된 HUD
        HUDUpdate(false);

        base.StateUpdate();
    }


    public override void SwitchMode()
    {
        m_stateChangePhase = C_Constants.HOVER_ACTIVATE;
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
