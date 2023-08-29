using UnityEngine;

public class C_MinimapResize : MonoBehaviour
{
    private RectTransform mp_rectTransform = null;
#if PLATFORM_STANDALONE_WIN
    private float m_currentHeight = 0.0f;
#endif

    private void Resize()
    {
        float t_d = 0.23323615160349854227405247813411f * Screen.height;
        mp_rectTransform.sizeDelta = new Vector2(t_d, t_d);
#if PLATFORM_STANDALONE_WIN
        m_currentHeight = Screen.height;
#endif
    }

    private void Awake()
    {
        mp_rectTransform = GetComponent<RectTransform>();
        Resize();
#if PLATFORM_ANDROID
        Destroy(this);
#endif
    }

#if PLATFORM_STANDALONE_WIN
    private void Update()
    {
        if (m_currentHeight != Screen.height)
        {
            Resize();
        }
    }
#endif
}
