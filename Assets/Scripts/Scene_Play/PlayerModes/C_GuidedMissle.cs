using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class C_GuidedMissle : MonoBehaviour, I_State<E_PlayState>
{
    /* ========== Fields ========== */
    
    [SerializeField] private Transform mp_attachedTarget = null;
    [Header("HUD 참조")]
    [SerializeField] private GameObject mp_HUDCanvas = null;
    [SerializeField] private GameObject mp_explosionParticle = null;
    [SerializeField] private TextMeshProUGUI mp_altitudeText = null;
    [SerializeField] private Image mp_noiseImage = null;
    [SerializeField] private RectTransform mp_centerCircle = null;
    [SerializeField] private RectTransform mp_movingCircle = null;
    [SerializeField] private RectTransform mp_noise = null;
    [SerializeField] private Volume mp_volume = null;
    [Header("Actor")]
    [SerializeField] private Transform mp_actor = null;
    private ColorAdjustments mp_colourAdjustment = null;
    private ChromaticAberration mp_chromaticAberration = null;
    private Material mp_noiseMaterial = null;
    private float m_currentRotationX = 45.0f;
    private float m_currentRotationY = 0.0f;
    private Vector3 m_attachingOffset = new Vector3(0.0f, -0.2f, 0.0f);
    private E_GuidedMissleStates m_currentState = E_GuidedMissleStates.BROWSING;
    private float m_cameraRotateSpeed = 0.0f;
    private float m_movingCircleLerpWeight = 0.0f;
    private float m_missleAccelerator = 0.0f;
    private float m_noiseSpeed = 0.0f;
    private float m_minNoiseAlpha = 0.0f;
    private float m_noiseAlphaSpeed = 0.0f;
    private float m_saturation = 0.0f;
    private float m_CAIntensity = 0.0f;
    private float m_UIMoveAmount = 0.0f;
    private float m_missleVelocity = 0.0f;
    private float m_damageRange = 0.0f;
    private byte m_damage = 0;
#if PLATFORM_STANDALONE_WIN
    private float m_currentScreenHeight = 0.0f;
#endif



    /* ========== Public Methods ========== */

    public void Execute()
    {
        mp_colourAdjustment.saturation.Override(m_saturation);
        mp_chromaticAberration.intensity.Override(m_CAIntensity);
        StateBrowsingExecute();
        gameObject.SetActive(true);
        mp_HUDCanvas.SetActive(true);
    }


    public void ChangeState(E_PlayState t_state)
    {
        mp_colourAdjustment.saturation.Override(0.0f);
        mp_chromaticAberration.intensity.Override(0.0f);
        mp_HUDCanvas.SetActive(false);
        gameObject.SetActive(false);
        C_PlayManager.instance.SetState(t_state);
    }


    public void StateUpdate()
    {
#if PLATFORM_STANDALONE_WIN

        #region 화면 크기 변경 시
        if (m_currentScreenHeight != Screen.height)
        {
            m_currentScreenHeight = Screen.height;
            m_UIMoveAmount = m_currentScreenHeight / Camera.main.fieldOfView;
        }
        #endregion
        #region 조작
        // 상,하 회전
        if (Input.GetKey(KeyCode.W))
        {
            if (0.0f < m_currentRotationX && 270.0f > m_currentRotationX)
            {
                float t_amount = -m_cameraRotateSpeed * Time.deltaTime;

                m_currentRotationX += t_amount;
                mp_movingCircle.localPosition += new Vector3(
                    0.0f,
                    t_amount * m_UIMoveAmount,
                    0.0f
                );
            }
            else
            {
                m_currentRotationX = 0.0f;
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (90.0f > m_currentRotationX && 180.0f > m_currentRotationX)
            {
                float t_amount = m_cameraRotateSpeed * Time.deltaTime;

                m_currentRotationX += t_amount;
                mp_movingCircle.localPosition += new Vector3(
                    0.0f,
                    t_amount * m_UIMoveAmount,
                    0.0f
                );
            }
            else
            {
                m_currentRotationX = 90.0f;
            }
        }

        // 좌, 우 회전
        if (Input.GetKey(KeyCode.A))
        {
            float t_amount = -m_cameraRotateSpeed * Time.deltaTime;

            m_currentRotationY += t_amount;
            mp_movingCircle.localPosition += new Vector3(
                -t_amount * m_UIMoveAmount,
                0.0f,
                0.0f
            );
        }
        else if (Input.GetKey(KeyCode.D))
        {
            float t_amount = m_cameraRotateSpeed * Time.deltaTime;

            m_currentRotationY += t_amount;
            mp_movingCircle.localPosition += new Vector3(
                -t_amount * m_UIMoveAmount,
                0.0f,
                0.0f
            );
        }
        #endregion
        #region 상태 변경
        // Airplane으로 상태 변경
        if (Input.GetKeyDown(KeyCode.V) && m_currentState == E_GuidedMissleStates.BROWSING)
        {
            ChangeState(E_PlayState.AIRPLANE);
        }

        // Actor로 상태 변경
        if (Input.GetKeyDown(KeyCode.B) && m_currentState == E_GuidedMissleStates.BROWSING)
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
            ChangeState(E_PlayState.ACTOR);
        }
        #endregion
#endif
        
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
                #region Browsing 상태에서 Update 동작
#if PLATFORM_STANDALONE_WIN
                // 미사일 발사
                if (Input.GetKeyDown(KeyCode.F))
                {
                    StateLaunchingExecute();
                }
#endif
                // 장식용 중앙 원 회전
                mp_centerCircle.localRotation = Quaternion.Euler(0.0f, 0.0f, transform.localRotation.eulerAngles.y);

                // 장식용 움직이는 원 이동
                mp_movingCircle.localPosition = Vector3.Lerp(
                    mp_movingCircle.localPosition,
                    Vector3.zero,
                    m_movingCircleLerpWeight
                );
                #endregion
                return;

            case E_GuidedMissleStates.LAUNCHING:
                #region Launching 상태에서 Update 동작
#if PLATFORM_STANDALONE_WIN
                // 미사일 포기
                if (Input.GetKeyDown(KeyCode.F))
                {
                    MissleExplode();
                }
#endif
                #endregion
                return;

            default:
#if UNITY_EDITOR
                Debug.LogError("잘못된 GuidedMissle 상태");
#endif
                return;
        }
    }


    public void StateFixedUpdate()
    {
        // 회전
        transform.localRotation = mp_attachedTarget.localRotation * Quaternion.Euler(m_currentRotationX, m_currentRotationY, 0.0f);

        // 위치
        switch (m_currentState)
        {
            case E_GuidedMissleStates.BROWSING:
                transform.localPosition = mp_attachedTarget.localPosition + m_attachingOffset;
                return;

            case E_GuidedMissleStates.LAUNCHING:
                m_missleVelocity += m_missleAccelerator * Time.fixedDeltaTime;
                transform.localPosition += transform.localRotation
                    * new Vector3(0.0f, 0.0f, m_missleVelocity * Time.fixedDeltaTime);
                if (0.0f >= transform.localPosition.y)
                {
                    MissleExplode();
                }
                return;

            default:
#if UNITY_EDITOR
                Debug.LogError("잘못된 GuidedMissle 상태");
#endif
                return;
        }
    }



    /* ========== Private Methods ========== */

    /// <summary>
    /// Browsing 상태 실행
    /// </summary>
    private void StateBrowsingExecute()
    {
        m_currentState = E_GuidedMissleStates.BROWSING;
        mp_movingCircle.localPosition = Vector3.zero;
        mp_centerCircle.gameObject.SetActive(true);
        mp_movingCircle.gameObject.SetActive(true);
        mp_noiseMaterial.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }


    /// <summary>
    /// Launching 상태 실행
    /// </summary>
    private void StateLaunchingExecute()
    {
        m_currentState = E_GuidedMissleStates.LAUNCHING;
        m_missleVelocity = 0.0f;
        mp_centerCircle.gameObject.SetActive(false);
        mp_movingCircle.gameObject.SetActive(false);
    }


    /// <summary>
    /// 가이드 미사일 폭발
    /// </summary>
    private void MissleExplode()
    {
        foreach (Collider t_col in Physics.OverlapSphere(transform.localPosition, m_damageRange))
        {
            if (t_col.tag.Equals("tag_enemy"))
            {
                t_col.GetComponent<C_Enemy>().Hit((byte)(
                    m_damage
                    * (1.0f
                    - Vector3.Distance(transform.localPosition, t_col.transform.localPosition)
                    / m_damageRange)
                ));
            }
        }
        GameObject t_particle = Instantiate(mp_explosionParticle);
        t_particle.transform.localPosition = transform.localPosition;
        StateBrowsingExecute();
    }


    private void Awake()
    {
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

        // 처음에는 비활성화
        gameObject.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (0 < (LayerMask.GetMask("layer_ground") & 1 << other.gameObject.layer)
            || other.gameObject.tag.Equals("tag_enemy"))
        {
            MissleExplode();
        }
    }
}
