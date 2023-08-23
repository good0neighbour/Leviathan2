using UnityEngine;

public class C_EnemyBase : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private GameObject m_enemy = null;
    [SerializeField] private float m_areaRadius = 10.0f;
    [SerializeField] private byte m_numOfEnemies = 5;
    private float m_minPatrolUpdateTime = 0.0f;
    private float m_maxPatrolUpdateTime = 0.0f;



    /* ========== Private Methodes ========== */

    private void Awake()
    {
        // 설정 가져온다.
        C_EnemySettings t_settings = Resources.Load<C_EnemySettings>("EnemySettings");
        m_minPatrolUpdateTime = t_settings.m_minPatrolUpdateTime;
        m_maxPatrolUpdateTime = t_settings.m_maxPatrolUpdateTime;

        // 적 생성
        for (byte t_i = 0; t_i < m_numOfEnemies; ++t_i)
        {
            // 적 생성
            Transform t_enemy = Instantiate(m_enemy).transform;

            // 적 위치 배정
            float t_ranAngle = Random.Range(0.0f, C_Constants.DOUBLE_PI);
            float t_ranRad = Random.Range(0.0f, m_areaRadius);
            t_enemy.localPosition = transform.localPosition + new Vector3(
                Mathf.Cos(t_ranAngle) * t_ranRad,
                0.0f,
                Mathf.Sin(t_ranAngle) * t_ranRad
            );

            // 적 초기화
            t_enemy.GetComponent<C_Enemy>().EnemyInitialize(
                transform.localPosition,
                m_areaRadius,
                Random.Range(m_minPatrolUpdateTime, m_maxPatrolUpdateTime),
                t_settings.m_sightRange,
                t_settings.m_attackRange,
                t_settings.m_attackTimer,
                t_settings.m_hitPoint,
                t_settings.m_damage
            );
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.localPosition, m_areaRadius);
    }
#endif
}
