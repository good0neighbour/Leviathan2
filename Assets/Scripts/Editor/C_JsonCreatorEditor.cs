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
        // json ���� ����
        GUILayout.Label("\njson ���� ����", EditorStyles.boldLabel);
        if (GUILayout.Button("�ѱ��� json �� ������ ���� ����"))
        {
            C_Language.instance.LanguageSave();
            AssetDatabase.Refresh();
            mp_status = "�ѱ��� json �����.";
        }
        if (GUILayout.Button("�ٸ� ��� json ����"))
        {
            C_Language.instance.SaveOtherLanguages();
            AssetDatabase.Refresh();
            mp_status = "�ٸ� ��� json �����.";
        }

        GUILayout.Label(mp_status, EditorStyles.boldLabel);

        // ��� �׽�Ʈ
        GUILayout.Label("\n��� �׽�Ʈ", EditorStyles.boldLabel);

        // json �ҷ�����
        EditorGUILayout.LabelField("===== json �ҷ����� =====");
        m_language = (E_LanguageType)EditorGUILayout.EnumFlagsField(m_language);
        if (GUILayout.Button("json �ҷ�����"))
        {
            m_isLoaded = LoadJson(m_language);
            if (m_isLoaded)
            {
                mp_isLoadedString = "����";
            }
            else
            {
                mp_isLoadedString = "����";
            }
        }
        EditorGUILayout.LabelField(mp_isLoadedString);

        // �о���� ���� �ÿ���
        if (m_isLoaded)
        {
            // �ѱ��� Ű �˻�
            EditorGUILayout.LabelField("===== �ѱ��� Ű �˻� =====");
            mp_koreanKey = EditorGUILayout.TextField("�ѱ��� Ű", mp_koreanKey);
            if (GUILayout.Button("�˻�"))
            {
                try
                {
                    mp_value = m_json.Texts[mp_texts[mp_koreanKey]];
                }
                catch
                {
                    mp_value = "�������� �ʴ� Ű";
                }
            }

            // ��
            GUILayout.Label(mp_value, EditorStyles.boldLabel);
        }

        // �ݱ� ��ư
        if (GUILayout.Button("�ݱ�"))
        {
            mp_window.Close();
        }
    }


    private bool LoadJson(E_LanguageType language)
    {
        try
        {
            // �ؽ����̺� ������ �� ������
            if (null == mp_texts)
            {
                // �ѱ��� json �о����
                m_json = JsonUtility.FromJson<S_JsonLanguage>(Resources.Load("Korean").ToString());

                // �ؽ����̺� ����
                mp_texts = new Dictionary<string, int>();

                // Ű, �� ���
                for (int i = 0; i < m_json.Texts.Length; ++i)
                {
                    mp_texts.Add(m_json.Texts[i], i);
                }
            }
            
            // ���� �� ����
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
                    Debug.LogError("�߸��� ��� ����.");
                    return false;
            }

            // json �о����
            m_json = JsonUtility.FromJson<S_JsonLanguage>(Resources.Load(filename).ToString());

            // ����
            return true;
        }
        catch
        {
            // ����
            return false;
        }
    }
}