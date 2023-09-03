using UnityEngine;

public class C_CanvasInitialScreen : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private GameObject mp_startScreen = null;
    [SerializeField] private GameObject mp_reinforceScreen = null;
    [SerializeField] private GameObject mp_audioManager = null;



    /* ========== Public Methods ========== */

    public void ButtonStart()
    {
        gameObject.SetActive(false);
        mp_startScreen.SetActive(true);
    }


    public void ButtonReinforce()
    {
        gameObject.SetActive(false);
        mp_reinforceScreen.SetActive(true);
    }


    public void ButtonQuite()
    {
        Application.Quit();
    }


    public void ButtonLanguage(int t_index)
    {
        C_Language.instance.LoadLangeage((E_LanguageType)t_index);
        C_GameManager.instance.SaveGameData();
    }



    /* ========== Private Methods ========== */

    private void Awake()
    {
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
