using UnityEngine;
using UnityEngine.UI;

public class C_CanvasActorHUD : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private GameObject mp_conquestingDisplay = null;
    [SerializeField] private GameObject mp_buttonConquest = null;
    [SerializeField] private GameObject mp_buttonAttack = null;
    [SerializeField] private Image mp_hitPointBar = null;
    [SerializeField] private Image mp_conquestBar = null;
    [SerializeField] private Image mp_actorPortrait = null;
    [SerializeField] private C_Joystick mp_joystick = null;
    [SerializeField] private RectTransform mp_enemyPointer = null;
    private GameObject mp_enemyPointerObject = null;
    private float m_pixelPerRotation = Screen.height / (Mathf.PI* 0.25f);
    private float m_enemyPointerLimit = Screen.height * Screen.height * 0.25f;
    private bool m_enemyPointerEnabled = false;
#if PLATFORM_STANDALONE_WIN
    private float m_currentHeight = Screen.height;
#endif

    public static C_CanvasActorHUD instance
    {
        get;
        private set;
    }

    public C_Actor actor
    {
        private get;
        set;
    }



    /* ========== Public Methods ========== */

    public void CanvasEnable(bool t_enable)
    {
        gameObject.SetActive(t_enable);
    }


    public void ConquestButtonEnable(bool t_enable)
    {
        mp_buttonConquest.SetActive(t_enable);
    }


    public void SetHitPointBar(float t_amount)
    {
        mp_hitPointBar.fillAmount = t_amount;
    }


    public void ConquestDisplayEnable(bool  t_enable)
    {
        mp_conquestingDisplay.SetActive(t_enable);
    }


    public void SetConquestBar(float t_amount)
    {
        mp_conquestBar.fillAmount = t_amount;
    }


    /// <summary>
    /// HUD에 Actor 정보 전달
    /// </summary>
    public void SetActor(C_Actor tp_actor, float t_hitPointAmount, Sprite tp_portrait)
    {
        actor = tp_actor;
        mp_hitPointBar.fillAmount = t_hitPointAmount;
        mp_actorPortrait.sprite = tp_portrait;
        mp_buttonConquest.SetActive(false);
        mp_conquestingDisplay.SetActive(false);
        gameObject.SetActive(true);
    }


    public void ButtonConquest(bool t_active)
    {
        actor.ButtonConquest(t_active);
    }


    public void ButtonAttack()
    {
        // 처음 위치, 나중 위치
        Vector3 t_start;
        Vector3 t_goal;
        actor.GetTargetEnemy(out t_start, out t_goal);

        // 투척 물건
        GameObject tp_bullet = C_ObjectPool.instance.GetObject(E_ObjectPool.ACTORBULLET);

        // 값 전달
        C_ActorBullet tp_actBull = tp_bullet.GetComponent<C_ActorBullet>();
        tp_actBull.startPosition = t_start;
        tp_actBull.goalPosition = t_goal;

        // 활성화
        tp_bullet.SetActive(true);
    }


    public void ButtonAeroplane()
    {
        actor.ButtonAeroplane();
    }


    public C_Joystick GetUIJoystick()
    {
        return mp_joystick;
    }


    public void DisableEnemyPointer()
    {
        mp_enemyPointerObject.SetActive(false);
        mp_buttonAttack.SetActive(false);
        m_enemyPointerEnabled = false;
    }


    public void SetEnemyPointerPosition(Vector2 t_pos)
    {
        t_pos *= m_pixelPerRotation;
        if (m_enemyPointerLimit < t_pos.x * t_pos.x + t_pos.y * t_pos.y)
        {
            DisableEnemyPointer();
            return;
        }

        mp_enemyPointer.localPosition = t_pos;
        if (m_enemyPointerEnabled)
        {
            return;
        }
        mp_enemyPointerObject.SetActive(true);
        mp_buttonAttack.SetActive(true);
    }



    /* ========== Private Methods ========== */

    private void Awake()
    {
        // 유니티식 싱글턴패턴
        instance = this;

        // 참조
        mp_enemyPointerObject = mp_enemyPointer.gameObject;

        // 비활성화로 시작
        gameObject.SetActive(false);
    }


    #if PLATFORM_STANDALONE_WIN
    private void Update()
    {
        if (Screen.height != m_currentHeight)
        {
            m_currentHeight = Screen.height;
            m_pixelPerRotation = m_currentHeight / (Mathf.PI * 0.25f);
            m_enemyPointerLimit = m_currentHeight * m_currentHeight * 0.25f;
        }
    }
    #endif
}
