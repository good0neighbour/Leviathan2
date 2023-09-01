using UnityEngine;
using UnityEngine.UI;

public class C_CanvasActorHUD : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private GameObject mp_conquestingDisplay = null;
    [SerializeField] private GameObject mp_buttonConquest = null;
    [SerializeField] private Image mp_hitPointBar = null;
    [SerializeField] private Image mp_conquestBar = null;
    [SerializeField] private Image mp_actorPortrait = null;
    [SerializeField] private C_Joystick mp_joystick = null;

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


    public void ButtonAeroplane()
    {
        actor.ButtonAeroplane();
    }


    public C_Joystick GetUIJoystick()
    {
        return mp_joystick;
    }



    /* ========== Private Methods ========== */

    private void Awake()
    {
        // 유니티식 싱글턴패턴
        instance = this;

        // 비활성화로 시작
        gameObject.SetActive(false);
    }
}
