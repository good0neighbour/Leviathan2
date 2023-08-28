using UnityEngine;
using UnityEngine.AI;

public class C_EnemyBase : MonoBehaviour
{
    /* ========== Fields ========== */

    [Header("적 생성")]
    [SerializeField] private GameObject mp_patrolEnemy = null;
    [SerializeField] private E_ObjectPool m_enemyType = E_ObjectPool.ATTACKENEMY_LANDFORCE;
    [SerializeField] private float m_attackEnemyTimer = 10.0f;
    [SerializeField] private float m_areaRadius = 10.0f;
    [SerializeField] private byte m_numOfPatrolEnemies = 5;
    [Header("장치")]
    [SerializeField] private GameObject mp_device = null;
    [SerializeField] private MeshRenderer mp_flag = null;
    [SerializeField] private Material mp_conquestedMaterial = null;
    [SerializeField] private Vector2 m_attackEnemySpawnPosint = Vector3.zero;
    private C_MinionSettings mp_settings = null;
    private float m_minPatrolUpdateTime = 0.0f;
    private float m_maxPatrolUpdateTime = 0.0f;
    private float m_timer = 0.0f;
    private bool m_enemyControl = true;



    /* ========== Private Methodes ========== */

    public void BaseConquested()
    {
        mp_device.tag = "Untagged";
        mp_flag.material = mp_conquestedMaterial;
        m_enemyControl = false;
    }



    /* ========== Private Methodes ========== */

    private void Awake()
    {
        // 공격용 적 생성 위치 계산
        m_attackEnemySpawnPosint.x += transform.localPosition.x;
        m_attackEnemySpawnPosint.y += transform.localPosition.z;
    }


    private void Start()
    {
        // 설정 가져온다.
        mp_settings = C_PlayManager.instance.GetEnemySettings();
        m_minPatrolUpdateTime = mp_settings.m_minPatrolUpdateTime;
        m_maxPatrolUpdateTime = mp_settings.m_maxPatrolUpdateTime;

        // 적 생성
        for (byte t_i = 0; t_i < m_numOfPatrolEnemies; ++t_i)
        {
            // 적 생성
            Transform t_enemyTrans = Instantiate(mp_patrolEnemy).transform;

            // 적 위치 배정
            float t_ranAngle = Random.Range(0.0f, C_Constants.DOUBLE_PI);
            float t_ranRad = Random.Range(0.0f, m_areaRadius);
            t_enemyTrans.localPosition = transform.localPosition + new Vector3(
                Mathf.Cos(t_ranAngle) * t_ranRad,
                0.0f,
                Mathf.Sin(t_ranAngle) * t_ranRad
            );

            // 적 초기화
            t_enemyTrans.GetComponent<C_PatrolEnemy>().PatrolEnemyInitialize(
                mp_settings,
                transform.localPosition,
                m_areaRadius,
                Random.Range(m_minPatrolUpdateTime, m_maxPatrolUpdateTime)
            );

            t_enemyTrans.GetComponent<NavMeshAgent>().enabled = true;
        }
    }


    private void Update()
    {
        m_timer += Time.deltaTime;
        if (m_attackEnemyTimer <= m_timer)
        {
            m_timer -= m_attackEnemyTimer;
            if (m_enemyControl)
            {
                // 적 생성
                GameObject tp_enemy = C_ObjectPool.instance.GetObject(m_enemyType);
                tp_enemy.transform.localPosition = new Vector3(
                    m_attackEnemySpawnPosint.x,
                    0.0f,
                    m_attackEnemySpawnPosint.y
                );
                tp_enemy.GetComponent<C_AttackEnemy>().AttackEnemyInitialize(mp_settings);
                tp_enemy.GetComponent<NavMeshAgent>().enabled = true;
                tp_enemy.SetActive(true);
            }
            else
            {
                // 아군 생성
                GameObject tp_ally = C_ObjectPool.instance.GetObject(E_ObjectPool.ALLYMINION);
                tp_ally.transform.localPosition = new Vector3(
                    m_attackEnemySpawnPosint.x,
                    0.0f,
                    m_attackEnemySpawnPosint.y
                );
                tp_ally.GetComponent<C_AllyMinion>().AllyMinionInitialize(mp_settings);
                tp_ally.GetComponent<NavMeshAgent>().enabled = true;
                tp_ally.SetActive(true);
            }
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.localPosition, m_areaRadius);

        Gizmos.color = Color.red;
        if (Application.isPlaying)
        {
            Gizmos.DrawSphere(new Vector3(
                m_attackEnemySpawnPosint.x,
                0.0f,
                m_attackEnemySpawnPosint.y
            ), 1.0f);
        }
        else
        {
            Gizmos.DrawSphere(new Vector3(
                transform.localPosition.x + m_attackEnemySpawnPosint.x,
                0.0f,
                transform.localPosition.z + m_attackEnemySpawnPosint.y
            ), 1.0f);
        }
    }
#endif
}
