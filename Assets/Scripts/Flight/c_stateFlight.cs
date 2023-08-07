using UnityEngine;

public class c_stateFlight : c_airPlaneStateBase
{
    /* ========== Public Methods ========== */

    public c_stateFlight(Transform t_transform)
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
        Vector3 t_accelation;

        switch (m_velocity.z)
        {
            case 0.0f:
                t_accelation = new Vector3(0.0f, -9.8f, 0.0f);
                break;
            default:
                t_accelation = new Vector3(0.0f, -Mathf.Abs(9.8f / m_velocity.z), 0.0f);
                break;
        }

        t_accelation += m_transform.localRotation * new Vector3(0.0f, 0.0f, power);

        m_velocity += t_accelation * Time.deltaTime;

        m_transform.localPosition += m_velocity * Time.deltaTime;
    }


    public override void update()
    {
        throw new System.NotImplementedException();
    }
}
