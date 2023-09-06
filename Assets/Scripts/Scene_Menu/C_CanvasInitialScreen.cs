using UnityEngine;
using TMPro;

public class C_CanvasInitialScreen : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private GameObject mp_startScreen = null;
    [SerializeField] private GameObject mp_reinforceScreen = null;
    [SerializeField] private GameObject mp_audioManager = null;
    [SerializeField] private TextMeshProUGUI mp_frameText = null;



    /* ========== Public Methods ========== */

    public void ButtonStart()
    {
        // 소리 재생
        C_AudioManager.instance.PlayAuido(E_AudioType.TOUCH);

        gameObject.SetActive(false);
        mp_startScreen.SetActive(true);
    }


    public void ButtonReinforce()
    {
        // 소리 재생
        C_AudioManager.instance.PlayAuido(E_AudioType.TOUCH);

        gameObject.SetActive(false);
        mp_reinforceScreen.SetActive(true);
    }


    public void ButtonQuite()
    {
        // 소리 재생
        C_AudioManager.instance.PlayAuido(E_AudioType.TOUCH);

        Application.Quit();
    }


    public void ButtonLanguage(int t_index)
    {
        // 소리 재생
        C_AudioManager.instance.PlayAuido(E_AudioType.TOUCH);

        C_Language.instance.LoadLangeage((E_LanguageType)t_index);
        C_GameManager.instance.SaveGameData();
    }


    public void ButtonTargetFrame(int t_frameRate)
    {
        // 소리 재생
        C_AudioManager.instance.PlayAuido(E_AudioType.TOUCH);

        Application.targetFrameRate = t_frameRate;
        C_GameManager.instance.targetFrameRate = (byte)t_frameRate;
        mp_frameText.text = $"Target frame rate {t_frameRate.ToString()}";
    }



    /* ========== Private Methods ========== */

    private void Awake()
    {
        // 초당 프레임 수 조정
        Application.targetFrameRate = C_GameManager.instance.targetFrameRate;
        mp_frameText.text = $"Target frame rate {C_GameManager.instance.targetFrameRate.ToString()}";

        // AudioManager 없으면 생성
        switch (C_AudioManager.instance)
        {
            case null:
                GameObject tp_audMgr = Instantiate(mp_audioManager);
                DontDestroyOnLoad(tp_audMgr);
                C_AudioManager.instance = tp_audMgr.GetComponent<C_AudioManager>();
                break;

            default:
                break;
        }

        // 자동 번역 작동
        foreach (C_AutoTranslation tp_auTra in FindObjectsOfType<C_AutoTranslation>(true))
        {
            tp_auTra.TranslationReady();
        }

        // 언어 불러온다
        C_Language.instance.LoadLangeage(C_GameManager.instance.currentLanguage);
    }
}
