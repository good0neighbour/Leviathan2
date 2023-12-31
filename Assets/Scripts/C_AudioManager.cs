using UnityEngine;

public class C_AudioManager : MonoBehaviour
{
    /* ==================== Variables ==================== */

    // 생성하는 쪽에서 이미 값 할당
    private static C_AudioManager _instance = null;

    [Header("설정")]
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

    [Header("참조")]
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
                // 이미 생성돼있는 경우 새로 생성한 것을 파괴한다.
                Destroy(value.gameObject);
                Debug.LogError("이미 생성된 AudioManager.");
            }
#endif
        }
    }



    /* ==================== Public Methods ==================== */

    /// <summary>
    /// 소리 재생
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
                Debug.Log("C_AudioManager : 잘못된 AudioType");
                return;
#endif
        }
    }



    /* ==================== Private Methods ==================== */

    /// <summary>
    /// 오디오 채널 할당 후 재생
    /// </summary>
    private void UseChannel(AudioClip clip, float volume)
    {
        // 오디오 채널에 소리 등록하고 재생
        _channels[_currentChannel].clip = clip;
        _channels[_currentChannel].volume = volume;
        _channels[_currentChannel].Play();

        // 사용할 채널 인덱스 증가
        ++_currentChannel;

        // 채널 개수 초과 시 되돌아온다.
        if (_currentChannel == m_numberOfChannel)
        {
            _currentChannel = 0;
        }
    }


    private void Awake()
    {
        // 채널 배열 생성
        _channels = new AudioSource[m_numberOfChannel];

        // 1개는 기본
        _channels[0] = _audioSource;

        // 나머지는 추가 생성
        for (byte i = 1; i < m_numberOfChannel; ++i)
        {
            AudioSource aS = Instantiate(_audioSource.gameObject, transform).GetComponent<AudioSource>();
            _channels[i] = aS;
        }
    }
}
