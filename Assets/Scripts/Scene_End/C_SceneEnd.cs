using UnityEngine;

public class C_SceneEnd : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private GameObject mp_winText = null;
    [SerializeField] private GameObject mp_loseText = null;



    /* ========== Private Methods ========== */

    private void Awake()
    {
        if (C_GameManager.instance.gameWin)
        {
            mp_winText.SetActive(true);
        }
        else
        {
            mp_loseText.SetActive(true);
        }
    }
}
