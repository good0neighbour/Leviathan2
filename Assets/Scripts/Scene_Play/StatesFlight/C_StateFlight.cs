using UnityEngine;

public class C_StateFlight : C_AirPlaneStateBase
{
    /* ========== Fields ========== */

    private Vector3 m_rotatePower = Vector3.zero;
    private float m_flightPowerMultiply = 0.0f;
    private float m_flightFalldownForce = 0.0f;
    private float m_altitudeLimitMult = 0.0f;
    private float m_liftPower = 0.0f;



    /* ========== Public Methods ========== */

    public C_StateFlight(C_AirPlane tp_machine, C_AirplaneSettings tp_settings, Animator tp_animator) : base(tp_machine, tp_settings)
    {
        m_rotatePower = tp_settings.m_flightRotatePower;
        m_flightPowerMultiply = tp_settings.m_flightPowerMultiply;
        m_flightFalldownForce = tp_settings.m_flightFalldownForce;
        m_liftPower = tp_settings.m_liftPower;
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
        mp_machine.SetState(E_FlightStates.FLIGHT);
    }


    public override void StateFixedUpdate()
    {
        // �������� ����
        Vector3 t_acceleration = new Vector3(
            0.0f,
            0.0f,
            power * m_flightPowerMultiply * (1.0f - mp_transform.localPosition.y * m_altitudeLimitMult)
        );

        // ��Į ��ǥ�� ���� ���� ���� �ӷ�
        float t_velocityZ;

        // ��ġ �̵�
        SetVelocity(t_acceleration, out t_velocityZ);

        // Z�� ȸ��
        float t_rotationZ = mp_transform.localRotation.eulerAngles.z * Mathf.Deg2Rad;

        // ��ü ȸ��
        mp_transform.localRotation *= Quaternion.Euler(
            // ��ü ���ۿ� ���� ȸ��
            new Vector3(
                mp_joystick.value.y * m_rotatePower.x,
                mp_rotationY.value * m_rotatePower.y,
                -mp_joystick.value.x * m_rotatePower.z
            ) * Time.fixedDeltaTime * t_velocityZ
            // ��¿� ���� ȸ��
            + new Vector3(
                (m_flightFalldownForce * Mathf.Cos(t_rotationZ) - (m_flightFalldownForce - 1.0f / ((t_velocityZ + 1.0f) * m_liftPower))) * Time.fixedDeltaTime * 0.5f,
                -m_flightFalldownForce * Mathf.Sin(t_rotationZ) * Time.fixedDeltaTime,
                0.0f
            )
        );
    }


    public override void StateUpdate()
    {
        HUDUpdate(true);

        base.StateUpdate();
    }


    public override void SwitchMode()
    {
        mp_animator.SetBool("FlightMode", false);
        ChangeState(E_FlightStates.HOVER);
    }
}
