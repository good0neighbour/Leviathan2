using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class JsonCreatorEditor : EditorWindow
{
    private struct S_JsonLanguage
    {
        public string[] Texts;
    }



    /* ==================== Variables ==================== */

    static private JsonCreatorEditor mp_window = null;
    private S_JsonLanguage m_json;
    private Dictionary<string, int> mp_texts = null;
    private E_LanguageType m_language = E_LanguageType.ENGLISH;
    private string mp_status = null;
    private string mp_koreanKey = null;
    private string mp_value = null;
    private string mp_isLoadedString = null;
    private bool m_isLoaded = false;



    /* ==================== Private Methods ==================== */

    [MenuItem("Leviathan/Language Json Create")]
    private static void Open()
    {
        if (null == mp_window)
        {
            mp_window = CreateInstance<JsonCreatorEditor>();

            mp_window.position = new Rect(100.0f, 100.0f, 1000.0f, 1000.0f);
        }

        mp_window.Show();
    }


    private void OnGUI()
    {
        // json 파일 생성
        GUILayout.Label("\njson 파일 생성", EditorStyles.boldLabel);
        if (GUILayout.Button("한국어 json 및 번역용 파일 생성"))
        {
            C_Language.instance.LanguageSave();
            AssetDatabase.Refresh();
            mp_status = "한국어 json 저장됨.";
        }
        if (GUILayout.Button("다른 언어 json 생성"))
        {
            C_Language.instance.SaveOtherLanguages();
            AssetDatabase.Refresh();
            mp_status = "다른 언어 json 저장됨.";
        }

        GUILayout.Label(mp_status, EditorStyles.boldLabel);

        // 언어 테스트
        GUILayout.Label("\n언어 테스트", EditorStyles.boldLabel);

        // json 불러오기
        EditorGUILayout.LabelField("===== json 불러오기 =====");
        m_language = (E_LanguageType)EditorGUILayout.EnumFlagsField(m_language);
        if (GUILayout.Button("json 불러오기"))
        {
            m_isLoaded = LoadJson(m_language);
            if (m_isLoaded)
            {
                mp_isLoadedString = "성공";
            }
            else
            {
                mp_isLoadedString = "실패";
            }
        }
        EditorGUILayout.LabelField(mp_isLoadedString);

        // 읽어오기 성공 시에만
        if (m_isLoaded)
        {
            // 한국어 키 검색
            EditorGUILayout.LabelField("===== 한국어 키 검색 =====");
            mp_koreanKey = EditorGUILayout.TextField("한국어 키", mp_koreanKey);
            if (GUILayout.Button("검색"))
            {
                try
                {
                    mp_value = m_json.Texts[mp_texts[mp_koreanKey]];
                }
                catch
                {
                    mp_value = "존재하지 않는 키";
                }
            }

            // 값
            GUILayout.Label(mp_value, EditorStyles.boldLabel);
        }

        // 닫기 버튼
        if (GUILayout.Button("닫기"))
        {
            mp_window.Close();
        }
    }


    private bool LoadJson(E_LanguageType language)
    {
        try
        {
            // 해쉬테이블 생성한 적 없으면
            if (null == mp_texts)
            {
                // 한국어 json 읽어오기
                m_json = JsonUtility.FromJson<S_JsonLanguage>(Resources.Load("Korean").ToString());

                // 해쉬테이블 생성
                mp_texts = new Dictionary<string, int>();

                // 키, 값 등록
                for (int i = 0; i < m_json.Texts.Length; ++i)
                {
                    mp_texts.Add(m_json.Texts[i], i);
                }
            }
            
            // 파일 명 설정
            string filename;
            switch (language)
            {
                case E_LanguageType.KOREAN:
                    filename = "Korean";
                    break;
                case E_LanguageType.ENGLISH:
                    filename = "English";
                    break;
                default:
                    Debug.LogError("잘못된 언어 종류.");
                    return false;
            }

            // json 읽어오기
            m_json = JsonUtility.FromJson<S_JsonLanguage>(Resources.Load(filename).ToString());

            // 성공
            return true;
        }
        catch
        {
            // 실패
            return false;
        }
    }
}