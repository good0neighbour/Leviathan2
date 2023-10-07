using UnityEngine;

public class C_Slider : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private RectTransform mp_pointer = null;
    [SerializeField] private float m_restoreSpeed = 4.0f;
    private float m_range = 0.0f;
    private float m_initialMousePosition = 0.0f;
    private bool m_pressed = false;
#if PLATFORM_STANDALONE_WIN
    private int m_currentwidth = 0;
#elif UNITY_EDITOR
#elif PLATFORM_ANDROID
    private byte m_currentMouse = 0;
#endif

    public float value
    {
        get;
        set;
    }



    /* ========== Public Methods ========== */

    public void OnTouch(bool t_active)
    {
        m_pressed = t_active;
        if (m_pressed)
        {
#if UNITY_EDITOR
            m_initialMousePosition = Input.mousePosition.x;
#elif PLATFORM_STANDALONE_WIN
            m_initialMousePosition = Input.mousePosition.x;
#elif PLATFORM_ANDROID
            m_currentMouse = (byte)(Input.touchCount - 1);
            m_initialMousePosition = Input.GetTouch(m_currentMouse).position.x;
#endif
        }
    }



    /* ========== Private Methods ========== */

    private void SliderWidth()
    {
        RectTransform t_rect = GetComponent<RectTransform>();
        m_range = Screen.width * (t_rect.anchorMax.x - t_rect.anchorMin.x) * 0.5f;
#if PLATFORM_STANDALONE_WIN
        m_currentwidth = Screen.width;
#endif
    }


    private void Awake()
    {
        SliderWidth();
    }


    private void Update()
    {
#if PLATFORM_STANDALONE_WIN
        if (m_currentwidth != Screen.width)
        {
            SliderWidth();
        }
#endif
        if (m_pressed)
        {
            // ���콺 �̵���
#if UNITY_EDITOR
            float t_mousePos = Input.mousePosition.x - m_initialMousePosition;
#elif PLATFORM_STANDALONE_WIN
            float t_mousePos = Input.mousePosition.x - m_initialMousePosition;
#elif PLATFORM_ANDROID
            float t_mousePos = Input.GetTouch(m_currentMouse).position.x - m_initialMousePosition;
#endif

            // �����ڸ� ����
            if (m_range < t_mousePos)
            {
                t_mousePos = m_range;
            }
            else if (-m_range > t_mousePos)
            {
                t_mousePos = -m_range;
            }

            // ������ �̵�
            mp_pointer.localPosition = new Vector3(t_mousePos, 0.0f, 0.0f);

            // ��
            value = t_mousePos / m_range;
        }
        else
        {
            float t_pos = value * m_range;
#if PLATFORM_STANDALONE_WIN
            // Ű����� �Է��� ���� ������ ���� ���
            if (m_range < t_pos)
            {
                t_pos = m_range;
            }
            else if (-m_range > t_pos)
            {
                t_pos = -m_range;
            }
#endif
            // ������ ��ġ
            mp_pointer.localPosition = new Vector3(t_pos, 0.0f, 0.0f);

            // ������ ����ġ
            if (0.0f < value)
            {
                value -= Time.deltaTime * m_restoreSpeed;
                if (0.0f > value)
                {
                    value = 0.0f;
                }
            }
            else if (0.0f > value)
            {
                value += Time.deltaTime * m_restoreSpeed;
                if (0.0f < value)
                {
                    value = 0.0f;
                }
            }
        }
    }


    private void OnDisable()
    {
        m_pressed = false;
        mp_pointer.localPosition = Vector3.zero;
        value = 0.0f;
    }
}
