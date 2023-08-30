using UnityEngine;

public class C_Joystick : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private RectTransform mp_pointer = null;
    [SerializeField] private float m_restoreSpeed = 4.0f;
    [SerializeField] private ushort m_pointingRadius = 50;
    public Vector2 value = Vector2.zero;
    private Vector3 m_initialMousePosition = Vector2.zero;
    private ushort m_radiusPower = 0;
    private bool m_pressed = false;



    /* ========== Public Methods ========== */

    public void OnTouch(bool t_active)
    {
        m_pressed = t_active;
        if (m_pressed)
        {
            m_initialMousePosition = Input.mousePosition;
        }
    }


    public float GetScalarValue()
    {
        return Mathf.Sqrt(value.sqrMagnitude);
    }



    /* ========== Private Methods ========== */

    private void Awake()
    {
        m_radiusPower = (ushort)(m_pointingRadius * m_pointingRadius);
    }


    private void Update()
    {
        if (m_pressed)
        {
            // ���콺 �̵���
            Vector2 t_mousePos = (Vector2)(Input.mousePosition - m_initialMousePosition);

            // �����ڸ� ����
            float t_dis = t_mousePos.x * t_mousePos.x + t_mousePos.y * t_mousePos.y;
            if (t_dis > m_radiusPower)
            {
                value = t_mousePos / Mathf.Sqrt(t_dis);
                t_mousePos = value * m_pointingRadius;
            }

            // ������ �̵�
            mp_pointer.localPosition = t_mousePos;

            // ��
            value = t_mousePos / m_pointingRadius;
        }
        else
        {
#if PLATFORM_STANDALONE_WIN
            // Ű����� �Է��� ���� ������ ���� ���
            float t_dis = value.x * value.x + value.y * value.y;
            if (1.0f < t_dis)
            {
                value = value / Mathf.Sqrt(t_dis);
            }
#endif
            // ������ ��ġ
            mp_pointer.localPosition = value * m_pointingRadius;

            // ������ ����ġ
            float t_angle = Mathf.Abs(Mathf.Atan(value.y / value.x));
            if (0.0f < value.x)
            {
                value.x -= Mathf.Cos(t_angle) *  Time.deltaTime * m_restoreSpeed;
                if (0.0f > value.x)
                {
                    value.x = 0.0f;
                }
            }
            else if (0.0f > value.x)
            {
                value.x += Mathf.Cos(t_angle) * Time.deltaTime * m_restoreSpeed;
                if (0.0f < value.x)
                {
                    value.x = 0.0f;
                }
            }
            if (0.0f < value.y)
            {
                value.y -= Mathf.Sin(t_angle) * Time.deltaTime * m_restoreSpeed;
                if (0.0f > value.y)
                {
                    value.y = 0.0f;
                }
            }
            else if (0.0f > value.y)
            {
                value.y += Mathf.Sin(t_angle) * Time.deltaTime * m_restoreSpeed;
                if (0.0f < value.y)
                {
                    value.y = 0.0f;
                }
            }
        }
    }


    private void OnDisable()
    {
        m_pressed = false;
        mp_pointer.localPosition = Vector3.zero;
        value = Vector2.zero;
    }
}
