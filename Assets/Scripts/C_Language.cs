using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using TMPro;

/// <summary>
/// ������ �� ���� ��� ���� Ŭ����
/// </summary>
public class C_Language
{
    /// <summary>
    /// json ���Ϸκ��� �о�� �� ���� ����ü
    /// </summary>
    private struct S_JsonLanguage
    {
        public string[] mp_texts;

        /// <summary>
        /// �ʱ�ȭ�� ���⼭
        /// </summary>
        public S_JsonLanguage(bool t_initialize)
        {
            mp_texts = new string[]
            {
                "����",
                "��ȭ",
                "����",
                "�ڷ�",
                "�ൿ��� ����",
                "�ҷ����� ��",
                "�¸�",
                "�ܼ��� ħ�����κ��� ����� �ùε��� ���ѳ½��ϴ�.",
                "�й�",
                "�ܼ��� ħ�����κ��� �ùε��� ������� ���߽��ϴ�.",
                "�ָ޴�",
                "���۹� ����",
                "�÷��̾� ���� ����",
                "�������",
                "�ؾ缼��",
                "���� ��",
                "�ΰ�",
                "�޴�",
                "�簳",
                "�ž�",
                "���",
                "�̵� �ӷ�",
                "���� �ӵ�",
                "��ȣ�ۿ� ����",
                "��ȭ �� ������",
                "�� ������ �����߽��ϴ�.",
                "������ ���ݹް� �ֽ��ϴ�.",
                "������ ������ ���� �����Դϴ�.",
                "������ �ı��Ǳ� �����Դϴ�.",
                "����� ���ݹް� �ֽ��ϴ�.",
                "����� ����߽��ϴ�.",
                "��ü�� ���ݹް� �ֽ��ϴ�.",
                "��������� ��� ������ �����߽��ϴ�.",
                "�ؾ缼���� ��� ������ �����߽��ϴ�.",
                "�ұ� ������ �뱹�� �������� ����.",
                "��� ���� ȸ�����ڴ� �������Ե� ȯ������ ���Ѵ�."
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
    /// �̰��� �� �����̹Ƿ� ���� ������ ���� �������. �ѱ��� 'Ű'�� �Է��ϸ� ������ '��'�� ��ȯ�Ѵ�.
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
                Debug.LogError($"���� �ѱ��� Ű \"{mp_koreanKey}\"");
                return null;
            }
#endif
        }
    }

    /// <summary>
    /// �ε����� ���� ����
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
    /// ���ڿ��� �ε��� ��ȣ ��ȯ
    /// </summary>
    public ushort GetLanguageIndex(string mp_koreanKey)
    {
        return mp_texts[mp_koreanKey];
    }


    /// <summary>
    /// json ���� �ҷ��´�.
    /// </summary>
    public void LoadLangeage(E_LanguageType t_language)
    {
        // json ���� ���� ��´�.
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
                Debug.LogError("LoadLangeage - LanguageType ���� ���.");
#endif
                return;
        }

        // json ������ �ҷ��´�.
        m_jsonLanguage = JsonUtility.FromJson<S_JsonLanguage>(Resources.Load(tp_filename).ToString());

        // �븮��
        onLanguageChange?.Invoke();
        C_GameManager.instance.currentLanguage = t_language;
    }



    /* ==================== Private Methods ==================== */

    private C_Language()
    {
        // �ѱ���� �ʱ�ȭ
        S_JsonLanguage t_jsonLanguage = new S_JsonLanguage(true);

        // Dictionary�� �߰�
        for (ushort t_i = 0; t_i < t_jsonLanguage.mp_texts.Length; ++t_i)
        {
            try
            {
                // �ش� �ѱ�� ��� �ε����� �ִ��� ����
                mp_texts.Add(t_jsonLanguage.mp_texts[t_i], t_i);
            }
            catch
            {
#if UNITY_EDITOR
                Debug.LogError($"���� Ű�� ���� - \"{t_jsonLanguage.mp_texts[t_i]}\"");
#endif
            }
        }
    }



    /* ==================== UNITY_EDITOR ==================== */

#if UNITY_EDITOR
    /// <summary>
    /// �����ϴ� Ű���� Ȯ��
    /// </summary>
    public bool GetContainsKey(string t_koreanKey)
    {
        return mp_texts.ContainsKey(t_koreanKey);
    }


    /// <summary>
    /// �����Ϳ��� json ���� �����Ѵ�.
    /// </summary>
    public void LanguageSave()
    {
        // ����ü�� �ʱ�ȭ�Ѵ�.
        S_JsonLanguage t_jsonLanguage = new S_JsonLanguage(true);

        // json ������ �����Ѵ�.
        File.WriteAllText($"{Application.dataPath}/Resources/Korean.Json", JsonUtility.ToJson(t_jsonLanguage, true));

        // ���� ������ ���� �ؽ�Ʈ ���Ϸ� �����Ѵ�.
        TextFileForGoogleTranslate(t_jsonLanguage);
    }


    /// <summary>
    /// �ٸ� ��� json ������ �����Ѵ�.
    /// </summary>
    public void SaveOtherLanguages()
    {
        // �غ� �ܰ�
        string[] tp_path = Directory.GetFiles($"{Application.dataPath}/Translations/", "*.txt", SearchOption.AllDirectories);
        string tp_jsonForm = Resources.Load("Korean").ToString();
        List<StringBuilder> tp_words = new List<StringBuilder>();
        StringBuilder tp_result = new StringBuilder();

        // �����ϴ� ��� ������ ����
        for (byte t_i = 0; t_i < tp_path.Length; ++t_i)
        {
            // ��� �ϳ� �غ� �ܰ�
            string tp_text = File.ReadAllText(tp_path[t_i]);
            ushort t_index = 0;
            tp_words.Clear();

            // �ܾ� ����
            for (int t_j = 0; t_j < tp_text.Length; ++t_j)
            {
                if (';' == tp_text[t_j])
                {
                    // ���� �ܾ�
                    t_j += 2;
                    ++t_index;
                }
                else
                {
                    // �����迭�� �߰� �� ������ �߰�
                    if (t_index == tp_words.Count)
                    {
                        tp_words.Add(new StringBuilder());
                    }

                    // �ܾ� ���
                    tp_words[t_index].Append(tp_text[t_j]);
                }
            }

            // jsonȭ ���� �غ� �ܰ�
            int t_count = 0;
            bool t_proceed = true;
            t_index = 0;
            tp_result.Clear();

            // json ���� ���󰡱�
            for (int t_j = 0; t_j < tp_jsonForm.Length; ++t_j)
            {
                // ���
                if (t_proceed)
                {
                    tp_result.Append(tp_jsonForm[t_j]);
                }

                // ū����ǥ ����
                if ('\"' == tp_jsonForm[t_j])
                {
                    ++t_count;

                    // ū����ǥ 3�� �̻��� ��
                    if (3 <= t_count)
                    {
                        // ū����ǥ ������ ��
                        if (1 == t_count % 2)
                        {
                            // ���ο� �ܾ� ����
                            if (t_index < tp_words.Count)
                            {
                                tp_result.Append(tp_words[t_index]);
                            }

                            tp_result.Append('\"');

                            // ���� �ܾ�
                            ++t_index;

                            // ��� ����
                            t_proceed = false;
                        }
                        // ū����ǥ ������ ��
                        else
                        {
                            // ��� �簳
                            t_proceed = true;
                        }
                    }
                }
            }

            // json ���Ϸ� ����
            File.WriteAllText($"{Application.dataPath}/Resources/{Path.GetFileNameWithoutExtension(tp_path[t_i])}.Json", tp_result.ToString());
        }
    }


    /// <summary>
    /// ���� ������ ���� �ؽ�Ʈ ���Ϸ� ����
    /// </summary>
    private void TextFileForGoogleTranslate(S_JsonLanguage t_jsonLanguage)
    {
        // ���ڿ� ����
        StringBuilder tp_text = new StringBuilder();
        for (ushort i = 0; i < t_jsonLanguage.mp_texts.Length; ++i)
        {
            tp_text.Append($"{t_jsonLanguage.mp_texts[i]};\n");
        }

        // �ؽ�Ʈ ���Ϸ� ����
        File.WriteAllText($"{Application.dataPath}/TranslateThis.txt", tp_text.ToString());
    }
#endif
}
