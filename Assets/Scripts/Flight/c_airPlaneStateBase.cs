using UnityEngine;

public abstract class C_AirPlaneStateBase : C_StateBase
{
    /* ========== Fields ========== */

    protected Transform m_transform = null;
    protected Vector3 m_velocity = Vector3.zero;
    protected Vector3 m_airResist = Vector3.zero;
    protected float m_liftPower = 0.0f;



    /* ========== Properties ========== */

    public float power
    {
        protected get; set;
    }



    /* ========== Protected Methods ========== */

    /// <summary>
    /// 공기 저항, 양력 계산
    /// </summary>
    protected Vector3 AirResistAndLiftPower()
    {
        // 로칼 좌표계 기준 속력
        Vector3 t_velocity = Quaternion.Inverse(m_transform.localRotation) * m_velocity;

        Debug.Log(Mathf.Abs(t_velocity.y) * t_velocity.y * m_airResist.y);

        // 양력
        return new Vector3(0.0f, -m_velocity.y * Mathf.Abs(m_velocity.z) * m_liftPower, 0.0f)

            // 공기저항 뺄셈
             - new Vector3(
                 Mathf.Abs(t_velocity.x) * t_velocity.x * m_airResist.x,
                 Mathf.Abs(t_velocity.y) * t_velocity.y * m_airResist.y,
                 0.0f
               );
    }
}
