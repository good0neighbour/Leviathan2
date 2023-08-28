using UnityEngine;
using UnityEngine.UI;

public class C_CanvasAlwaysShow : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private Image mp_playerBaseHitPointImage = null;

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



    /* ========== Public Methodes ========== */

    private void Awake()
    {
        // 유니티식 싱글턴패턴
        instance = this;

        // 플레이어 기지 방어력 표시 초기화
        mp_playerBaseHitPointImage.fillAmount = 1.0f;
    }
}
