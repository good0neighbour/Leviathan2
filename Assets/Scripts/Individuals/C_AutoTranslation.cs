using UnityEngine;
using TMPro;

public class C_AutoTranslation : MonoBehaviour
{
    /* ==================== Variables ==================== */

    private string mp_koreanKey = null;
    private TMP_Text mp_text = null;



    /* ==================== Public Methods ==================== */

    /// <summary>
    /// ó�� �� �ε� �� ��κ� �ؽ�Ʈ�� ��Ȱ��ȭ ���±� ������ �ٸ� Ŭ�������� ȣ���� ���̴�.
    /// </summary>
    public void TranslationReady()
    {
        // ������Ʈ �����´�.
        mp_text = GetComponent<TMP_Text>();

#if UNITY_EDITOR
        // ã�� �� ������
        if (null == mp_text)
        {
            Debug.LogError($"AutoTranslation �߸��� ��ġ�� ���� - {name}");
            return;
        }
#endif

        // ��� �븮�ڿ� ����Ѵ�.
        C_Language.instance.onLanguageChange += OnLanguageChange;

        // �̰��� �ѱ��� Ű��.
        mp_koreanKey = mp_text.text;

#if UNITY_EDITOR
        // �ش� Ű�� �����ϴ��� Ȯ��
        if (!C_Language.instance.GetContainsKey(mp_koreanKey))
        {
            Debug.LogError($"\"{mp_koreanKey}\" - �������� �ʴ� Ű. ��Ÿ ���� �� ���� ���.\n{transform.parent.name}/{name}");
            C_Language.instance.onLanguageChange -= OnLanguageChange;
        }
#endif
    }



    /* ==================== Private Methods ==================== */

    /// <summary>
    /// ��� ���� �� ����
    /// </summary>
    private void OnLanguageChange()
    {
        // ���� �����´�.
        mp_text.text = C_Language.instance[mp_koreanKey];
    }
}
