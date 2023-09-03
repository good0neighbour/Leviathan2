using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class C_SceneEnd : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private GameObject[] mp_winTexts = new GameObject[2];
    [SerializeField] private GameObject[] mp_loseTexts = new GameObject[2];
    [SerializeField] private GameObject mp_returnButton = null;



    /* ========== Private Methods ========== */

    public void ButtonReturn()
    {
        SceneManager.LoadScene("Scene_Menu");
        C_GameManager.instance.SaveGameData();
    }



    /* ========== Private Methods ========== */

    private IEnumerator EndTextAnimation(GameObject[] tp_witchTexts)
    {
        float t_timer = 0.0f;

        // 승리, 패배 글자
        tp_witchTexts[0].SetActive(true);

        // 1초 후
        while (1.0f > t_timer)
        {
            t_timer += Time.deltaTime;
            yield return null;
        }

        // 승리, 패배 상황
        tp_witchTexts[1].SetActive(true);

        // 1초 후
        t_timer = 0.0f;
        while (1.0f > t_timer)
        {
            t_timer += Time.deltaTime;
            yield return null;
        }

        // 주메뉴 버튼
        mp_returnButton.SetActive(true);
    }


    private void Awake()
    {
        if (C_GameManager.instance.gameWin)
        {
            C_GameManager.instance.faith += 5;
            StartCoroutine(EndTextAnimation(mp_winTexts));
        }
        else
        {
            C_GameManager.instance.faith += 1;
            StartCoroutine(EndTextAnimation(mp_loseTexts));
        }
    }
}
