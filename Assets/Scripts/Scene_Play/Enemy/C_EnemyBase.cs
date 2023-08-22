using UnityEngine;

public class C_EnemyBase : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] GameObject m_enemy = null;
    [SerializeField] private float m_areaRadius = 10.0f;
    [SerializeField] private byte m_numOfEnemies = 5;



    /* ========== Private Methodes ========== */

    private void Awake()
    {
        
    }
}
