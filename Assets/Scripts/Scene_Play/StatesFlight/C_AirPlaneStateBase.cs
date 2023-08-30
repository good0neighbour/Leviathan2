using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class C_AirPlaneStateBase : I_State<E_FlightStates>
{
    /* ========== Fields ========== */

    protected C_AirPlane mp_machine = null;
    protected Animator mp_animator = null;
    protected Transform mp_transform = null;
    protected C_Joystick mp_joystick = null;
    protected C_Slider mp_rotationY = null;
    protected Vector3 m_airResist = Vector3.zero;
    protected float m_HUDUpDownMoveAmount = 0.0f;
    private RectTransform mp_HUDUpDown = null;
    private RectTransform mp_directionImage = null;
    private TextMeshProUGUI mp_velocityText = null;
    private Rigidbody mp_rigidbody = null;
    private float m_power = 0.0f;
    private float m_maxEnginePower = 0.0f;
#if PLATFORM_STANDALONE_WIN
    private float m_rotateSpeedmult = 0.0f;
    private int m_currentScreenHeight = 0;
#endif



    /* ========== Properties ========== */

    public float power
    {
        protected get
        {
            return m_power;
        }
        set
        {
            m_power = value;
            mp_animator.SetFloat("EnginePower", m_power / m_maxEnginePower);
        }
    }


    public Vector3 velocity
    {
        get
        {
            return mp_rigidbody.velocity;
        }
        set
        {
            mp_rigidbody.velocity = value;
        }
    }



    /* ========== Public Methods ========== */

    public C_AirPlaneStateBase(C_AirPlane tp_machine, C_AirplaneSettings tp_settings)
    {
        mp_machine = tp_machine;
        mp_transform = tp_machine.transform;
        mp_rigidbody = tp_machine.GetComponent<Rigidbody>();
        tp_machine.GetHUDs(out mp_HUDUpDown, out mp_velocityText, out mp_directionImage, out mp_joystick, out mp_rotationY);

        m_airResist = tp_settings.m_airResist;
        m_maxEnginePower = tp_settings.m_maxEnginePower;

        // 화면 크기 가져온다.
        m_HUDUpDownMoveAmount = Screen.height / Camera.main.fieldOfView;

#if PLATFORM_STANDALONE_WIN
        m_rotateSpeedmult = tp_settings.m_hoverRotateSpeedmult;
        m_currentScreenHeight = Screen.height;
#endif
    }


    public virtual void StateUpdate()
    {
#if PLATFORM_STANDALONE_WIN
        // 전, 후 기울기
        if (Input.GetKey(KeyCode.W))
        {
            if (1.0f < mp_joystick.value.y)
            {
                mp_joystick.value.y = 1.0f;
            }
            else
            {
                mp_joystick.value.y += Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (-1.0f > mp_joystick.value.y)
            {
                mp_joystick.value.y = -1.0f;
            }
            else
            {
                mp_joystick.value.y -= Time.deltaTime * m_rotateSpeedmult;
            }
        }

        // 좌, 우 기울기
        if (Input.GetKey(KeyCode.A))
        {
            if (-1.0f > mp_joystick.value.x)
            {
                mp_joystick.value.x = -1.0f;
            }
            else
            {
                mp_joystick.value.x -= Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (1.0f < mp_joystick.value.x)
            {
                mp_joystick.value.x = 1.0f;
            }
            else
            {
                mp_joystick.value.x += Time.deltaTime * m_rotateSpeedmult;
            }
        }

        // 기체 회전 속도
        if (Input.GetKey(KeyCode.E))
        {
            if (1.0f < mp_rotationY.value)
            {
                mp_rotationY.value = 1.0f;
            }
            else
            {
                mp_rotationY.value += Time.deltaTime * m_rotateSpeedmult;
            }
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            if (-1.0f > mp_rotationY.value)
            {
                mp_rotationY.value = -1.0f;
            }
            else
            {
                mp_rotationY.value -= Time.deltaTime * m_rotateSpeedmult;
            }
        }

        // 비행 상태 변경
        if (Input.GetKeyDown(KeyCode.F))
        {
            SwitchMode();
        }
#endif
    }


    public abstract void Execute();

    
    public abstract void ChangeState(E_FlightStates t_state);

    
    public abstract void StateFixedUpdate();


    public abstract void SwitchMode();



    /* ========== Protected Methods ========== */

    /// <summary>
    /// 속력 결정
    /// </summary>
    protected void SetVelocity(Vector3 t_acceleration)
    {
        // 로칼 좌표계 기준 속력
        Vector3 t_velocity = Quaternion.Inverse(mp_transform.localRotation) * velocity
        
            // 가속
            + t_acceleration * Time.fixedDeltaTime;

        // 기체 방향
        velocity = mp_transform.localRotation

            // 공기저항
            * new Vector3(
                t_velocity.x * (1.0f - m_airResist.x * Mathf.Abs(t_velocity.x)),
                t_velocity.y * (1.0f - m_airResist.y * Mathf.Abs(t_velocity.y)),
                t_velocity.z * (1.0f - m_airResist.z * Mathf.Abs(t_velocity.z))
            )

            // 중력가속도
            + new Vector3(0.0f, -9.8f, 0.0f) * Time.fixedDeltaTime;
    }


    /// <summary>
    /// 속력 결정 및 로칼 좌표계 기준 속력
    /// </summary>
    protected void SetVelocity(Vector3 t_acceleration, out float t_localVelocityZ)
    {
        // 로칼 좌표계 기준 속력
        Vector3 t_velocity = Quaternion.Inverse(mp_transform.localRotation) * velocity

            // 가속
            + t_acceleration * Time.fixedDeltaTime;

        // 로칼 좌표계 기준 전방 방향 속력
        t_localVelocityZ = t_velocity.z;

        // 기체 방향
        velocity = mp_transform.localRotation

            // 공기저항
            * new Vector3(
                t_velocity.x * (1.0f - m_airResist.x * Mathf.Abs(t_velocity.x)),
                t_velocity.y * (1.0f - m_airResist.y * Mathf.Abs(t_velocity.y)),
                t_velocity.z * (1.0f - m_airResist.z * Mathf.Abs(t_velocity.z))
            )

            // 중력가속도
            + new Vector3(0.0f, -9.8f, 0.0f) * Time.fixedDeltaTime;
    }


    /// <summary>
    /// 헤드업디스플레이 업데이트
    /// </summary>
    protected void HUDUpdate(bool t_dynamicPos)
    {
#if PLATFORM_STANDALONE_WIN
        // 화면 크기 바뀐 경우 HUD 크기 변경
        if (m_currentScreenHeight != Screen.height)
        {
            float t_FOV = 1.0f / Camera.main.fieldOfView;
            m_currentScreenHeight = Screen.height;
            m_HUDUpDownMoveAmount = m_currentScreenHeight * t_FOV;

            mp_HUDUpDown.offsetMax = new Vector2(
                mp_HUDUpDown.offsetMax.x,
                m_currentScreenHeight * t_FOV * 90.0f
            );
            mp_HUDUpDown.offsetMin = new Vector2(
                mp_HUDUpDown.offsetMin.x,
                -m_currentScreenHeight * t_FOV * 90.0f
            );
        }
#endif

        // 기체 각도
        Vector3 t_rotation = mp_transform.localRotation.eulerAngles;
        if (180.0f < t_rotation.x)
        {
            t_rotation.x -= 360.0f;
        }
        if (180.0f < t_rotation.y)
        {
            t_rotation.y -= 360.0f;
        }
        if (180.0f < t_rotation.z)
        {
            t_rotation.z -= 360.0f;
        }

        // 기체 이동 방향
        float t_velocity = Mathf.Sqrt(velocity.x * velocity.x + velocity.y * velocity.y + velocity.z * velocity.z);
        Vector3 t_direction;

        switch (t_velocity)
        {
            case 0.0f:
                t_direction = new Vector3(0.0f, 0.0f, 1.0f);
                break;

            default:
                t_direction = Quaternion.Inverse(mp_transform.localRotation) * velocity / t_velocity;
                break;
        }
        
        // 속력 표시
        mp_velocityText.text = Mathf.RoundToInt(t_velocity).ToString();

        // 계산량 절약
        float t_radianZ = t_rotation.z * Mathf.Deg2Rad;
        float t_cosZ = Mathf.Cos(t_radianZ);
        float t_sinZ = Mathf.Sin(t_radianZ);
        float t_degreeX = Mathf.Atan(t_direction.x) * Mathf.Rad2Deg * m_HUDUpDownMoveAmount;
        float t_degreeY = Mathf.Atan(t_direction.y) * Mathf.Rad2Deg * m_HUDUpDownMoveAmount;

        // 방향 표시 이미지 위치
        mp_directionImage.localPosition = new Vector3(t_degreeX, t_degreeY, 0.0f);

        // 위, 아래 각도 HUD 회전
        mp_HUDUpDown.localRotation = Quaternion.Euler(0.0f, 0.0f, -t_rotation.z);

        // 위, 아래 각도 HUD 위치
        if (t_dynamicPos)
        {
            mp_HUDUpDown.localPosition = new Vector3(
                t_degreeX,
                t_degreeY,
                0.0f
            ) + mp_HUDUpDown.localRotation * new Vector3(
                0.0f,
                t_rotation.x * m_HUDUpDownMoveAmount,
                0.0f
            );
        }
        else
        {
            mp_HUDUpDown.localPosition = new Vector3(
                t_rotation.x * t_sinZ * m_HUDUpDownMoveAmount,
                t_rotation.x * t_cosZ * m_HUDUpDownMoveAmount,
                0.0f
            );
        }
    }
}
