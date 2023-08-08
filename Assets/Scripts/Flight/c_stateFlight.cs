using UnityEngine;

public class C_StateFlight : C_AirPlaneStateBase
{
    /* ========== Public Methods ========== */

    public C_StateFlight(Transform t_transform, C_AirplaneSettings t_settings)
    {
        m_transform = t_transform;
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
        if (Input.GetKey(KeyCode.S))
        {
            m_transform.localRotation *= Quaternion.Euler(-50.0f * Time.deltaTime, 0.0f, 0.0f);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            m_transform.localRotation *= Quaternion.Euler(50.0f * Time.deltaTime, 0.0f, 0.0f);
        }
        if (Input.GetKey(KeyCode.A))
        {
            m_transform.localRotation *= Quaternion.Euler(0.0f, 0.0f, 50.0f * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            m_transform.localRotation *= Quaternion.Euler(0.0f, 0.0f, -50.0f * Time.deltaTime);
        }
    }
}
