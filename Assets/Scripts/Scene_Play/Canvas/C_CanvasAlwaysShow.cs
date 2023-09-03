using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class C_CanvasAlwaysShow : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private Image mp_playerBaseHitPointImage = null;
    [SerializeField] private TextMeshProUGUI mp_landForceNum = null;
    [SerializeField] private TextMeshProUGUI mp_oceanForceNum = null;
    [SerializeField] private GameObject mp_canvasMenu = null;

    public static C_CanvasAlwaysShow instance
    {
        get;
        private set;
    }



    /* ========== Public Methodes ========== */

    public void SetPlayerBaseHitPointImage(float t_amount)
    {
        mp_playerBaseHitPointImage.fillAmount = t_amount;
    }


    public void SetEnemyBaseNum(byte t_land, byte t_ocean)
    {
        mp_landForceNum.text = t_land.ToString();
        mp_oceanForceNum.text = t_ocean.ToString();
    }


    public void ButtonMenu()
    {
        Time.timeScale = 0;
        mp_canvasMenu.SetActive(true);
    }



    /* ========== Public Methodes ========== */

    private void Awake()
    {
        // 유니티식 싱글턴패턴
        instance = this;

        // 플레이어 기지 방어력 표시 초기화
        mp_playerBaseHitPointImage.fillAmount = 1.0f;
    }


    private void Update()
    {
        // 단축키 동작, 윈도우에서 ESC, 안드로이드에서 뒤로가기
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ButtonMenu();
        }
    }
}
