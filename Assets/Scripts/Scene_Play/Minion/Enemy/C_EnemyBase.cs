using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private Vector2 m_attackEnemySpawnPosint = Vector3.zero;
    [Header("미니맵")]
    [SerializeField] private Transform mp_canvasTransform = null;
    [SerializeField] private Image mp_iconImage = null;
    private C_MinionSettings mp_settings = null;
    private Transform mp_minimapCameraTransform = null;
    private float m_minPatrolUpdateTime = 0.0f;
    private float m_maxPatrolUpdateTime = 0.0f;
    private float m_timer = 0.0f;


#if UNITY_EDITOR
    public Vector2 attackEnemySpawnPosint
    {
        get
        {
            return m_attackEnemySpawnPosint;
        }
        set
        {
            m_attackEnemySpawnPosint = value;
        }
    }
#endif



    /* ========== Private Methodes ========== */

    public void BaseConquested()
    {
        mp_device.tag = "Untagged";
        mp_flag.material = C_PlayManager.instance.GetPlayerFlagMaterial();
        mp_iconImage.sprite = C_PlayManager.instance.GetPlayerFlagSprite();
        C_PlayManager.instance.EnemyBaseConquested(m_enemyType);
        m_enemyType = E_ObjectPool.ALLYMINION;
    }



    /* ========== Private Methodes ========== */

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

        // 참조
        mp_minimapCameraTransform = C_MinimapCameraMove.instance.transform;
    }


    private void Update()
    {
        // Minion 생성
        m_timer += Time.deltaTime;
        if (m_attackEnemyTimer <= m_timer)
        {
            m_timer -= m_attackEnemyTimer;
            GameObject tp_minion = C_ObjectPool.instance.GetObject(m_enemyType);
            tp_minion.transform.localPosition = new Vector3(
                m_attackEnemySpawnPosint.x,
                0.0f,
                m_attackEnemySpawnPosint.y
            );
            tp_minion.GetComponent<C_Minion>().MinionInitialize(mp_settings);
            tp_minion.GetComponent<NavMeshAgent>().enabled = true;
            tp_minion.SetActive(true);
        }

        // 미니맵 아이콘 표시
        mp_canvasTransform.rotation = Quaternion.Euler(
            90.0f,
            mp_minimapCameraTransform.localEulerAngles.y,
            0.0f
        );
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.localPosition, m_areaRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(
            m_attackEnemySpawnPosint.x,
            0.0f,
            m_attackEnemySpawnPosint.y
        ), 1.0f);
    }
#endif
}
