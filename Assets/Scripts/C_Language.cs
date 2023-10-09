using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using TMPro;

/// <summary>
/// 번역된 언어를 위한 언어 관리 클래스
/// </summary>
public class C_Language
{
    /// <summary>
    /// json 파일로부터 읽어온 언어를 담을 구조체
    /// </summary>
    private struct S_JsonLanguage
    {
        public string[] mp_texts;

        /// <summary>
        /// 초기화는 여기서
        /// </summary>
        public S_JsonLanguage(bool t_initialize)
        {
            mp_texts = new string[]
            {
                "시작",
                "강화",
                "종료",
                "뒤로",
                "행동요원 선택",
                "불러오는 중",
                "승리",
                "외세의 침략으로부터 당신의 시민들을 지켜냈습니다.",
                "패배",
                "외세의 침략으로부터 시민들을 방어하지 못했습니다.",
                "주메뉴",
                "조작법 설명",
                "플레이어 기지 방어력",
                "대륙세력",
                "해양세력",
                "점령 중",
                "부관",
                "메뉴",
                "재개",
                "신앙",
                "방어",
                "이동 속력",
                "점령 속도",
                "상호작용 범위",
                "강화 시 증가량",
                "적 거점을 점령했습니다.",
                "기지가 공격받고 있습니다.",
                "기지의 방어력이 절반 이하입니다.",
                "기지가 파괴되기 직전입니다.",
                "요원이 공격받고 있습니다.",
                "요원이 사망했습니다.",
                "기체가 공격받고 있습니다.",
                "대륙세력의 모든 거점을 점령했습니다.",
                "해양세력의 모든 거점을 점령했습니다.",
                "소국 주제에 대국에 대항하지 마라.",
                "당신 같은 회색분자는 누구에게도 환영받지 못한다."
            };
        }
    }



    /* ==================== Variables ==================== */

    private static C_Language mp_instance = null;

    private S_JsonLanguage m_jsonLanguage;
    private Dictionary<string, ushort> mp_texts = new Dictionary<string, ushort>();

    public static C_Language instance
    {
        get
        {
            if (null == mp_instance)
            {
                mp_instance = new C_Language();
            }

            return mp_instance;
        }
    }

    public D_PlayDelegate onLanguageChange
    {
        get;
        set;
    }

    /// <summary>
    /// 이것이 주 목적이므로 편리한 접근을 위해 만들었다. 한국어 '키'를 입력하면 번역된 '값'을 반환한다.
    /// </summary>
    public string this[string mp_koreanKey]
    {
        get
        {
#if UNITY_EDITOR
            if (mp_texts.ContainsKey(mp_koreanKey))
            {
                try
                {
#endif
                    return m_jsonLanguage.mp_texts[mp_texts[mp_koreanKey]];
#if UNITY_EDITOR
                }
                catch
                {
                    Debug.LogError(mp_koreanKey);
                    return null;
                }
            }
            else
            {
                Debug.LogError($"없는 한국어 키 \"{mp_koreanKey}\"");
                return null;
            }
#endif
        }
    }

    /// <summary>
    /// 인덱스로 직접 접근
    /// </summary>
    public string this[ushort t_index]
    {
        get
        {
            return m_jsonLanguage.mp_texts[t_index];
        }
    }



    /* ==================== Public Methods ==================== */

    /// <summary>
    /// 문자열의 인덱스 번호 반환
    /// </summary>
    public ushort GetLanguageIndex(string mp_koreanKey)
    {
        return mp_texts[mp_koreanKey];
    }


    /// <summary>
    /// json 파일 불러온다.
    /// </summary>
    public void LoadLangeage(E_LanguageType t_language)
    {
        // json 파일 명을 담는다.
        string tp_filename;
        switch (t_language)
        {
            case E_LanguageType.KOREAN:
                tp_filename = "Korean";
                break;

            case E_LanguageType.ENGLISH:
                tp_filename = "English";
                break;
            default:
#if UNITY_EDITOR
                Debug.LogError("LoadLangeage - LanguageType 수정 요망.");
#endif
                return;
        }

        // json 파일을 불러온다.
        m_jsonLanguage = JsonUtility.FromJson<S_JsonLanguage>(Resources.Load(tp_filename).ToString());

        // 대리자
        onLanguageChange?.Invoke();
        C_GameManager.instance.currentLanguage = t_language;
    }



    /* ==================== Private Methods ==================== */

    private C_Language()
    {
        // 한국어로 초기화
        S_JsonLanguage t_jsonLanguage = new S_JsonLanguage(true);

        // Dictionary에 추가
        for (ushort t_i = 0; t_i < t_jsonLanguage.mp_texts.Length; ++t_i)
        {
            try
            {
                // 해당 한국어가 어느 인덱스에 있는지 저장
                mp_texts.Add(t_jsonLanguage.mp_texts[t_i], t_i);
            }
            catch
            {
#if UNITY_EDITOR
                Debug.LogError($"같은 키가 존재 - \"{t_jsonLanguage.mp_texts[t_i]}\"");
#endif
            }
        }
    }



    /* ==================== UNITY_EDITOR ==================== */

#if UNITY_EDITOR
    /// <summary>
    /// 존재하는 키인지 확인
    /// </summary>
    public bool GetContainsKey(string t_koreanKey)
    {
        return mp_texts.ContainsKey(t_koreanKey);
    }


    /// <summary>
    /// 에디터에서 json 파일 생성한다.
    /// </summary>
    public void LanguageSave()
    {
        // 구조체를 초기화한다.
        S_JsonLanguage t_jsonLanguage = new S_JsonLanguage(true);

        // json 파일을 저장한다.
        File.WriteAllText($"{Application.dataPath}/Resources/Korean.Json", JsonUtility.ToJson(t_jsonLanguage, true));

        // 구글 번역을 위해 텍스트 파일로 저장한다.
        TextFileForGoogleTranslate(t_jsonLanguage);
    }


    /// <summary>
    /// 다른 언어 json 파일을 생성한다.
    /// </summary>
    public void SaveOtherLanguages()
    {
        // 준비 단계
        string[] tp_path = Directory.GetFiles($"{Application.dataPath}/Translations/", "*.txt", SearchOption.AllDirectories);
        string tp_jsonForm = Resources.Load("Korean").ToString();
        List<StringBuilder> tp_words = new List<StringBuilder>();
        StringBuilder tp_result = new StringBuilder();

        // 존재하는 모든 번역본 생성
        for (byte t_i = 0; t_i < tp_path.Length; ++t_i)
        {
            // 언어 하나 준비 단계
            string tp_text = File.ReadAllText(tp_path[t_i]);
            ushort t_index = 0;
            tp_words.Clear();

            // 단어 추출
            for (int t_j = 0; t_j < tp_text.Length; ++t_j)
            {
                if (';' == tp_text[t_j])
                {
                    // 다음 단어
                    t_j += 2;
                    ++t_index;
                }
                else
                {
                    // 가변배열에 추가 안 됐으면 추가
                    if (t_index == tp_words.Count)
                    {
                        tp_words.Add(new StringBuilder());
                    }

                    // 단어 기록
                    tp_words[t_index].Append(tp_text[t_j]);
                }
            }

            // json화 시작 준비 단계
            int t_count = 0;
            bool t_proceed = true;
            t_index = 0;
            tp_result.Clear();

            // json 형식 따라가기
            for (int t_j = 0; t_j < tp_jsonForm.Length; ++t_j)
            {
                // 기록
                if (t_proceed)
                {
                    tp_result.Append(tp_jsonForm[t_j]);
                }

                // 큰따옴표 세기
                if ('\"' == tp_jsonForm[t_j])
                {
                    ++t_count;

                    // 큰따옴표 3개 이상일 때
                    if (3 <= t_count)
                    {
                        // 큰따옴표 열렸을 때
                        if (1 == t_count % 2)
                        {
                            // 새로운 단어 저장
                            if (t_index < tp_words.Count)
                            {
                                tp_result.Append(tp_words[t_index]);
                            }

                            tp_result.Append('\"');

                            // 다음 단어
                            ++t_index;

                            // 기록 중지
                            t_proceed = false;
                        }
                        // 큰따옴표 닫혔을 때
                        else
                        {
                            // 기록 재개
                            t_proceed = true;
                        }
                    }
                }
            }

            // json 파일로 저장
            File.WriteAllText($"{Application.dataPath}/Resources/{Path.GetFileNameWithoutExtension(tp_path[t_i])}.Json", tp_result.ToString());
        }
    }


    /// <summary>
    /// 구글 번역을 위해 텍스트 파일로 저장
    /// </summary>
    private void TextFileForGoogleTranslate(S_JsonLanguage t_jsonLanguage)
    {
        // 문자열 생성
        StringBuilder tp_text = new StringBuilder();
        for (ushort i = 0; i < t_jsonLanguage.mp_texts.Length; ++i)
        {
            tp_text.Append($"{t_jsonLanguage.mp_texts[i]};\n");
        }

        // 텍스트 파일로 저장
        File.WriteAllText($"{Application.dataPath}/TranslateThis.txt", tp_text.ToString());
    }
#endif
}
