using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class C_AirPlane : MonoBehaviour, I_State<E_PlayStates>, I_StateMachine<E_FlightStates>
{
    /* ========== Fields ========== */

    [Header("HUD ����")]
    [SerializeField] private GameObject mp_HUDCanvas = null;
    [SerializeField] private RectTransform mp_HUDUpDown = null;
    [SerializeField] private RectTransform mp_directionImage = null;
    [SerializeField] private RectTransform mp_powerImage = null;
    [SerializeField] private TextMeshProUGUI mp_velocityText = null;
    [SerializeField] private TextMeshProUGUI mp_altitudeText = null;
    private C_AirPlaneStateBase[] mp_state = null;
    private Material[] mp_materials = new Material[2];
    private E_FlightStates m_currentState = E_FlightStates.HOVER;
    private float m_power = 9.83f;
    private float m_stealth = 0.0f;
    private float m_curPower = 0.0f;
    private float m_maxEnginePower = 0.0f;
    private float m_minEnginePower = 0.0f;
    private float m_powerMovement = 0.0f;
    private float m_powerImageLength = 0.0f;
    private byte m_stealthActive = 0;
#if PLATFORM_STANDALONE_WIN
    private float m_powerImageRaito = 0.0f;
    private float m_curScreenHeight = 0.0f;
#endif



    /* ========== Public Methods ========== */

    /// <summary>
    /// ���� ����
    /// </summary>
    public void SetState(E_FlightStates t_state)
    {
        // ���� ����
        m_currentState = t_state;
    }


    /// <summary>
    /// ���� Ŭ���� ��ȯ
    /// </summary>
    public C_AirPlaneStateBase GetState(E_FlightStates t_state)
    {
        return mp_state[(int)t_state];
    }


    /// <summary>
    /// �������÷��� ��������
    /// </summary>
    public void GetHUDs(out RectTransform tp_HUDUpDown, out TextMeshProUGUI tp_velocityText, out RectTransform tp_directionImage)
    {
        tp_HUDUpDown = mp_HUDUpDown;
        tp_velocityText = mp_velocityText;
        tp_directionImage = mp_directionImage;
    }


    public void Execute()
    {
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

        #region HUD ũ�� ����
        // ȭ�� ũ�� ���� ��
        if (m_curScreenHeight != Screen.height)
        {
            m_curScreenHeight = Screen.height;
            m_powerImageLength = m_powerImageRaito * m_curScreenHeight;
            EnginePowerUIUpdate();
        }
        #endregion
        #region ����
        // ���� ��� ����
        if (Input.GetKey(KeyCode.LeftShift) && m_maxEnginePower > m_power)
        {
            m_power += Time.deltaTime * m_powerMovement;

            // ���� ��� ǥ��
            EnginePowerUIUpdate();
        }
        else if (Input.GetKey(KeyCode.LeftControl) && m_minEnginePower < m_power)
        {
            m_power -= Time.deltaTime * m_powerMovement;

            // ���� ��� ǥ��
            EnginePowerUIUpdate();
        }

        // ���� (�ӽ�)
        if (Input.GetKeyDown(KeyCode.C) && 0 == (m_stealthActive & 0b10))
        {
            if (1 <= (C_Constants.STEALTH_ENABLE & m_stealthActive))
            {
                m_stealthActive ^= C_Constants.STEALTH_ENABLE;
            }
            else
            {
                m_stealthActive |= C_Constants.STEALTH_ENABLE;
            }

            m_stealthActive |= C_Constants.STEALTH_ANIMATION;
        }
        // ���̵� �̻��� ȭ������ ��ȯ
        else if (Input.GetKeyDown(KeyCode.V))
        {
            ChangeState(E_PlayStates.GUIDEDMISSLE);
        }
        #endregion
#endif

        // ������
        mp_state[(int)m_currentState].StateUpdate();

        // �� ǥ��
        mp_altitudeText.text = Mathf.RoundToInt(transform.localPosition.y).ToString();
    }


    public void StateFixedUpdate()
    {
        
    }



    /* ========== Private Methods ========== */

    /// <summary>
    /// HUD ����
    /// </summary>
    private void CreateHUD(Color32 t_HUDColour, float t_HUDColourDarkMultiply, float t_HUDLineWidth, float t_HUDHorizonWidthMultiply, float t_HUDTextSize)
    {
        TMP_FontAsset tp_font = Resources.Load<TMP_FontAsset>("TacticalFont SDF");

        for (byte t_i = 0; t_i <= 36; ++t_i)
        {
            float t_height = 1.0f / 36.0f * t_i;

            #region ��, �Ʒ� ���� ǥ�ÿ� HUD ����
            // ����
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

            // ����
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

            // 0���� ���� ����
            switch (t_i)
            {
                case 18:
                    continue;
            }

            #region ���� ���� ǥ��
            // ����
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

            // ����
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
    /// ���� ��� ǥ�� ������Ʈ
    /// </summary>
    private void EnginePowerUIUpdate()
    {
        mp_powerImage.localPosition = new Vector3(mp_powerImage.localPosition.x, m_power / m_maxEnginePower * m_powerImageLength - m_powerImageLength * 0.5f, 0.0f);
    }


    /// <summary>
    /// FixedUpdate �߿��� ���� ���� ������ ��
    /// </summary>
    private void EarlyFixedUpdate()
    {
        // ������
        mp_state[(int)m_currentState].StateFixedUpdate();
    }


    private void Awake()
    {
        // �װ��� ���� �����´�.
        C_AirplaneSettings tp_settings = Resources.Load<C_AirplaneSettings>("AirplaneSettings");
        m_maxEnginePower = tp_settings.m_maxEnginePower;
        m_minEnginePower = tp_settings.m_minEnginePower;
        m_powerMovement = tp_settings.m_powerMovement;
        m_powerImageLength = tp_settings.m_powerImageLength * Screen.height;
#if PLATFORM_STANDALONE_WIN
        m_curScreenHeight = Screen.height;
        m_powerImageRaito = tp_settings.m_powerImageLength;
#endif

        // �ִϸ����� �����´�.
        Animator tp_animator = GetComponent<Animator>();

        // ���� Ŭ���� ����
        mp_state = new C_AirPlaneStateBase[(int)E_FlightStates.END];
        mp_state[(int)E_FlightStates.HOVER] = new C_StateHover(this, tp_settings, tp_animator);
        mp_state[(int)E_FlightStates.FLIGHT] = new C_StateFlight(this, tp_settings, tp_animator);

        // �װ��� ��Ÿ���� ����, �ε��� 0�� �ܰ��� ��Ÿ����, �ε��� 1�� �� ��Ÿ����
        MeshRenderer[] tp_renderers = GetComponentsInChildren<MeshRenderer>();
        mp_materials[0] = new Material(tp_renderers[0].materials[0]);
        mp_materials[1] = new Material(tp_renderers[0].materials[1]);

        // ������ ��Ÿ���� �ٿ��ֱ�
        for (byte t_i = 0; t_i < tp_renderers.Length; ++t_i)
        {
            tp_renderers[t_i].materials = mp_materials;
        }

        // HUD ũ�� ȭ�鿡 ����
        float t_FOV = 1.0f / Camera.main.fieldOfView;
        mp_HUDUpDown.offsetMax = new Vector2(
            mp_HUDUpDown.offsetMax.x,
            Screen.height * t_FOV * 90.0f
        );
        mp_HUDUpDown.offsetMin = new Vector2(
            mp_HUDUpDown.offsetMin.x,
            -Screen.height * t_FOV * 90.0f
        );

        // HUD �ʱ�ȭ
        CreateHUD(
            tp_settings.m_HUDColour,
            tp_settings.m_HUDColourDarkMultiply,
            tp_settings.m_HUDLineWidth,
            tp_settings.m_HUDHorizonWidthMultiply,
            tp_settings.m_HUDTextSize
        );
        EnginePowerUIUpdate();
    }


    private void Start()
    {
        // �븮�� ���
        C_PlayManager.instance.earlyFixedUpdate += EarlyFixedUpdate;
    }


    // ���¿� ������� �ʴ� Update ����
    private void Update()
    {
        // ���� ��Ÿ���� �� ����
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

            // ��Ÿ���� �� ����, �ε��� 0�� �ܰ��� ��Ÿ����, �ε��� 1�� �� ��Ÿ����
            mp_materials[0].SetFloat("_DissolveAmount", m_stealth);
            mp_materials[1].SetFloat("_DissolveAmount", m_stealth);
        }
    }


    // ���¿� ������� �ʴ� FixedUpdate ����
    private void FixedUpdate()
    {
        // ���� ��� ����
        if (m_curPower != m_power)
        {
            m_curPower = m_power;
            mp_state[(int)m_currentState].power = m_power;
        }

        // ���� �ձ� ����
        if (0.0f > transform.localPosition.y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, 0.0f, transform.localPosition.z);
        }
    }
}
