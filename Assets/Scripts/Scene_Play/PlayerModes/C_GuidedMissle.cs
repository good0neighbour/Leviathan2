using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class C_GuidedMissle : MonoBehaviour, I_State<E_PlayStates>
{
    /* ========== Fields ========== */
    
    [SerializeField] private C_AirPlane mp_airplane = null;
    [Header("HUD 참조")]
    [SerializeField] private GameObject mp_HUDCanvas = null;
    [SerializeField] private GameObject[] mp_enableOnBrowsing = null;
    [SerializeField] private GameObject mp_enableOnLaunching = null;
    [SerializeField] private TextMeshProUGUI mp_altitudeText = null;
    [SerializeField] private Image mp_noiseImage = null;
    [SerializeField] private RectTransform mp_centerCircle = null;
    [SerializeField] private RectTransform mp_movingCircle = null;
    [SerializeField] private RectTransform mp_noise = null;
    [SerializeField] private Volume mp_volume = null;
    [SerializeField] private C_Joystick mp_joystick = null;
    private ColorAdjustments mp_colourAdjustment = null;
    private ChromaticAberration mp_chromaticAberration = null;
    private Material mp_noiseMaterial = null;
    private Transform mp_targetTransform = null;
    private Transform mp_actor = null;
    private Vector3 m_attachingOffset = new Vector3(0.0f, -0.2f, 0.0f);
    private Vector3 m_initialVelocity = Vector3.zero;
    private E_GuidedMissleStates m_currentState = E_GuidedMissleStates.BROWSING;
    private float m_currentRotationX = 45.0f;
    private float m_currentRotationY = 0.0f;
    private float m_cameraRotateSpeed = 0.0f;
    private float m_movingCircleLerpWeight = 0.0f;
    private float m_missleAccelerator = 0.0f;
    private float m_missleVelocity = 0.0f;
    private float m_noiseSpeed = 0.0f;
    private float m_minNoiseAlpha = 0.0f;
    private float m_noiseAlphaSpeed = 0.0f;
    private float m_saturation = 0.0f;
    private float m_CAIntensity = 0.0f;
    private float m_UIMoveAmount = 0.0f;
    private float m_damageRange = 0.0f;
    private float m_angleLimitTop = 0.0f;
    private float m_angleLimitBottom = 0.0f;
    private byte m_damage = 0;
#if PLATFORM_STANDALONE_WIN
    private float m_currentScreenHeight = 0.0f;
#endif

    public static C_GuidedMissle instance
    {
        get;
        private set;
    }



    /* ========== Public Methods ========== */

    public void Execute()
    {
        mp_colourAdjustment.saturation.Override(m_saturation);
        mp_chromaticAberration.intensity.Override(m_CAIntensity);
        StateBrowsingExecute();
        gameObject.SetActive(true);
        mp_HUDCanvas.SetActive(true);
    }


    public void ChangeState(E_PlayStates t_state)
    {
        mp_colourAdjustment.saturation.Override(0.0f);
        mp_chromaticAberration.intensity.Override(0.0f);
        mp_HUDCanvas.SetActive(false);
        gameObject.SetActive(false);
        Camera.main.fieldOfView = 45.0f;
        C_PlayManager.instance.SetState(t_state);
    }


    public void StateUpdate()
    {
#if PLATFORM_STANDALONE_WIN
        // 화면 크기 변경 시
        if (m_currentScreenHeight != Screen.height)
        {
            m_currentScreenHeight = Screen.height;
            m_UIMoveAmount = m_currentScreenHeight / Camera.main.fieldOfView;
        }

        // 상,하 회전
        if (Input.GetKey(KeyCode.W))
        {
            mp_joystick.value.y = 1.0f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            mp_joystick.value.y = -1.0f;
        }
        else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            mp_joystick.value.y = 0.0f;
        }

        // 좌, 우 회전
        if (Input.GetKey(KeyCode.A))
        {
            mp_joystick.value.x = -1.0f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            mp_joystick.value.x = 1.0f;
        }
        else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            mp_joystick.value.x = 0.0f;
        }

        // Airplane으로 상태 변경
        if (Input.GetKeyDown(KeyCode.V) && m_currentState == E_GuidedMissleStates.BROWSING)
        {
            ButtonBack();
        }

        // Actor로 상태 변경
        if (Input.GetKeyDown(KeyCode.B) && m_currentState == E_GuidedMissleStates.BROWSING)
        {
            ButtonDeployActor();
        }

        // 확대 축소
        if (Input.GetKeyDown(KeyCode.C))
        {
            ButtonZoom();
        }
#endif
        // UI 조이스틱 동작
        JoystickControl();

        // 고도 표시
        mp_altitudeText.text = Mathf.RoundToInt(transform.localPosition.y).ToString();

        // 노이즈 애니메이션
        mp_noise.localPosition += new Vector3(0.0f, m_noiseSpeed, 0.0f);
        if (m_currentScreenHeight * 0.5f < mp_noise.localPosition.y)
        {
            mp_noise.localPosition -= new Vector3(0.0f, m_currentScreenHeight, 0.0f);
        }

        // 노이즈 어두워지기
        mp_noiseMaterial.color = new Color(
            1.0f,
            1.0f,
            1.0f,
            mp_noiseMaterial.color.a + (m_minNoiseAlpha - mp_noiseMaterial.color.a) * Time.deltaTime * m_noiseAlphaSpeed
        );

        // 상태에 따른 Update 동작
        switch (m_currentState)
        {
            case E_GuidedMissleStates.BROWSING:
#if PLATFORM_STANDALONE_WIN
                // 미사일 발사
                if (Input.GetKeyDown(KeyCode.F))
                {
                    StateLaunchingExecute();
                }
#endif
                // 장식용 중앙 원 회전
                mp_centerCircle.localRotation = Quaternion.Euler(0.0f, 0.0f, m_currentRotationY);

                // 장식용 움직이는 원 이동
                mp_movingCircle.localPosition = Vector3.Lerp(
                    mp_movingCircle.localPosition,
                    Vector3.zero,
                    m_movingCircleLerpWeight
                );
                return;

            case E_GuidedMissleStates.LAUNCHING:
#if PLATFORM_STANDALONE_WIN
                // 미사일 포기
                if (Input.GetKeyDown(KeyCode.F))
                {
                    MissleExplode();
                }
#endif
                return;
        }
    }


    public void StateFixedUpdate()
    {
        // 회전
        transform.localRotation = mp_targetTransform.localRotation * Quaternion.Euler(m_currentRotationX, m_currentRotationY, 0.0f);

        // 위치
        switch (m_currentState)
        {
            case E_GuidedMissleStates.BROWSING:
                transform.localPosition = mp_targetTransform.localPosition + m_attachingOffset;
                return;

            case E_GuidedMissleStates.LAUNCHING:
                m_initialVelocity.x -= m_initialVelocity.x * Time.fixedDeltaTime;
                m_initialVelocity.y -= m_initialVelocity.y * Time.fixedDeltaTime;
                m_initialVelocity.z -= m_initialVelocity.z * Time.fixedDeltaTime;
                m_missleVelocity += m_missleAccelerator * Time.fixedDeltaTime;
                transform.localPosition += (transform.localRotation * new Vector3(0.0f, 0.0f, m_missleVelocity) + m_initialVelocity) * Time.fixedDeltaTime;
                if (0.0f >= transform.localPosition.y)
                {
                    MissleExplode();
                }
                return;
        }
    }


    public void SetActor(Transform tp_actor)
    {
        mp_actor = tp_actor;
    }


    public void ButtonBack()
    {
        ChangeState(E_PlayStates.AIRPLANE);
    }


    public void ButtonDeployActor()
    {
        // 소환 위치 찾는다.
        RaycastHit t_raycast;
        Physics.Raycast(
            transform.localPosition,
            transform.localRotation * new Vector3(0.0f, 0.0f, 1.0f),
            out t_raycast,
            float.MaxValue,
            1 << LayerMask.NameToLayer("layer_ground")
        );

        // Actor 소환
        mp_actor.localPosition = t_raycast.point;
        mp_actor.localRotation = Quaternion.Euler(0.0f, transform.localRotation.eulerAngles.y, 0.0f);
        mp_actor.gameObject.SetActive(true);

        // 상태 변경
        ChangeState(E_PlayStates.ACTOR);
    }


    public void ButtonZoom()
    {
        switch (Camera.main.fieldOfView)
        {
            case 45.0f:
                Camera.main.fieldOfView = 15.0f;
                break;

            case 15.0f:
                Camera.main.fieldOfView = 45.0f;
                break;
        }
    }


    /// <summary>
    /// 가이드 미사일 폭발
    /// </summary>
    public void MissleExplode()
    {
        foreach (Collider t_col in Physics.OverlapSphere(transform.localPosition, m_damageRange))
        {
            if (t_col.tag.Equals("tag_landForce") || t_col.tag.Equals("tag_oceanForce"))
            {
                t_col.GetComponent<C_Minion>().Hit((byte)(
                    m_damage
                    * (1.0f
                    - Vector3.Distance(transform.localPosition, t_col.transform.localPosition)
                    / m_damageRange)
                ));
            }
        }
        GameObject t_particle = C_ObjectPool.instance.GetObject(E_ObjectPool.EXPLOSION);
        t_particle.transform.localPosition = transform.localPosition;
        t_particle.SetActive(true);
        StateBrowsingExecute();
    }


    /// <summary>
    /// Launching 상태 실행
    /// </summary>
    public void StateLaunchingExecute()
    {
        m_currentState = E_GuidedMissleStates.LAUNCHING;
        m_initialVelocity = mp_airplane.GetState(E_FlightStates.HOVER).velocity;
        m_missleVelocity = 0.0f;
        EnalbingButtons(false);
        mp_centerCircle.gameObject.SetActive(false);
        mp_movingCircle.gameObject.SetActive(false);
    }



    /* ========== Private Methods ========== */

    /// <summary>
    /// Browsing 상태 실행
    /// </summary>
    private void StateBrowsingExecute()
    {
        m_currentState = E_GuidedMissleStates.BROWSING;
        mp_movingCircle.localPosition = Vector3.zero;
        EnalbingButtons(true);
        mp_centerCircle.gameObject.SetActive(true);
        mp_movingCircle.gameObject.SetActive(true);
        mp_noiseMaterial.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }


    private void JoystickControl()
    {
        // 상,하 회전
        float t_amount = -m_cameraRotateSpeed * mp_joystick.value.y * Time.deltaTime;
        m_currentRotationX += t_amount;
        if (m_angleLimitTop > m_currentRotationX)
        {
            m_currentRotationX = m_angleLimitTop;
        }
        else if (m_angleLimitBottom < m_currentRotationX)
        {
            m_currentRotationX = m_angleLimitBottom;
        }
        else
        {
            mp_movingCircle.localPosition += new Vector3(
                0.0f,
                t_amount * m_UIMoveAmount,
                0.0f
            );
        }

        // 좌, 우 회전
        t_amount = m_cameraRotateSpeed * mp_joystick.value.x * Time.deltaTime;
        m_currentRotationY += t_amount;
        mp_movingCircle.localPosition += new Vector3(
            -t_amount * m_UIMoveAmount,
            0.0f,
            0.0f
        );
    }


    private void EnalbingButtons(bool t_isBrowsing)
    {
        foreach (GameObject tp_button in mp_enableOnBrowsing)
        {
            tp_button.SetActive(t_isBrowsing);
        }
        mp_enableOnLaunching.SetActive(!t_isBrowsing);
    }


    private void Awake()
    {
        // 유니티식 싱글턴패턴
        instance = this;

        // 가이드 미사일 설정 가져온다.
        C_GuidedMissleSettings tp_settings = Resources.Load<C_GuidedMissleSettings>("GuidedMissleSettings");
        m_cameraRotateSpeed = tp_settings.m_cameraRotateSpeed;
        m_movingCircleLerpWeight = tp_settings.m_movingCircleLerpWeight;
        m_missleAccelerator = tp_settings.m_missleAccelerator;
        m_noiseSpeed = tp_settings.m_noiseSpeedmult;
        m_minNoiseAlpha = tp_settings.m_minNoiseAlpha;
        m_noiseAlphaSpeed = tp_settings.m_noiseAlphaSpeed;
        m_saturation = tp_settings.m_saturation;
        m_CAIntensity = tp_settings.m_CAIntensity;
        m_damageRange = tp_settings.m_damageRange;
        m_damage = tp_settings.m_damage;
        m_angleLimitTop = tp_settings.m_angleLimit;
        m_angleLimitBottom = 180.0f - m_angleLimitTop;

        // 1픽셀 당 각도
        m_UIMoveAmount = Screen.height / Camera.main.fieldOfView;
#if PLATFORM_STANDALONE_WIN
        m_currentScreenHeight = Screen.height;
#endif

        // 후처리 요소
        mp_volume.profile.TryGet(out mp_colourAdjustment);
        mp_volume.profile.TryGet(out mp_chromaticAberration);

        // 노이즈 메타리얼 복사
        mp_noiseMaterial = new Material(mp_noiseImage.material);
        mp_noiseImage.material = mp_noiseMaterial;

        // 참조
        mp_targetTransform = mp_airplane.transform;

        // 처음에는 비활성화
        gameObject.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        switch (m_currentState)
        {
            case E_GuidedMissleStates.LAUNCHING:
                if (0 < (LayerMask.GetMask("layer_ground") & 1 << other.gameObject.layer)
                    || other.gameObject.tag.Equals("tag_enemy"))
                {
                    MissleExplode();
                }
                break;
        }
    }
}
