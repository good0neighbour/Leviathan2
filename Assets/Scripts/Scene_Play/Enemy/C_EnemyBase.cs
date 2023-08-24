using UnityEngine;

public class C_EnemyBase : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private GameObject mp_enemy = null;
    [SerializeField] private float m_areaRadius = 10.0f;
    [SerializeField] private byte m_numOfEnemies = 5;
    private C_Enemy[] mp_enemyArray = null;
    private E_PlayState m_state = E_PlayState.AIRPLANE;
    private float m_minPatrolUpdateTime = 0.0f;
    private float m_maxPatrolUpdateTime = 0.0f;



    /* ========== Private Methodes ========== */

    /// <summary>
    /// PlayState ���� ��
    /// </summary>
    private void OnStateChange()
    {
        m_state = C_PlayManager.instance.currentState;
    }


    private void Awake()
    {
        // ���� �����´�.
        C_EnemySettings t_settings = Resources.Load<C_EnemySettings>("EnemySettings");
        m_minPatrolUpdateTime = t_settings.m_minPatrolUpdateTime;
        m_maxPatrolUpdateTime = t_settings.m_maxPatrolUpdateTime;

        // �迭 ����
        mp_enemyArray = new C_Enemy[m_numOfEnemies];

        // �� ����
        for (byte t_i = 0; t_i < m_numOfEnemies; ++t_i)
        {
            // �� ����
            Transform t_enemyTrans = Instantiate(mp_enemy).transform;

            // �� ��ġ ����
            float t_ranAngle = Random.Range(0.0f, C_Constants.DOUBLE_PI);
            float t_ranRad = Random.Range(0.0f, m_areaRadius);
            t_enemyTrans.localPosition = transform.localPosition + new Vector3(
                Mathf.Cos(t_ranAngle) * t_ranRad,
                0.0f,
                Mathf.Sin(t_ranAngle) * t_ranRad
            );

            // �� �ʱ�ȭ
            C_Enemy t_enemy = t_enemyTrans.GetComponent<C_Enemy>();
            t_enemy.EnemyInitialize(
                transform.localPosition,
                m_areaRadius,
                Random.Range(m_minPatrolUpdateTime, m_maxPatrolUpdateTime),
                t_settings.m_sightRange,
                t_settings.m_attackRange,
                t_settings.m_attackTimer,
                t_settings.m_hitPoint,
                t_settings.m_damage
            );

            // �迭�� �߰�
            mp_enemyArray[t_i] = t_enemy;
        }

        // �븮�� ���
        C_PlayManager.instance.onStateChange += OnStateChange;
    }


    private void Update()
    {
        switch (m_state)
        {
            case E_PlayState.GUIDEDMISSLE:

                return;

            default:
                return;
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.localPosition, m_areaRadius);
    }
#endif
}
