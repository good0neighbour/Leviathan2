using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class C_GuidedMissle : MonoBehaviour, I_State<E_PlayState>
{
    /* ========== Fields ========== */
    
    [SerializeField] private Transform mp_attachedTarget = null;
    [SerializeField] private Vector3 m_attachingOffset = Vector3.zero;
    [Header("HUD 참조")]
    [SerializeField] private GameObject mp_HUDCanvas = null;
    [SerializeField] private TextMeshProUGUI mp_altitudeText = null;
    [SerializeField] private RectTransform mp_centerCircle = null;
    [SerializeField] private RectTransform mp_movingCircle = null;
    [SerializeField] private RectTransform mp_noise = null;
    [SerializeField] private Volume mp_volume = null;
    private ColorAdjustments mp_colourAdjustment = null;
    private ChromaticAberration mp_chromaticAberration = null;
    private Quaternion m_currentRotationX = Quaternion.Euler(45.0f, 0.0f, 0.0f);
    private Quaternion m_currentRotationY = Quaternion.identity;
    private float m_cameraRotateSpeedmult = 0.0f;
    private float m_movingCircleLerpWeight = 0.0f;
    private float m_saturation = 0.0f;
    private float m_CAIntensity = 0.0f;
    private float m_UIMoveAmount = 0.0f;
#if PLATFORM_STANDALONE_WIN
    private float m_currentScreenHeight = 0.0f;
#endif



    /* ========== Public Methods ========== */

    public void Execute()
    {
        mp_colourAdjustment.saturation.Override(m_saturation);
        mp_chromaticAberration.intensity.Override(m_CAIntensity);
        mp_HUDCanvas.SetActive(true);
    }


    public void ChangeState(E_PlayState t_state)
    {
        mp_colourAdjustment.saturation.Override(0.0f);
        mp_chromaticAberration.intensity.Override(0.0f);
        mp_HUDCanvas.SetActive(false);
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
            if (0.0f < m_currentRotationX.eulerAngles.x && 270.0f > m_currentRotationX.eulerAngles.x)
            {
                float t_amount = -m_cameraRotateSpeedmult * Time.deltaTime;

                m_currentRotationX *= Quaternion.Euler(t_amount, 0.0f, 0.0f);
                mp_movingCircle.localPosition += new Vector3(
                    0.0f,
                    t_amount * m_UIMoveAmount,
                    0.0f
                );
            }
            else
            {
                m_currentRotationX = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (90.0f > m_currentRotationX.eulerAngles.x && 180.0f > m_currentRotationX.eulerAngles.z)
            {
                float t_amount = m_cameraRotateSpeedmult * Time.deltaTime;

                m_currentRotationX *= Quaternion.Euler(t_amount, 0.0f, 0.0f);
                mp_movingCircle.localPosition += new Vector3(
                    0.0f,
                    t_amount * m_UIMoveAmount,
                    0.0f
                );
            }
            else
            {
                m_currentRotationX = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            }
        }

        // 좌, 우 회전
        if (Input.GetKey(KeyCode.A))
        {
            float t_amount = -m_cameraRotateSpeedmult * Time.deltaTime;
            m_currentRotationY *= Quaternion.Euler(0.0f, t_amount, 0.0f);
            mp_movingCircle.localPosition += new Vector3(
                -t_amount * m_UIMoveAmount,
                0.0f,
                0.0f
            );
        }
        else if (Input.GetKey(KeyCode.D))
        {
            float t_amount = m_cameraRotateSpeedmult * Time.deltaTime;
            m_currentRotationY *= Quaternion.Euler(0.0f, t_amount, 0.0f);
            mp_movingCircle.localPosition += new Vector3(
                -t_amount * m_UIMoveAmount,
                0.0f,
                0.0f
            );
        }
        #endregion
        #region 상태 변경
        // 비행기로 상태 변경
        if (Input.GetKeyDown(KeyCode.V))
        {
            ChangeState(E_PlayState.AIRPLANE);
        }
        #endregion
#endif

        // 고도 표시
        mp_altitudeText.text = Mathf.RoundToInt(transform.localPosition.y).ToString();

        // 장식용 중앙 원 회전
        mp_centerCircle.localRotation = Quaternion.Euler(0.0f, 0.0f, transform.localRotation.eulerAngles.y);

        mp_noise.localPosition += new Vector3(0.0f, 5.0f, 0.0f);
        if (m_currentScreenHeight * 0.5f < mp_noise.localPosition.y)
        {
            mp_noise.localPosition -= new Vector3(0.0f, m_currentScreenHeight, 0.0f);
        }
    }


    public void StateFixedUpdate()
    {
        // 비행기에 달라붙는 위치
        transform.localPosition = mp_attachedTarget.localPosition + m_attachingOffset;
        transform.localRotation = mp_attachedTarget.localRotation * m_currentRotationY * m_currentRotationX;
    }



    /* ========== Private Methods ========== */

    private void Awake()
    {
        // 가이드 미사일 설정 가져온다.
        C_GuidedMissleSettings tp_settings = Resources.Load<C_GuidedMissleSettings>("GuidedMissleSettings");
        m_cameraRotateSpeedmult = tp_settings.m_cameraRotateSpeedmult;
        m_movingCircleLerpWeight = tp_settings.m_movingCircleLerpWeight;
        m_saturation = tp_settings.m_saturation;
        m_CAIntensity = tp_settings.m_CAIntensity;

        // 1픽셀 당 각도
        m_UIMoveAmount = Screen.height / Camera.main.fieldOfView;
#if PLATFORM_STANDALONE_WIN
        m_currentScreenHeight = Screen.height;
#endif

        mp_volume.profile.TryGet(out mp_colourAdjustment);
        mp_volume.profile.TryGet(out mp_chromaticAberration);
    }


    // 상태에 영향받지 않는 Update 동작
    private void Update()
    {
        // 장식용 움직이는 원 이동
        mp_movingCircle.localPosition = Vector3.Lerp(
            mp_movingCircle.localPosition,
            Vector3.zero,
            m_movingCircleLerpWeight
        );
    }
}
