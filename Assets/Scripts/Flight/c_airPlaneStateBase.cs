using UnityEngine;

public abstract class C_AirPlaneStateBase : C_StateBase
{
    /* ========== Fields ========== */

    protected C_AirPlane m_machine = null;
    protected Transform m_transform = null;
    protected Vector3 m_airResist = Vector3.zero;
    protected float m_liftPower = 0.0f;



    /* ========== Properties ========== */

    public float power
    {
        protected get; set;
    }


    public Vector3 velocity
    {
        protected get; set;
    }



    /* ========== Protected Methods ========== */

    /// <summary>
    /// �ӷ� ����
    /// </summary>
    protected Vector3 SetVelocity(Vector3 t_acceleration)
    {
        // ���
        t_acceleration += new Vector3(0.0f, -velocity.y * Mathf.Abs(velocity.z) * m_liftPower, 0.0f);

        // ��Į ��ǥ�� ���� �ӷ�
        Vector3 t_velocity = Quaternion.Inverse(m_transform.localRotation) * velocity
        
            // ����
            + t_acceleration * Time.fixedDeltaTime;

        // ��ü ����
        velocity = m_transform.localRotation

            // ��������
            * new Vector3(
                (float)(t_velocity.x - m_airResist.x * Mathf.Abs(t_velocity.x) * t_velocity.x),
                (float)(t_velocity.y - m_airResist.y * Mathf.Abs(t_velocity.y) * t_velocity.y),
                (float)(t_velocity.z - m_airResist.z * Mathf.Abs(t_velocity.z) * t_velocity.z)
            )

            // �߷°��ӵ�
            + new Vector3(0.0f, -9.8f, 0.0f) * Time.fixedDeltaTime;

        // ��ȯ
        return velocity;
    }


    /// <summary>
    /// �ӷ� ���� �� ��Į ��ǥ�� ���� �ӷ�
    /// </summary>
    protected Vector3 SetVelocity(Vector3 t_acceleration, out float t_localVelocityZ)
    {
        // ���
        t_acceleration += new Vector3(0.0f, -velocity.y * Mathf.Abs(velocity.z) * m_liftPower, 0.0f);

        // ��Į ��ǥ�� ���� �ӷ�
        Vector3 t_velocity = Quaternion.Inverse(m_transform.localRotation) * velocity

            // ����
            + t_acceleration * Time.fixedDeltaTime;

        // ��Į ��ǥ�� ���� ���� ���� �ӷ�
        t_localVelocityZ = t_velocity.z;

        // ��ü ����
        velocity = m_transform.localRotation

            // ��������
            * new Vector3(
                (float)(t_velocity.x - m_airResist.x * Mathf.Abs(t_velocity.x) * t_velocity.x),
                (float)(t_velocity.y - m_airResist.y * Mathf.Abs(t_velocity.y) * t_velocity.y),
                (float)(t_velocity.z - m_airResist.z * Mathf.Abs(t_velocity.z) * t_velocity.z)
            )

            // �߷°��ӵ�
            + new Vector3(0.0f, -9.8f, 0.0f) * Time.fixedDeltaTime;

        // ��ȯ
        return velocity;
    }
}
