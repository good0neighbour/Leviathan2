using UnityEngine;

public class C_AudioManager : MonoBehaviour
{
    /* ==================== Variables ==================== */

    // �����ϴ� �ʿ��� �̹� �� �Ҵ�
    private static C_AudioManager _instance = null;

    [Header("����")]
    [Range(1, byte.MaxValue)]
    [SerializeField] private byte m_numberOfChannel = 8;

    [Header("Touch")]
    [SerializeField] private AudioClip mp_touchClip = null;
    [SerializeField] private float m_touchVolume = 1.0f;

    [Header("Alert")]
    [SerializeField] private AudioClip mp_alertClip = null;
    [SerializeField] private float m_alertVolume = 1.0f;

    [Header("HoverMode")]
    [SerializeField] private AudioClip mp_hoverModeClip = null;
    [SerializeField] private float m_hoverModeVolume = 1.0f;

    [Header("FlightMode")]
    [SerializeField] private AudioClip mp_flightModeClip = null;
    [SerializeField] private float m_flightModeVolume = 1.0f;

    [Header("GuidedMissleTouch")]
    [SerializeField] private AudioClip mp_GMTouchClip = null;
    [SerializeField] private float m_GMTouchVolume = 1.0f;

    [Header("Dive")]
    [SerializeField] private AudioClip mp_diveClip = null;
    [SerializeField] private float m_diveVolume = 1.0f;

    [Header("Stealth")]
    [SerializeField] private AudioClip mp_stealthClip = null;
    [SerializeField] private float m_stealthVolume = 1.0f;

    [Header("Summon")]
    [SerializeField] private AudioClip mp_summonClip = null;
    [SerializeField] private float m_summonVolume = 1.0f;

    [Header("����")]
    [SerializeField] private AudioSource _audioSource = null;

    private AudioSource[] _channels = null;
    private byte _currentChannel = 0;

    public static C_AudioManager instance
    {
        get
        {
            return _instance;
        }
        set
        {
#if UNITY_EDITOR
            if (null == _instance)
            {
#endif
                _instance = value;
#if UNITY_EDITOR
            }
            else if (value != _instance)
            {
                // �̹� �������ִ� ��� ���� ������ ���� �ı��Ѵ�.
                Destroy(value.gameObject);
                Debug.LogError("�̹� ������ AudioManager.");
            }
#endif
        }
    }



    /* ==================== Public Methods ==================== */

    /// <summary>
    /// �Ҹ� ���
    /// </summary>
    public void PlayAuido(E_AudioType audio)
    {
        switch (audio)
        {
            case E_AudioType.TOUCH:
                UseChannel(mp_touchClip, m_touchVolume);
                return;

            case E_AudioType.ALERT:
                UseChannel(mp_alertClip, m_alertVolume);
                return;

            case E_AudioType.SWITCH_HOVER:
                UseChannel(mp_hoverModeClip, m_hoverModeVolume);
                return;

            case E_AudioType.SWITCH_FLIGHT:
                UseChannel(mp_flightModeClip, m_flightModeVolume);
                return;

            case E_AudioType.GUIDEDMISSILE_TOUCH:
                UseChannel(mp_GMTouchClip, m_GMTouchVolume);
                return;

            case E_AudioType.DIVE:
                UseChannel(mp_diveClip, m_diveVolume);
                return;

            case E_AudioType.STEALTH:
                UseChannel(mp_stealthClip, m_stealthVolume);
                return;

            case E_AudioType.ACTOR_SUMMON:
                UseChannel(mp_summonClip, m_summonVolume);
                return;

#if UNITY_EDITOR
            default:
                Debug.Log("C_AudioManager : �߸��� AudioType");
                return;
#endif
        }
    }



    /* ==================== Private Methods ==================== */

    /// <summary>
    /// ����� ä�� �Ҵ� �� ���
    /// </summary>
    private void UseChannel(AudioClip clip, float volume)
    {
        // ����� ä�ο� �Ҹ� ����ϰ� ���
        _channels[_currentChannel].clip = clip;
        _channels[_currentChannel].volume = volume;
        _channels[_currentChannel].Play();

        // ����� ä�� �ε��� ����
        ++_currentChannel;

        // ä�� ���� �ʰ� �� �ǵ��ƿ´�.
        if (_currentChannel == m_numberOfChannel)
        {
            _currentChannel = 0;
        }
    }


    private void Awake()
    {
        // ä�� �迭 ����
        _channels = new AudioSource[m_numberOfChannel];

        // 1���� �⺻
        _channels[0] = _audioSource;

        // �������� �߰� ����
        for (byte i = 1; i < m_numberOfChannel; ++i)
        {
            AudioSource aS = Instantiate(_audioSource.gameObject, transform).GetComponent<AudioSource>();
            _channels[i] = aS;
        }
    }
}
