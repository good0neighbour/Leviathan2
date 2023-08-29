using UnityEngine;

public class C_Joystick : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] RectTransform mp_pointer = null;
    [SerializeField] ushort m_pointingRadius = 50;
    private Vector3 m_initialMousePosition = Vector2.zero;
    private ushort m_radiusPower = 0;
    private bool m_pressed = false;

    public float value
    {
        get;
        private set;
    }



    /* ========== Public Methods ========== */

    public void OnTouch(bool t_active)
    {
        m_pressed = t_active;
        if (m_pressed)
        {
            m_initialMousePosition = Input.mousePosition;
        }
        else
        {
            mp_pointer.localPosition = Vector3.zero;
        }
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
            Vector2 t_mousePos = (Vector2)(Input.mousePosition - m_initialMousePosition);
            float t_dis = t_mousePos.x * t_mousePos.x + t_mousePos.y * t_mousePos.y;
            if (t_dis > m_radiusPower)
            {
                t_mousePos = t_mousePos.normalized * m_pointingRadius;
            }
            mp_pointer.localPosition = t_mousePos;
        }
    }


    private void OnDisable()
    {
        m_pressed = false;
        mp_pointer.localPosition = Vector3.zero;
    }
}
