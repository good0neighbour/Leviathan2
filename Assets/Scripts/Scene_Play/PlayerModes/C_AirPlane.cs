using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class C_AirPlane : MonoBehaviour, I_State<E_PlayStates>, I_StateMachine<E_FlightStates>, I_Hitable
{
    /* ========== Fields ========== */

    [Header("HUD 참조")]
    [SerializeField] private GameObject mp_HUDCanvas = null;
    [SerializeField] private GameObject mp_guidedMissileButton = null;
    [SerializeField] private RectTransform mp_HUDUpDown = null;
    [SerializeField] private RectTransform mp_directionImage = null;
    [SerializeField] private TextMeshProUGUI mp_velocityText = null;
    [SerializeField] private TextMeshProUGUI mp_altitudeText = null;
    [SerializeField] private C_Joystick mp_joystick = null;
    [SerializeField] private Slider mp_powerSlider = null;
    [SerializeField] private C_Slider mp_rotationY = null;
    [Header("Audio")]
    [SerializeField] private AudioSource mp_engineAudioSource = null;
    [SerializeField] private AudioClip mp_hoverEngine = null;
    [SerializeField] private AudioClip mp_flightEngine = null;
    private C_AirPlaneStateBase[] mp_state = null;
    private Material[] mp_materials = new Material[2];
    private E_FlightStates m_currentState = E_FlightStates.HOVER;
    private float m_power = 9.83f;
    private float m_stealth = 0.0f;
    private float m_curPower = 0.0f;
    private float m_maxEnginePower = 0.0f;
    private float m_minEnginePower = 0.0f;
    private float m_minEngineSoundSpeed = 0.0f;
    private float m_engineSoundSpeedMult = 0.0f;
    private short m_hitPoint = 0;
    private byte m_stealthActive = 0;
    private bool m_hitMessage = true;
#if PLATFORM_STANDALONE_WIN
    private float m_powerMovement = 0.0f;
#endif



    /* ========== Public Methods ========== */

    /// <summary>
    /// 상태 변경
    /// </summary>
    public void SetState(E_FlightStates t_state)
    {
        // 상태 변경
        m_currentState = t_state;

        switch (m_currentState)
        {
            case E_FlightStates.HOVER:
                mp_guidedMissileButton.SetActive(true);
                mp_engineAudioSource.clip = mp_hoverEngine;
                break;

            case E_FlightStates.FLIGHT:
                mp_guidedMissileButton.SetActive(false);
                mp_engineAudioSource.clip = mp_flightEngine;
                break;
        }
        mp_engineAudioSource.Play();
    }


    /// <summary>
    /// 상태 클래스 반환
    /// </summary>
    public C_AirPlaneStateBase GetState(E_FlightStates t_state)
    {
        return mp_state[(int)t_state];
    }


    /// <summary>
    /// 헤드업디스플레이 가져오기
    /// </summary>
    public void GetHUDs(out RectTransform tp_HUDUpDown, out TextMeshProUGUI tp_velocityText, out RectTransform tp_directionImage, out C_Joystick tp_joystick, out C_Slider tp_rotationY)
    {
        tp_HUDUpDown = mp_HUDUpDown;
        tp_velocityText = mp_velocityText;
        tp_directionImage = mp_directionImage;
        tp_joystick = mp_joystick;
        tp_rotationY = mp_rotationY;
    }


    public void Execute()
    {
        m_hitMessage = false;
        mp_HUDCanvas.SetActive(true);
    }

    
    public void ChangeState(E_PlayStates t_state)
    {
        switch (m_currentState)
        {
            case E_FlightStates.HOVER:
                C_PlayManager.instance.SetState(t_state);
                mp_HUDCanvas.SetActive(false);
                return;

            default:
                return;
        }
    }


    public void StateUpdate()
    {
#if PLATFORM_STANDALONE_WIN

        #region 조작
        // 엔진 출력 제어
        if (Input.GetKey(KeyCode.LeftShift) && m_maxEnginePower > m_power)
        {
            mp_powerSlider.value += Time.deltaTime * m_powerMovement;
        }
        else if (Input.GetKey(KeyCode.LeftControl) && m_minEnginePower < m_power)
        {
            mp_powerSlider.value -= Time.deltaTime * m_powerMovement;
        }

        // 은폐 (임시)
        if (Input.GetKeyDown(KeyCode.C))
        {
            ButtonStealth();
        }
        // 가이드 미사일 화면으로 전환
        else if (Input.GetKeyDown(KeyCode.V))
        {
            ButtonGuidedMissile();
        }
        #endregion
#endif
        // 엔진 출력 전달
        m_power = mp_powerSlider.value;

        mp_engineAudioSource.pitch = m_minEngineSoundSpeed + m_power * m_engineSoundSpeedMult;

        // 다형성
        mp_state[(int)m_currentState].StateUpdate();

        // 고도 표시
        mp_altitudeText.text = Mathf.RoundToInt(transform.localPosition.y).ToString();
    }


    public void StateFixedUpdate()
    {
        
    }


    public void ButtonSwitchMode()
    {
        mp_state[(int)m_currentState].SwitchMode();
    }


    public void ButtonGuidedMissile()
    {
        ChangeState(E_PlayStates.GUIDEDMISSILE);
    }


    public void ButtonStealth()
    {
        switch (m_stealthActive & C_Constants.STEALTH_ANIMATION)
        {
            case 0:
                C_AudioManager.instance.PlayAuido(E_AudioType.STEALTH);
                if (1 <= (C_Constants.STEALTH_ENABLE & m_stealthActive))
                {
                    m_stealthActive ^= C_Constants.STEALTH_ENABLE;
                    tag = "tag_player";
                }
                else
                {
                    m_stealthActive |= C_Constants.STEALTH_ENABLE;
                    tag = "Untagged";
                }
                m_stealthActive |= C_Constants.STEALTH_ANIMATION;
                return;

            default:
                return;
        }
    }


    public void Hit(byte t_damage)
    {
        m_hitPoint -= t_damage;
        if (0 >= m_hitPoint)
        {
            Die();
        }
        else if (m_hitMessage)
        {
            C_CanvasAlwaysShow.instance.DisplayMessage("기체가 공격받고 있습니다.");
            m_hitMessage = false;
        }
    }


    public void Die()
    {
        C_PlayManager.instance.GameEnd(false);
    }



    /* ========== Private Methods ========== */

    /// <summary>
    /// HUD 생성
    /// </summary>
    private void CreateHUD(Color32 t_HUDColour, float t_HUDColourDarkMultiply, float t_HUDLineWidth, float t_HUDHorizonWidthMultiply, float t_HUDTextSize)
    {
        TMP_FontAsset tp_font = Resources.Load<TMP_FontAsset>("TacticalFont SDF");

        for (byte t_i = 0; t_i <= 36; ++t_i)
        {
            float t_height = 1.0f / 36.0f * t_i;

            #region 위, 아래 각도 표시용 HUD 선분
            // 좌측
            GameObject tp_gameObject = new GameObject("HUDUpDownLineImageLeft", typeof(CanvasRenderer), typeof(Image));
            tp_gameObject.transform.SetParent(mp_HUDUpDown);

            RectTransform tp_rectTransform = tp_gameObject.GetComponent<RectTransform>();
            tp_rectTransform.offsetMax = Vector2.zero;
            tp_rectTransform.offsetMin = Vector2.zero;
            tp_rectTransform.anchorMax = new Vector2(0.45f, t_height + t_HUDLineWidth);
            tp_rectTransform.anchorMin = new Vector2(0.1f, t_height - t_HUDLineWidth);
            tp_rectTransform.localScale = Vector3.one;

            Image t_image = tp_gameObject.GetComponent<Image>();

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
                        tp_rectTransform.anchorMax = new Vector2(0.45f, 1.0f / 36.0f * t_i + t_HUDLineWidth * t_HUDHorizonWidthMultiply);
                        tp_rectTransform.anchorMin = new Vector2(0.0f, 1.0f / 36.0f * t_i - t_HUDLineWidth * t_HUDHorizonWidthMultiply);
                        break;
                }
            }

            // 우측
            tp_gameObject = new GameObject("HUDUpDownLineImageRight", typeof(CanvasRenderer), typeof(Image));
            tp_gameObject.transform.SetParent(mp_HUDUpDown);

            tp_rectTransform = tp_gameObject.GetComponent<RectTransform>();
            tp_rectTransform.offsetMax = Vector2.zero;
            tp_rectTransform.offsetMin = Vector2.zero;
            tp_rectTransform.anchorMax = new Vector2(0.9f, t_height + t_HUDLineWidth);
            tp_rectTransform.anchorMin = new Vector2(0.55f, t_height - t_HUDLineWidth);
            tp_rectTransform.localScale = Vector3.one;

            t_image = tp_gameObject.GetComponent<Image>();

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
                        tp_rectTransform.anchorMax = new Vector2(1.0f, 1.0f / 36.0f * t_i + t_HUDLineWidth * t_HUDHorizonWidthMultiply);
                        tp_rectTransform.anchorMin = new Vector2(0.55f, 1.0f / 36.0f * t_i - t_HUDLineWidth * t_HUDHorizonWidthMultiply);
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
            tp_gameObject = new GameObject("HUDUpDownTextLeft", typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            tp_gameObject.transform.SetParent(mp_HUDUpDown);

            tp_rectTransform = tp_gameObject.GetComponent<RectTransform>();
            tp_rectTransform.offsetMax = Vector2.zero;
            tp_rectTransform.offsetMin = Vector2.zero;
            tp_rectTransform.anchorMax = new Vector2(0.09f, t_height + t_HUDTextSize);
            tp_rectTransform.anchorMin = new Vector2(0.0f, t_height - t_HUDTextSize);
            tp_rectTransform.localScale = Vector3.one;

            TextMeshProUGUI tp_text = tp_gameObject.GetComponent<TextMeshProUGUI>();
            tp_text.text = ((t_i - 18) * 5).ToString();
            tp_text.enableAutoSizing = true;
            tp_text.font = tp_font;
            tp_text.fontSizeMax = 100.0f;
            tp_text.fontSizeMin = 0.0f;
            tp_text.alignment = TextAlignmentOptions.Right;

            if (18 > t_i)
            {
                tp_text.color = new Color32(
                    (byte)(t_HUDColour.r * t_HUDColourDarkMultiply),
                    (byte)(t_HUDColour.g * t_HUDColourDarkMultiply),
                    (byte)(t_HUDColour.b * t_HUDColourDarkMultiply),
                    t_HUDColour.a
                );
            }
            else
            {
                tp_text.color = t_HUDColour;
            }

            // 우측
            tp_gameObject = new GameObject("HUDUpDownTextRight", typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            tp_gameObject.transform.SetParent(mp_HUDUpDown);

            tp_rectTransform = tp_gameObject.GetComponent<RectTransform>();
            tp_rectTransform.offsetMax = Vector2.zero;
            tp_rectTransform.offsetMin = Vector2.zero;
            tp_rectTransform.anchorMax = new Vector2(1.0f, t_height + t_HUDTextSize);
            tp_rectTransform.anchorMin = new Vector2(0.91f, t_height - t_HUDTextSize);
            tp_rectTransform.localScale = Vector3.one;

            tp_text = tp_gameObject.GetComponent<TextMeshProUGUI>();
            tp_text.text = ((t_i - 18) * 5).ToString();
            tp_text.enableAutoSizing = true;
            tp_text.font = tp_font;
            tp_text.fontSizeMax = 100.0f;
            tp_text.fontSizeMin = 0.0f;
            tp_text.alignment = TextAlignmentOptions.Left;

            if (18 > t_i)
            {
                tp_text.color = new Color32(
                    (byte)(t_HUDColour.r * t_HUDColourDarkMultiply),
                    (byte)(t_HUDColour.g * t_HUDColourDarkMultiply),
                    (byte)(t_HUDColour.b * t_HUDColourDarkMultiply),
                    t_HUDColour.a
                );
            }
            else
            {
                tp_text.color = t_HUDColour;
            }
            #endregion
        }
    }


    /// <summary>
    /// FixedUpdate 중에서 제일 먼저 동작할 것
    /// </summary>
    private void EarlyFixedUpdate()
    {
        // 다형성
        mp_state[(int)m_currentState].StateFixedUpdate();
    }


    private void Awake()
    {
        // 항공기 설정 가져온다.
        C_AirplaneSettings tp_settings = Resources.Load<C_AirplaneSettings>("AirplaneSettings");
        m_maxEnginePower = tp_settings.m_maxEnginePower;
        m_minEnginePower = tp_settings.m_minEnginePower;
        m_hitPoint = tp_settings.m_maxHitPoint;
        m_minEngineSoundSpeed = tp_settings.m_minEngineSoundSpeed;
        m_engineSoundSpeedMult = tp_settings.m_engineSoundSpeedMult;
#if PLATFORM_STANDALONE_WIN
        m_powerMovement = tp_settings.m_powerMovement;
#endif

        // 엔진 출력 HUD에 전달
        mp_powerSlider.minValue = m_minEnginePower;
        mp_powerSlider.maxValue = m_maxEnginePower;
        mp_powerSlider.value = m_power;

        // 애니메이터 가져온다.
        Animator tp_animator = GetComponent<Animator>();

        // 상태 클래스 생성
        mp_state = new C_AirPlaneStateBase[(int)E_FlightStates.END];
        mp_state[(int)E_FlightStates.HOVER] = new C_StateHover(this, tp_settings, tp_animator);
        mp_state[(int)E_FlightStates.FLIGHT] = new C_StateFlight(this, tp_settings, tp_animator);

        // 상태 실행
        SetState(E_FlightStates.HOVER);

        // 항공기 메타리얼 복사, 인덱스 0은 외곽선 메타리얼, 인덱스 1은 주 메타리얼
        MeshRenderer[] tp_renderers = GetComponentsInChildren<MeshRenderer>();
        mp_materials[0] = new Material(tp_renderers[0].materials[0]);
        mp_materials[1] = new Material(tp_renderers[0].materials[1]);

        // 복사한 메타리얼 붙여넣기
        for (byte t_i = 0; t_i < tp_renderers.Length; ++t_i)
        {
            tp_renderers[t_i].materials = mp_materials;
        }

        // HUD 크기 화면에 맞춤
        float t_FOV = 1.0f / Camera.main.fieldOfView;
        mp_HUDUpDown.offsetMax = new Vector2(
            mp_HUDUpDown.offsetMax.x,
            Screen.height * t_FOV * 90.0f
        );
        mp_HUDUpDown.offsetMin = new Vector2(
            mp_HUDUpDown.offsetMin.x,
            -Screen.height * t_FOV * 90.0f
        );

        // HUD 초기화
        CreateHUD(
            tp_settings.m_HUDColour,
            tp_settings.m_HUDColourDarkMultiply,
            tp_settings.m_HUDLineWidth,
            tp_settings.m_HUDHorizonWidthMultiply,
            tp_settings.m_HUDTextSize
        );
    }


    private void Start()
    {
        // 대리자 등록
        C_PlayManager.instance.earlyFixedUpdate += EarlyFixedUpdate;
    }


    // 상태에 영향받지 않는 Update 동작
    private void Update()
    {
        // 은폐 메타리얼 값 전달
        if (0 < (C_Constants.STEALTH_ANIMATION & m_stealthActive))
        {
            if (0 < (C_Constants.STEALTH_ENABLE & m_stealthActive))
            {
                m_stealth += Time.deltaTime;
                if (1.0f < m_stealth)
                {
                    m_stealth = 1.0f;
                    m_stealthActive ^= C_Constants.STEALTH_ANIMATION;
                }
            }
            else
            {
                m_stealth -= Time.deltaTime;
                if (0.0f > m_stealth)
                {
                    m_stealth = 0.0f;
                    m_stealthActive ^= C_Constants.STEALTH_ANIMATION;
                }
            }

            // 메타리얼 값 전달, 인덱스 0은 외곽선 메타리얼, 인덱스 1은 주 메타리얼
            mp_materials[0].SetFloat("_DissolveAmount", m_stealth);
            mp_materials[1].SetFloat("_DissolveAmount", m_stealth);
        }
    }


    // 상태에 영향받지 않는 FixedUpdate 동작
    private void FixedUpdate()
    {
        // 엔진 출력 전달
        if (m_curPower != m_power)
        {
            m_curPower = m_power;
            mp_state[(int)m_currentState].power = m_power;
        }

        // 지면 뚫기 방지
        if (0.0f > transform.localPosition.y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, 0.0f, transform.localPosition.z);
        }
    }
}
