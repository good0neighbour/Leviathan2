using UnityEngine;

public class C_StateHover : C_AirPlaneStateBase
{
    /* ========== Fields ========== */

    private float m_rotationFront = 0.0f;
    private float m_rotationSide = 0.0f;
    private float m_rotationY = 0.0f;
    private float m_rotateSpeedmult = 0.0f;
    private float m_rotatePower = 0.0f;
    private float m_reverseRotateSpeedmult = 0.0f;
    private float m_rotateRestorePower = 0.0f;



    /* ========== Public Methods ========== */

    public C_StateHover(Transform t_transform, C_AirplaneSettings t_settings)
    {
        m_transform = t_transform;
        m_rotateSpeedmult = t_settings.m_hoverRotateSpeedmult;
        m_rotatePower = t_settings.m_hoverRotatePower;
        m_rotateRestorePower = t_settings.m_hoverRotateRestorePower;
        m_airResist = t_settings.m_airResist;
        m_reverseRotateSpeedmult = m_rotateSpeedmult * 0.5f;
    }


    public override void ChangeState()
    {
        
    }


    public override void Execute()
    {
        
    }


    public override void FixedUpdate()
    {
        // 기체의 위 방향으로 가속
        Vector3 t_acellation = new Vector3(0.0f, power, 0.0f);

        // 공기저항과 양력
        t_acellation += AirResistAndLiftPower();

        // 중력 가속도 및 가속 방향
        t_acellation = m_transform.localRotation * t_acellation + new Vector3(0, -9.8f, 0);

        // 속력 결정
        m_velocity += t_acellation * Time.deltaTime;

        // 위치 이동
        m_transform.localPosition += m_velocity * Time.deltaTime;

        // 기체 회전
        m_transform.localRotation *= Quaternion.Euler(new Vector3(m_rotationFront, m_rotationY, m_rotationSide) * Time.deltaTime * m_rotatePower);

        // 기체 회전 회복
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
            (-t_rotationX) * Time.deltaTime * m_rotateRestorePower,
            0.0f,
            (-t_rotationZ) * Time.deltaTime * m_rotateRestorePower
        );
    }


    public override void Update()
    {
        // 전, 후 기울기
        if (Input.GetKey(KeyCode.W))
        {
            if (1 < m_rotationFront)
            {
                m_rotationFront = 1;
            }
            else
            {
                m_rotationFront += Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (-1 > m_rotationFront)
            {
                m_rotationFront = -1;
            }
            else
            {
                m_rotationFront -= Time.deltaTime * m_rotateSpeedmult;
            }
        }

        // 전, 후 기울기 속도 감소
        if (0 < m_rotationFront)
        {
            m_rotationFront -= Time.deltaTime * m_reverseRotateSpeedmult;
        }
        else if (0 > m_rotationFront)
        {
            m_rotationFront += Time.deltaTime * m_reverseRotateSpeedmult;
        }

        // 좌, 우 기울기
        if (Input.GetKey(KeyCode.A))
        {
            if (1 < m_rotationSide)
            {
                m_rotationSide = 1;
            }
            else
            {
                m_rotationSide += Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (-1 > m_rotationSide)
            {
                m_rotationSide = -1;
            }
            else
            {
                m_rotationSide -= Time.deltaTime * m_rotateSpeedmult;
            }
        }

        // 좌, 우 기울기 속도 김소
        if (0 < m_rotationSide)
        {
            m_rotationSide -= Time.deltaTime * m_reverseRotateSpeedmult;
        }
        else if (0 > m_rotationSide)
        {
            m_rotationSide += Time.deltaTime * m_reverseRotateSpeedmult;
        }

        // 기체 회전 속도
        if (Input.GetKey(KeyCode.E))
        {
            if (1 < m_rotationY)
            {
                m_rotationY = 1;
            }
            else
            {
                m_rotationY += Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            if (-1 > m_rotationY)
            {
                m_rotationY = -1;
            }
            else
            {
                m_rotationY -= Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else
        {
            // 기체 회전 속도 감소
            if (0 < m_rotationY)
            {
                m_rotationY -= Time.deltaTime * m_reverseRotateSpeedmult;
            }
            else if (0 > m_rotationY)
            {
                m_rotationY += Time.deltaTime * m_reverseRotateSpeedmult;
            }
        }
    }
}
