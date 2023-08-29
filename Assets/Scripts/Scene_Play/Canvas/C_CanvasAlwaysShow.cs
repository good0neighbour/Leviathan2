using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class C_CanvasAlwaysShow : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private Image mp_playerBaseHitPointImage = null;
    [SerializeField] private TextMeshProUGUI mp_landForceNum = null;
    [SerializeField] private TextMeshProUGUI mp_oceanForceNum = null;

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



    /* ========== Public Methodes ========== */

    private void Awake()
    {
        // ����Ƽ�� �̱�������
        instance = this;

        // �÷��̾� ���� ���� ǥ�� �ʱ�ȭ
        mp_playerBaseHitPointImage.fillAmount = 1.0f;
    }
}
