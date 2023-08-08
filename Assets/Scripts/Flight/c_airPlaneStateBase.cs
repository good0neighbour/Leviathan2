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
    /// 속력 결정
    /// </summary>
    protected Vector3 SetVelocity(Vector3 t_acceleration)
    {
        // 양력
        t_acceleration += new Vector3(0.0f, -velocity.y * Mathf.Abs(velocity.z) * m_liftPower, 0.0f);

        // 로칼 좌표계 기준 속력
        Vector3 t_velocity = Quaternion.Inverse(m_transform.localRotation) * velocity
        
            // 가속
            + t_acceleration * Time.fixedDeltaTime;

        // 기체 방향
        velocity = m_transform.localRotation

            // 공기저항
            * new Vector3(
                (float)(t_velocity.x - m_airResist.x * Mathf.Abs(t_velocity.x) * t_velocity.x),
                (float)(t_velocity.y - m_airResist.y * Mathf.Abs(t_velocity.y) * t_velocity.y),
                (float)(t_velocity.z - m_airResist.z * Mathf.Abs(t_velocity.z) * t_velocity.z)
            )

            // 중력가속도
            + new Vector3(0.0f, -9.8f, 0.0f) * Time.fixedDeltaTime;

        // 반환
        return velocity;
    }


    /// <summary>
    /// 속력 결정 및 로칼 좌표계 기준 속력
    /// </summary>
    protected Vector3 SetVelocity(Vector3 t_acceleration, out float t_localVelocityZ)
    {
        // 양력
        t_acceleration += new Vector3(0.0f, -velocity.y * Mathf.Abs(velocity.z) * m_liftPower, 0.0f);

        // 로칼 좌표계 기준 속력
        Vector3 t_velocity = Quaternion.Inverse(m_transform.localRotation) * velocity

            // 가속
            + t_acceleration * Time.fixedDeltaTime;

        // 로칼 좌표계 기준 전방 방향 속력
        t_localVelocityZ = t_velocity.z;

        // 기체 방향
        velocity = m_transform.localRotation

            // 공기저항
            * new Vector3(
                (float)(t_velocity.x - m_airResist.x * Mathf.Abs(t_velocity.x) * t_velocity.x),
                (float)(t_velocity.y - m_airResist.y * Mathf.Abs(t_velocity.y) * t_velocity.y),
                (float)(t_velocity.z - m_airResist.z * Mathf.Abs(t_velocity.z) * t_velocity.z)
            )

            // 중력가속도
            + new Vector3(0.0f, -9.8f, 0.0f) * Time.fixedDeltaTime;

        // 반환
        return velocity;
    }
}
