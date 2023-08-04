using UnityEngine;

public class c_stateHover : c_airPlaneStateBase
{
    /* ========== Public Methods ========== */

    public c_stateHover(Transform t_transform)
    {
        m_transform = t_transform;
    }

    public override void changeState()
    {
        throw new System.NotImplementedException();
    }

    public override void execute()
    {
        throw new System.NotImplementedException();
    }

    public override void fixedUpdate()
    {
        // 중력 가속도
        Vector3 t_acellation = new Vector3(0, -9.8f, 0);

        t_acellation += new Vector3(0, power, 0);

        // 속력 결정
        m_velocity += t_acellation * Time.deltaTime;

        // 위치 이동
        m_transform.position += m_velocity * Time.deltaTime;
    }

    public override void update()
    {
        throw new System.NotImplementedException();
    }
}
