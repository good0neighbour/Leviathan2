using UnityEngine;

public class C_CanvasInitialScreen : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private GameObject mp_startScreen = null;



    /* ========== Public Methods ========== */

    public void ButtonStart()
    {
        gameObject.SetActive(false);
        mp_startScreen.SetActive(true);
    }


    public void ButtonQuite()
    {
        Application.Quit();
    }
}
