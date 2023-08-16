using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class C_AirPlane : MonoBehaviour, I_StateBase
{
    /* ========== Fields ========== */

    [SerializeField][Range(0.0f, 100.0f)] private float m_power = 9.8f;
    [SerializeField][Range(0.0f, 1.0f)] private float m_stealth = 0.0f;
    [Header("HUD ����")]
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
        return m_state[(int)t_state];
    }


    /// <summary>
    /// �������÷��� ��������
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

        #region HUD ũ�� ����
        // ȭ�� ũ�� ���� ��
        if (m_currentScreenHeight != Screen.height)
        {
            m_currentScreenHeight = Screen.height;
            m_powerImageLength = m_powerImageRaito * m_currentScreenHeight;
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

        // ������
        m_state[(int)m_currentState].StateUpdate();

        // �� ǥ��
        m_altitudeText.text = Mathf.RoundToInt(transform.localPosition.y).ToString();
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
        for (byte t_i = 0; t_i <= 36; ++t_i)
        {
            float t_height = 1.0f / 36.0f * t_i;

            #region ��, �Ʒ� ���� ǥ�ÿ� HUD ����
            // ����
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

            // ����
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

            // 0���� ���� ����
            switch (t_i)
            {
                case 18:
                    continue;
            }

            #region ���� ���� ǥ��
            // ����
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

            // ����
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
    /// ���� ��� ǥ�� ������Ʈ
    /// </summary>
    private void EnginePowerUIUpdate()
    {
        m_powerImage.localPosition = new Vector3(m_powerImage.localPosition.x, m_power / m_maxEnginePower * m_powerImageLength - m_powerImageLength * 0.5f, 0.0f);
    }


    private void Awake()
    {
        // �װ��� ���� �����´�.
        C_AirplaneSettings t_settings = Resources.Load<C_AirplaneSettings>("AirplaneSettings");
        m_maxEnginePower = t_settings.m_maxEnginePower;
        m_minEnginePower = t_settings.m_minEnginePower;
        m_powerMovement = t_settings.m_powerMovement;
        m_powerImageLength = t_settings.m_powerImageLength * Screen.height;
#if PLATFORM_STANDALONE_WIN
        m_currentScreenHeight = Screen.height;
        m_powerImageRaito = t_settings.m_powerImageLength;
#endif

        // �ִϸ����� �����´�.
        Animator t_animator = GetComponent<Animator>();

        // ���� Ŭ���� ����
        m_state = new C_AirPlaneStateBase[(int)E_FlightStates.END];
        m_state[(int)E_FlightStates.HOVER] = new C_StateHover(this, t_settings, t_animator);
        m_state[(int)E_FlightStates.FLIGHT] = new C_StateFlight(this, t_settings, t_animator);

        // �װ��� ��Ÿ���� ����, �ε��� 0�� �ܰ��� ��Ÿ����, �ε��� 1�� �� ��Ÿ����
        MeshRenderer[] t_renderers = GetComponentsInChildren<MeshRenderer>();
        m_materials[0] = new Material(t_renderers[0].materials[0]);
        m_materials[1] = new Material(t_renderers[0].materials[1]);

        // ������ ��Ÿ���� �ٿ��ֱ�
        for (byte t_i = 0; t_i < t_renderers.Length; ++t_i)
        {
            t_renderers[t_i].materials = m_materials;
        }

        // HUD ũ�� ȭ�鿡 ����
        float t_FOV = 1.0f / Camera.main.fieldOfView;
        m_HUDUpDown.offsetMax = new Vector2(
            m_HUDUpDown.offsetMax.x,
            Screen.height * t_FOV * 90.0f
        );
        m_HUDUpDown.offsetMin = new Vector2(
            m_HUDUpDown.offsetMin.x,
            -Screen.height * t_FOV * 90.0f
        );

        // HUD �ʱ�ȭ
        CreateHUD(
            t_settings.m_HUDColour,
            t_settings.m_HUDColourDarkMultiply,
            t_settings.m_HUDLineWidth,
            t_settings.m_HUDHorizonWidthMultiply,
            t_settings.m_HUDTextSize
        );
        EnginePowerUIUpdate();
    }


    // ���¿� ������� �ʴ� Update ����
    private void Update()
    {
        // ���� ��Ÿ���� �� ����
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

            // ��Ÿ���� �� ����, �ε��� 0�� �ܰ��� ��Ÿ����, �ε��� 1�� �� ��Ÿ����
            m_materials[0].SetFloat("_DissolveAmount", m_stealth);
            m_materials[1].SetFloat("_DissolveAmount", m_stealth);
        }
    }


    // ���¿� ������� �ʴ� FixedUpdate ����
    private void FixedUpdate()
    {
        // ���� ��� ����
        if (m_currentPower != m_power)
        {
            m_currentPower = m_power;
            m_state[(int)m_currentState].power = m_power;
        }

        // ������
        m_state[(int)m_currentState].StateFixedUpdate();

        // ���� �ձ� ����
        if (0.0f > transform.localPosition.y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, 0.0f, transform.localPosition.z);
        }
    }
}
