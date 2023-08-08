using UnityEngine;

public abstract class C_AirPlaneStateBase : C_StateBase
{
    /* ========== Fields ========== */

    protected Transform m_transform = null;
    protected Vector3 m_velocity = Vector3.zero;
    protected float m_airResist = 0.0f;
    protected float m_liftPower = 0.0f;



    /* ========== Properties ========== */

    public float power
    {
        protected get; set;
    }



    /* ========== Protected Methods ========== */

    /// <summary>
    /// ���� ����, ��� ���
    /// </summary>
    protected Vector3 AirResistAndLiftPower()
    {
        // ��Į ��ǥ�� ���� �ӷ�
        Vector3 t_velocity = Quaternion.Inverse(m_transform.localRotation) * m_velocity;

        // ���
        return new Vector3(0.0f, -m_velocity.y * Mathf.Abs(m_velocity.z) * m_liftPower, 0.0f)

            // �������� ����
            - new Vector3(Mathf.Abs(t_velocity.x) * t_velocity.x, Mathf.Abs(t_velocity.y) * t_velocity.y, Mathf.Abs(t_velocity.z) * t_velocity.z) * m_airResist;
    }
}
