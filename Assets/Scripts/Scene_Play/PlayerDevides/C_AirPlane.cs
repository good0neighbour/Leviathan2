using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class C_AirPlane : MonoBehaviour, I_StateBase
{
    /* ========== Fields ========== */

    [SerializeField][Range(0.0f, 100.0f)] private float m_power = 9.8f;
    [SerializeField][Range(0.0f, 1.0f)] private float m_stealth = 0.0f;
    [Header("HUD 참조")]
    [SerializeField] private RectTransform m_HUDUpDown = null;
    [SerializeField] private RectTransform m_directionImage = null;
    [SerializeField] private RectTransform m_powerImage = null;
    [SerializeField] private TMP_Text m_velocityText = null;
    [SerializeField] private TMP_Text m_altitudeText = null;
    private C_AirPlaneStateBase[] m_state = null;
    private Material[] m_materials = new Material[2];
    private E_FlightStates m_currentState = E_FlightStates.HOVER;
    private float m_currentPower = 0.0f;
    private float m_maxEnginePower = 0.0f;
    private float m_minEnginePower = 0.0f;
    private float m_powerMovement = 0.0f;
    private float m_powerImageLength = 0.0f;
    private byte m_stealthActive = 0;
#if PLATFORM_STANDALONE_WIN
    private float m_powerImageRaito = 0.0f;
    private float m_currentScreenHeight = 0.0f;
#endif



    /* ========== Public Methods ========== */

    /// <summary>
    /// 상태 변경
    /// </summary>
    public void SetState(E_FlightStates t_state)
    {
        // 상태 변경
        m_currentState = t_state;
    }


    /// <summary>
    /// 상태 클래스 반환
    /// </summary>
    public C_AirPlaneStateBase GetState(E_FlightStates t_state)
    {
        return m_state[(int)t_state];
    }


    /// <summary>
    /// 헤드업디스플레이 가져오기
    /// </summary>
    public void GetHUDs(out RectTransform t_HUDUpDown, out TMP_Text t_velocityText, out RectTransform t_directionImage)
    {
        t_HUDUpDown = m_HUDUpDown;
        t_velocityText = m_velocityText;
        t_directionImage = m_directionImage;
    }


    public void Execute()
    {

    }

    
    public void ChangeState()
    {

    }


    public void StateUpdate()
    {
#if PLATFORM_STANDALONE_WIN

        #region HUD 크기 변경
        // 화면 크기 변경 시
        if (m_currentScreenHeight != Screen.height)
        {
            m_currentScreenHeight = Screen.height;
            m_powerImageLength = m_powerImageRaito * m_currentScreenHeight;
            EnginePowerUIUpdate();
        }
        #endregion
        #region 조작
        // 엔진 출력 제어
        if (Input.GetKey(KeyCode.LeftShift) && m_maxEnginePower > m_power)
        {
            m_power += Time.deltaTime * m_powerMovement;

            // 엔진 출력 표시
            EnginePowerUIUpdate();
        }
        else if (Input.GetKey(KeyCode.LeftControl) && m_minEnginePower < m_power)
        {
            m_power -= Time.deltaTime * m_powerMovement;

            // 엔진 출력 표시
            EnginePowerUIUpdate();
        }

        // 은폐 (임시)
        if (Input.GetKeyDown(KeyCode.C) && 0 == (m_stealthActive & 0b10))
        {
            if (1 <= (1 & m_stealthActive))
            {
                m_stealthActive -= 0b01;
            }
            else
            {
                m_stealthActive += 0b01;
            }

            m_stealthActive += 0b10;
        }
        #endregion
#endif

        // 다형성
        m_state[(int)m_currentState].StateUpdate();

        // 고도 표시
        m_altitudeText.text = Mathf.RoundToInt(transform.localPosition.y).ToString();
    }


    public void StateFixedUpdate()
    {
        
    }



    /* ========== Private Methods ========== */

    /// <summary>
    /// HUD 생성
    /// </summary>
    private void CreateHUD(Color32 t_HUDColour, float t_HUDColourDarkMultiply, float t_HUDLineWidth, float t_HUDHorizonWidthMultiply, float t_HUDTextSize)
    {
        for (byte t_i = 0; t_i <= 36; ++t_i)
        {
            float t_height = 1.0f / 36.0f * t_i;

            #region 위, 아래 각도 표시용 HUD 선분
            // 좌측
            GameObject t_gameObject = new GameObject("HUDUpDownLineImageLeft", typeof(CanvasRenderer), typeof(Image));
            t_gameObject.transform.SetParent(m_HUDUpDown);

            RectTransform t_rectTransform = t_gameObject.GetComponent<RectTransform>();
            t_rectTransform.offsetMax = Vector2.zero;
            t_rectTransform.offsetMin = Vector2.zero;
            t_rectTransform.anchorMax = new Vector2(0.45f, t_height + t_HUDLineWidth);
            t_rectTransform.anchorMin = new Vector2(0.1f, t_height - t_HUDLineWidth);

            Image t_image = t_gameObject.GetComponent<Image>();

            if (18 > t_i)
            {
                t_image.color = new Color32(
                    (byte)(t_HUDColour.r * t_HUDColourDarkMultiply),
                    (byte)(t_HUDColour.g * t_HUDColourDarkMultiply),
                    (byte)(t_HUDColour.b * t_HUDColourDarkMultiply),
                    t_HUDColour.a);
            }
            else
            {
                t_image.color = t_HUDColour;
                switch (t_i)
                {
                    case 18:
                        t_rectTransform.anchorMax = new Vector2(0.45f, 1.0f / 36.0f * t_i + t_HUDLineWidth * t_HUDHorizonWidthMultiply);
                        t_rectTransform.anchorMin = new Vector2(0.0f, 1.0f / 36.0f * t_i - t_HUDLineWidth * t_HUDHorizonWidthMultiply);
                        break;
                }
            }

            // 우측
            t_gameObject = new GameObject("HUDUpDownLineImageRight", typeof(CanvasRenderer), typeof(Image));
            t_gameObject.transform.SetParent(m_HUDUpDown);

            t_rectTransform = t_gameObject.GetComponent<RectTransform>();
            t_rectTransform.offsetMax = Vector2.zero;
            t_rectTransform.offsetMin = Vector2.zero;
            t_rectTransform.anchorMax = new Vector2(0.9f, t_height + t_HUDLineWidth);
            t_rectTransform.anchorMin = new Vector2(0.55f, t_height - t_HUDLineWidth);

            t_image = t_gameObject.GetComponent<Image>();

            if (18 > t_i)
            {
                t_image.color = new Color32(
                    (byte)(t_HUDColour.r * t_HUDColourDarkMultiply),
                    (byte)(t_HUDColour.g * t_HUDColourDarkMultiply),
                    (byte)(t_HUDColour.b * t_HUDColourDarkMultiply),
                    t_HUDColour.a
                );
            }
            else
            {
                t_image.color = t_HUDColour;
                switch (t_i)
                {
                    case 18:
                        t_rectTransform.anchorMax = new Vector2(1.0f, 1.0f / 36.0f * t_i + t_HUDLineWidth * t_HUDHorizonWidthMultiply);
                        t_rectTransform.anchorMin = new Vector2(0.55f, 1.0f / 36.0f * t_i - t_HUDLineWidth * t_HUDHorizonWidthMultiply);
                        break;
                }
            }
            #endregion

            // 0도는 숫자 생략
            switch (t_i)
            {
                case 18:
                    continue;
            }

            #region 각도 숫자 표시
            // 좌측
            t_gameObject = new GameObject("HUDUpDownTextLeft", typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            t_gameObject.transform.SetParent(m_HUDUpDown);

            t_rectTransform = t_gameObject.GetComponent<RectTransform>();
            t_rectTransform.offsetMax = Vector2.zero;
            t_rectTransform.offsetMin = Vector2.zero;
            t_rectTransform.anchorMax = new Vector2(0.09f, t_height + t_HUDTextSize);
            t_rectTransform.anchorMin = new Vector2(0.0f, t_height - t_HUDTextSize);

            TextMeshProUGUI t_text = t_gameObject.GetComponent<TextMeshProUGUI>();
            t_text.text = ((t_i - 18) * 5).ToString();
            t_text.enableAutoSizing = true;
            t_text.fontSizeMax = 100.0f;
            t_text.fontSizeMin = 0.0f;
            t_text.alignment = TextAlignmentOptions.Right;

            if (18 > t_i)
            {
                t_text.color = new Color32(
                    (byte)(t_HUDColour.r * t_HUDColourDarkMultiply),
                    (byte)(t_HUDColour.g * t_HUDColourDarkMultiply),
                    (byte)(t_HUDColour.b * t_HUDColourDarkMultiply),
                    t_HUDColour.a
                );
            }
            else
            {
                t_text.color = t_HUDColour;
            }

            // 우측
            t_gameObject = new GameObject("HUDUpDownTextRight", typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            t_gameObject.transform.SetParent(m_HUDUpDown);

            t_rectTransform = t_gameObject.GetComponent<RectTransform>();
            t_rectTransform.offsetMax = Vector2.zero;
            t_rectTransform.offsetMin = Vector2.zero;
            t_rectTransform.anchorMax = new Vector2(1.0f, t_height + t_HUDTextSize);
            t_rectTransform.anchorMin = new Vector2(0.91f, t_height - t_HUDTextSize);

            t_text = t_gameObject.GetComponent<TextMeshProUGUI>();
            t_text.text = ((t_i - 18) * 5).ToString();
            t_text.enableAutoSizing = true;
            t_text.fontSizeMax = 100.0f;
            t_text.fontSizeMin = 0.0f;
            t_text.alignment = TextAlignmentOptions.Left;

            if (18 > t_i)
            {
                t_text.color = new Color32(
                    (byte)(t_HUDColour.r * t_HUDColourDarkMultiply),
                    (byte)(t_HUDColour.g * t_HUDColourDarkMultiply),
                    (byte)(t_HUDColour.b * t_HUDColourDarkMultiply),
                    t_HUDColour.a
                );
            }
            else
            {
                t_text.color = t_HUDColour;
            }
            #endregion
        }
    }


    /// <summary>
    /// 엔진 출력 표시 업데이트
    /// </summary>
    private void EnginePowerUIUpdate()
    {
        m_powerImage.localPosition = new Vector3(m_powerImage.localPosition.x, m_power / m_maxEnginePower * m_powerImageLength - m_powerImageLength * 0.5f, 0.0f);
    }


    private void Awake()
    {
        // 항공기 설정 가져온다.
        C_AirplaneSettings t_settings = Resources.Load<C_AirplaneSettings>("AirplaneSettings");
        m_maxEnginePower = t_settings.m_maxEnginePower;
        m_minEnginePower = t_settings.m_minEnginePower;
        m_powerMovement = t_settings.m_powerMovement;
        m_powerImageLength = t_settings.m_powerImageLength * Screen.height;
#if PLATFORM_STANDALONE_WIN
        m_currentScreenHeight = Screen.height;
        m_powerImageRaito = t_settings.m_powerImageLength;
#endif

        // 애니메이터 가져온다.
        Animator t_animator = GetComponent<Animator>();

        // 상태 클래스 생성
        m_state = new C_AirPlaneStateBase[(int)E_FlightStates.END];
        m_state[(int)E_FlightStates.HOVER] = new C_StateHover(this, t_settings, t_animator);
        m_state[(int)E_FlightStates.FLIGHT] = new C_StateFlight(this, t_settings, t_animator);

        // 항공기 메타리얼 복사, 인덱스 0은 외곽선 메타리얼, 인덱스 1은 주 메타리얼
        MeshRenderer[] t_renderers = GetComponentsInChildren<MeshRenderer>();
        m_materials[0] = new Material(t_renderers[0].materials[0]);
        m_materials[1] = new Material(t_renderers[0].materials[1]);

        // 복사한 메타리얼 붙여넣기
        for (byte t_i = 0; t_i < t_renderers.Length; ++t_i)
        {
            t_renderers[t_i].materials = m_materials;
        }

        // HUD 크기 화면에 맞춤
        float t_FOV = 1.0f / Camera.main.fieldOfView;
        m_HUDUpDown.offsetMax = new Vector2(
            m_HUDUpDown.offsetMax.x,
            Screen.height * t_FOV * 90.0f
        );
        m_HUDUpDown.offsetMin = new Vector2(
            m_HUDUpDown.offsetMin.x,
            -Screen.height * t_FOV * 90.0f
        );

        // HUD 초기화
        CreateHUD(
            t_settings.m_HUDColour,
            t_settings.m_HUDColourDarkMultiply,
            t_settings.m_HUDLineWidth,
            t_settings.m_HUDHorizonWidthMultiply,
            t_settings.m_HUDTextSize
        );
        EnginePowerUIUpdate();
    }


    // 상태에 영향받지 않는 Update 동작
    private void Update()
    {
        // 은폐 메타리얼 값 전달
        if (1 <= (m_stealthActive & 0b10))
        {
            if (1 <= (0b01 & m_stealthActive))
            {
                m_stealth += Time.deltaTime;
                if (1.0f < m_stealth)
                {
                    m_stealth = 1.0f;
                    m_stealthActive -= 0b10;
                }
            }
            else
            {
                m_stealth -= Time.deltaTime;
                if (0.0f > m_stealth)
                {
                    m_stealth = 0.0f;
                    m_stealthActive -= 0b10;
                }
            }

            // 메타리얼 값 전달, 인덱스 0은 외곽선 메타리얼, 인덱스 1은 주 메타리얼
            m_materials[0].SetFloat("_DissolveAmount", m_stealth);
            m_materials[1].SetFloat("_DissolveAmount", m_stealth);
        }
    }


    // 상태에 영향받지 않는 FixedUpdate 동작
    private void FixedUpdate()
    {
        // 엔진 출력 전달
        if (m_currentPower != m_power)
        {
            m_currentPower = m_power;
            m_state[(int)m_currentState].power = m_power;
        }

        // 다형성
        m_state[(int)m_currentState].StateFixedUpdate();

        // 지면 뚫기 방지
        if (0.0f > transform.localPosition.y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, 0.0f, transform.localPosition.z);
        }
    }
}
