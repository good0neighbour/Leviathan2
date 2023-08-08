using UnityEngine;

public class C_StateFlight : C_AirPlaneStateBase
{
    /* ========== Public Methods ========== */

    public C_StateFlight(Transform t_transform)
    {
        m_transform = t_transform;
        C_AirplaneSettings t_settings = Resources.Load<C_AirplaneSettings>("AirplaneSettings");
        m_airResist = t_settings.m_airResist;
        m_liftPower = t_settings.m_liftPower;
    }


    public override void ChangeState()
    {
        
    }


    public override void Execute()
    {
        
    }


    public override void FixedUpdate()
    {
        // �������� ����
        Vector3 t_accelation = new Vector3(0.0f, 0.0f, power);

        // �������װ� ���
        t_accelation += AirResistAndLiftPower();

        // �߷°��ӵ� �� ���� ����
        t_accelation += m_transform.localRotation * t_accelation + new Vector3(0, -9.8f, 0);

        // �ӷ� ����
        m_velocity += t_accelation * Time.deltaTime;

        // ��ġ �̵�
        m_transform.localPosition += m_velocity * Time.deltaTime;
    }


    public override void Update()
    {
        
    }
}
