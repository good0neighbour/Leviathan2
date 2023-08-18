using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class C_GuidedMissle : MonoBehaviour, I_State<E_PlayState>
{
    /* ========== Fields ========== */
    
    [SerializeField] private Transform mp_attachedTarget = null;
    [Header("HUD ����")]
    [SerializeField] private GameObject mp_HUDCanvas = null;
    [SerializeField] private TextMeshProUGUI mp_altitudeText = null;
    [SerializeField] private Image mp_noiseImage = null;
    [SerializeField] private RectTransform mp_centerCircle = null;
    [SerializeField] private RectTransform mp_movingCircle = null;
    [SerializeField] private RectTransform mp_noise = null;
    [SerializeField] private Volume mp_volume = null;
    private ColorAdjustments mp_colourAdjustment = null;
    private ChromaticAberration mp_chromaticAberration = null;
    private Material mp_noiseMaterial = null;
    private Quaternion m_currentRotationX = Quaternion.Euler(45.0f, 0.0f, 0.0f);
    private Quaternion m_currentRotationY = Quaternion.identity;
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
#if PLATFORM_STANDALONE_WIN
    private float m_currentScreenHeight = 0.0f;
#endif



    /* ========== Public Methods ========== */

    public void Execute()
    {
        mp_colourAdjustment.saturation.Override(m_saturation);
        mp_chromaticAberration.intensity.Override(m_CAIntensity);
        StateBrowsingExecute();
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

        #region ȭ�� ũ�� ���� ��
        if (m_currentScreenHeight != Screen.height)
        {
            m_currentScreenHeight = Screen.height;
            m_UIMoveAmount = m_currentScreenHeight / Camera.main.fieldOfView;
        }
        #endregion
        #region ����
        // ��,�� ȸ��
        if (Input.GetKey(KeyCode.W))
        {
            if (0.0f < m_currentRotationX.eulerAngles.x && 270.0f > m_currentRotationX.eulerAngles.x)
            {
                float t_amount = -m_cameraRotateSpeed * Time.deltaTime;

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
                float t_amount = m_cameraRotateSpeed * Time.deltaTime;

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

        // ��, �� ȸ��
        if (Input.GetKey(KeyCode.A))
        {
            float t_amount = -m_cameraRotateSpeed * Time.deltaTime;
            m_currentRotationY *= Quaternion.Euler(0.0f, t_amount, 0.0f);
            mp_movingCircle.localPosition += new Vector3(
                -t_amount * m_UIMoveAmount,
                0.0f,
                0.0f
            );
        }
        else if (Input.GetKey(KeyCode.D))
        {
            float t_amount = m_cameraRotateSpeed * Time.deltaTime;
            m_currentRotationY *= Quaternion.Euler(0.0f, t_amount, 0.0f);
            mp_movingCircle.localPosition += new Vector3(
                -t_amount * m_UIMoveAmount,
                0.0f,
                0.0f
            );
        }
        #endregion
        #region ���� ����
        // ������ ���� ����
        if (Input.GetKeyDown(KeyCode.V) && m_currentState == E_GuidedMissleStates.BROWSING)
        {
            ChangeState(E_PlayState.AIRPLANE);
        }
        #endregion
#endif
        
        // ���� ǥ��
        mp_altitudeText.text = Mathf.RoundToInt(transform.localPosition.y).ToString();

        // ������ �ִϸ��̼�
        mp_noise.localPosition += new Vector3(0.0f, m_noiseSpeed, 0.0f);
        if (m_currentScreenHeight * 0.5f < mp_noise.localPosition.y)
        {
            mp_noise.localPosition -= new Vector3(0.0f, m_currentScreenHeight, 0.0f);
        }

        // ������ ��ο�����
        mp_noiseMaterial.color = new Color(
            1.0f,
            1.0f,
            1.0f,
            mp_noiseMaterial.color.a + (m_minNoiseAlpha - mp_noiseMaterial.color.a) * Time.deltaTime * m_noiseAlphaSpeed
        );

        // ���¿� ���� Update ����
        switch (m_currentState)
        {
            case E_GuidedMissleStates.BROWSING:
                #region Browsing ���¿��� Update ����
#if PLATFORM_STANDALONE_WIN
                // �̻��� �߻�
                if (Input.GetKeyDown(KeyCode.F))
                {
                    StateLaunchingExecute();
                }
#endif
                // ��Ŀ� �߾� �� ȸ��
                mp_centerCircle.localRotation = Quaternion.Euler(0.0f, 0.0f, transform.localRotation.eulerAngles.y);

                // ��Ŀ� �����̴� �� �̵�
                mp_movingCircle.localPosition = Vector3.Lerp(
                    mp_movingCircle.localPosition,
                    Vector3.zero,
                    m_movingCircleLerpWeight
                );
                #endregion
                return;

            case E_GuidedMissleStates.LAUNCHING:
                #region Launching ���¿��� Update ����
#if PLATFORM_STANDALONE_WIN
                // �̻��� ����
                if (Input.GetKeyDown(KeyCode.F))
                {
                    StateBrowsingExecute();
                }
#endif
                #endregion
                return;

            default:
#if UNITY_EDITOR
                Debug.LogError("�߸��� GuidedMissle ����");
#endif
                return;
        }
    }


    public void StateFixedUpdate()
    {
        // ȸ��
        transform.localRotation = mp_attachedTarget.localRotation * m_currentRotationY * m_currentRotationX;

        // ��ġ
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
                    StateBrowsingExecute();
                }
                return;

            default:
#if UNITY_EDITOR
                Debug.LogError("�߸��� GuidedMissle ����");
#endif
                return;
        }
    }



    /* ========== Private Methods ========== */

    /// <summary>
    /// Browsing ���� ����
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
    /// Launching ���� ����
    /// </summary>
    private void StateLaunchingExecute()
    {
        m_currentState = E_GuidedMissleStates.LAUNCHING;
        m_missleVelocity = 0.0f;
        mp_centerCircle.gameObject.SetActive(false);
        mp_movingCircle.gameObject.SetActive(false);
    }


    private void Awake()
    {
        // ���̵� �̻��� ���� �����´�.
        C_GuidedMissleSettings tp_settings = Resources.Load<C_GuidedMissleSettings>("GuidedMissleSettings");
        m_cameraRotateSpeed = tp_settings.m_cameraRotateSpeed;
        m_movingCircleLerpWeight = tp_settings.m_movingCircleLerpWeight;
        m_missleAccelerator = tp_settings.m_missleAccelerator;
        m_noiseSpeed = tp_settings.m_noiseSpeedmult;
        m_minNoiseAlpha = tp_settings.m_minNoiseAlpha;
        m_noiseAlphaSpeed = tp_settings.m_noiseAlphaSpeed;
        m_saturation = tp_settings.m_saturation;
        m_CAIntensity = tp_settings.m_CAIntensity;

        // 1�ȼ� �� ����
        m_UIMoveAmount = Screen.height / Camera.main.fieldOfView;
#if PLATFORM_STANDALONE_WIN
        m_currentScreenHeight = Screen.height;
#endif

        // ��ó�� ���
        mp_volume.profile.TryGet(out mp_colourAdjustment);
        mp_volume.profile.TryGet(out mp_chromaticAberration);

        // ������ ��Ÿ���� ����
        mp_noiseMaterial = new Material(mp_noiseImage.material);
        mp_noiseImage.material = mp_noiseMaterial;
    }
}