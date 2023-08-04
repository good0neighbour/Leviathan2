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
        // �߷� ���ӵ�
        Vector3 t_acellation = new Vector3(0, -9.8f, 0);

        t_acellation += new Vector3(0, power, 0);

        // �ӷ� ����
        m_velocity += t_acellation * Time.deltaTime;

        // ��ġ �̵�
        m_transform.position += m_velocity * Time.deltaTime;
    }

    public override void update()
    {
        throw new System.NotImplementedException();
    }
}
