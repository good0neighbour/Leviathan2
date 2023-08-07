using UnityEngine;

public class c_stateHover : c_airPlaneStateBase
{
    private float m_rotationFront = 0.0f;
    private float m_rotationSide = 0.0f;
    private float m_rotationY = 0.0f;
    private float m_rotateSpeedmult = 0.0f;
    private float m_rotateRestore = 0.0f;
    private float m_maxRotation = 0.0f;

    /* ========== Public Methods ========== */

    public c_stateHover(Transform t_transform)
    {
        m_transform = t_transform;
        c_airplaneSettings t_settings = Resources.Load<c_airplaneSettings>("AirplaneSettings");
        m_rotateSpeedmult = t_settings.m_hoverRotateSpeedmult;
        m_rotateRestore = t_settings.m_hoverRotateRestoreSpeedmult;
        m_maxRotation = t_settings.m_hiverMaxRotation;
    }


    public override void changeState()
    {
        
    }


    public override void execute()
    {
        
    }


    public override void fixedUpdate()
    {
        // 중력 가속도
        Vector3 t_acellation = new Vector3(0, -9.8f, 0);

        // 기체의 위 방향으로 가속
        t_acellation += m_transform.localRotation * new Vector3(0, power, 0);

        // 공기 저항
        t_acellation -= new Vector3(m_velocity.x * m_velocity.x, m_velocity.y * m_velocity.y, m_velocity.z * m_velocity.z) * Time.deltaTime;

        // 속력 결정
        m_velocity += t_acellation * Time.deltaTime;

        // 위치 이동
        m_transform.localPosition += m_velocity * Time.deltaTime;
    }


    public override void update()
    {
        // 전, 후 기울기
        if (Input.GetKey(KeyCode.S))
        {
            if (m_maxRotation < m_rotationFront)
            {
                m_rotationFront = m_maxRotation;
            }
            else
            {
                m_rotationFront += Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else if (Input.GetKey(KeyCode.W))
        {
            if (-m_maxRotation > m_rotationFront)
            {
                m_rotationFront = -m_maxRotation;
            }
            else
            {
                m_rotationFront -= Time.deltaTime * m_rotateSpeedmult;
            }
        }

        // 전, 후 기울기 회복
        if (0 < m_rotationFront)
        {
            m_rotationFront -= Time.deltaTime * m_rotateRestore;
        }
        else if (0 > m_rotationFront)
        {
            m_rotationFront += Time.deltaTime * m_rotateRestore;
        }

        // 좌, 우 기울기
        if (Input.GetKey(KeyCode.D))
        {
            if (m_maxRotation < m_rotationSide)
            {
                m_rotationSide = m_maxRotation;
            }
            else
            {
                m_rotationSide += Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else if (Input.GetKey(KeyCode.A))
        {
            if (-m_maxRotation > m_rotationSide)
            {
                m_rotationSide = -m_maxRotation;
            }
            else
            {
                m_rotationSide -= Time.deltaTime * m_rotateSpeedmult;
            }
        }

        // 좌, 우 기울기 회복
        if (0 < m_rotationSide)
        {
            m_rotationSide -= Time.deltaTime * m_rotateRestore;
        }
        else if (0 > m_rotationSide)
        {
            m_rotationSide += Time.deltaTime * m_rotateRestore;
        }

        // 기체 회전 속도
        if (Input.GetKey(KeyCode.E))
        {
            if (m_maxRotation < m_rotationY)
            {
                m_rotationY = m_maxRotation;
            }
            else
            {
                m_rotationY += Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            if (-m_maxRotation > m_rotationY)
            {
                m_rotationY = -m_maxRotation;
            }
            else
            {
                m_rotationY -= Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else
        {
            // 기체 회전 속도 회복
            if (0 < m_rotationY)
            {
                m_rotationY -= Time.deltaTime * m_rotateSpeedmult;
            }
            else if (0 > m_rotationY)
            {
                m_rotationY += Time.deltaTime * m_rotateSpeedmult;
            }
        }

        // 기체 회전
        float t_rotationX = m_transform.localRotation.eulerAngles.x;
        float t_rotationZ = m_transform.localRotation.eulerAngles.z;
        if (180.0f < t_rotationX)
        {
            t_rotationX -= 360.0f;
        }
        if (180.0f < t_rotationZ)
        {
            t_rotationZ -= 360.0f;
        }
        m_transform.localRotation *= Quaternion.Euler(
            (m_rotationFront - t_rotationX) * Time.deltaTime,
            m_rotationY * Time.deltaTime,
            (m_rotationSide - t_rotationZ) * Time.deltaTime
        );
    }
}
