using UnityEngine;
using UnityEngine.UI;

public class C_CanvasActorHUD : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private GameObject mp_conquestingDisplay = null;
    [SerializeField] private GameObject mp_buttonConquest = null;
    [SerializeField] private Image mp_hitPointBar = null;
    [SerializeField] private Image mp_conquestBar = null;
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
        // ����Ƽ�� �̱�������
        instance = this;

        // ��Ȱ��ȭ�� ����
        gameObject.SetActive(false);
    }
}
