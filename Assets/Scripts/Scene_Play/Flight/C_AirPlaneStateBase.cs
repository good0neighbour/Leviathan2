using UnityEngine;
using TMPro;

public abstract class C_AirPlaneStateBase : I_StateBase
{
    /* ========== Fields ========== */

    protected C_AirPlane m_machine = null;
    protected Animator m_animator = null;
    protected Transform m_transform = null;
    protected Vector3 m_airResist = Vector3.zero;
    protected float m_HUDUpDownMoveAmount = 0.0f;
    private RectTransform m_HUDUpDown = null;
    private RectTransform m_directionImage = null;
    private TMP_Text m_velocityText = null;
    private float m_power = 0.0f;
    private float m_maxEnginePower = 0.0f;
#if PLATFORM_STANDALONE_WIN
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
            m_animator.SetFloat("EnginePower", m_power / m_maxEnginePower);
        }
    }


    public Vector3 velocity
    {
        get; set;
    }



    /* ========== Public Methods ========== */

    public C_AirPlaneStateBase(C_AirPlane t_machine, C_AirplaneSettings t_settings)
    {
        m_machine = t_machine;
        m_transform = t_machine.transform;
        t_machine.GetHUDs(out m_HUDUpDown, out m_velocityText, out m_directionImage);

        m_airResist = t_settings.m_airResist;
        m_maxEnginePower = t_settings.m_maxEnginePower;

        // 화면 크기 가져온다.
        m_HUDUpDownMoveAmount = Screen.height / Camera.main.fieldOfView;
#if PLATFORM_STANDALONE_WIN
        m_currentScreenHeight = Screen.height;
#endif
    }


    public abstract void Execute();

    
    public abstract void ChangeState();

    
    public abstract void StateUpdate();

    
    public abstract void StateFixedUpdate();



    /* ========== Protected Methods ========== */

    /// <summary>
    /// 속력 결정
    /// </summary>
    protected Vector3 SetVelocity(Vector3 t_acceleration)
    {
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


    /// <summary>
    /// 헤드업디스플레이 업데이트
    /// </summary>
    protected void HUDUpdate()
    {
#if PLATFORM_STANDALONE_WIN
        // 화면 크기 바뀐 경우 HUD 크기 변경
        if (m_currentScreenHeight != Screen.height)
        {
            float t_FOV = 1.0f / Camera.main.fieldOfView;
            m_currentScreenHeight = Screen.height;
            m_HUDUpDownMoveAmount = m_currentScreenHeight * t_FOV;

            m_HUDUpDown.offsetMax = new Vector2(
                m_HUDUpDown.offsetMax.x,
                m_currentScreenHeight * t_FOV * 90.0f
            );
            m_HUDUpDown.offsetMin = new Vector2(
                m_HUDUpDown.offsetMin.x,
                -m_currentScreenHeight * t_FOV * 90.0f
            );
        }
#endif

        // 기체 각도
        Vector3 t_rotation = m_transform.localRotation.eulerAngles;
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
        Vector3 t_direction = Quaternion.Inverse(m_transform.localRotation) * velocity / t_velocity;

        // 속력 표시
        m_velocityText.text = Mathf.RoundToInt(t_velocity).ToString();

        // 계산량 절약
        float t_radianZ = t_rotation.z * Mathf.Deg2Rad;
        float t_cosZ = Mathf.Cos(t_radianZ);
        float t_sinZ = Mathf.Sin(t_radianZ);
        float t_degreeX = Mathf.Atan(t_direction.x) * Mathf.Rad2Deg * m_HUDUpDownMoveAmount;
        float t_degreeY = Mathf.Atan(t_direction.y) * Mathf.Rad2Deg * m_HUDUpDownMoveAmount;

        // 방향 표시 이미지 위치
        m_directionImage.localPosition = new Vector3(t_degreeX, t_degreeY, 0.0f);

        // 위, 아래 각도 HUD 위치
        m_HUDUpDown.localPosition = new Vector3(
            t_rotation.x * t_sinZ * m_HUDUpDownMoveAmount + t_degreeX * t_cosZ,
            t_rotation.x * t_cosZ * m_HUDUpDownMoveAmount + t_degreeY * Mathf.Abs(t_sinZ),
            0.0f
        );

        // 위, 아래 각도 HUD 회전
        m_HUDUpDown.localRotation = Quaternion.Euler(0.0f, 0.0f, -t_rotation.z);
    }
}
