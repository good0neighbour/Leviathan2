using UnityEngine;
using TMPro;

public class C_AutoTranslation : MonoBehaviour
{
    /* ==================== Variables ==================== */

    private string mp_koreanKey = null;
    private TMP_Text mp_text = null;



    /* ==================== Public Methods ==================== */

    /// <summary>
    /// 처음 씬 로드 시 대부분 텍스트는 비활성화 상태기 때문에 다른 클래스에서 호출할 것이다.
    /// </summary>
    public void TranslationReady()
    {
        // 컴포넌트 가져온다.
        mp_text = GetComponent<TMP_Text>();

#if UNITY_EDITOR
        // 찾을 수 없으면
        if (null == mp_text)
        {
            Debug.LogError($"AutoTranslation 잘못된 위치에 부착 - {name}");
            return;
        }
#endif

        // 언어 대리자에 등록한다.
        C_Language.instance.onLanguageChange += OnLanguageChange;

        // 이것이 한국어 키다.
        mp_koreanKey = mp_text.text;

#if UNITY_EDITOR
        // 해당 키가 존재하는지 확인
        if (!C_Language.instance.GetContainsKey(mp_koreanKey))
        {
            Debug.LogError($"\"{mp_koreanKey}\" - 존재하지 않는 키. 오타 수정 및 삭제 요망.\n{transform.parent.name}/{name}");
            C_Language.instance.onLanguageChange -= OnLanguageChange;
        }
#endif
    }



    /* ==================== Private Methods ==================== */

    /// <summary>
    /// 언어 변경 시 동작
    /// </summary>
    private void OnLanguageChange()
    {
        // 번역 가져온다.
        mp_text.text = C_Language.instance[mp_koreanKey];
    }
}
