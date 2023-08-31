using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class C_PlayerBase : MonoBehaviour, I_Actor
{
    /* ========== Fields ========== */

    [SerializeField] private float m_minionSpawnTimer = 10.0f;
    [SerializeField] private Vector2 m_minionSpawnPoint = Vector3.zero;
    [SerializeField] private byte m_maxHitPoint = 100;
    [SerializeField] private byte m_hitPointRestorePerSpawn = 10;
    private C_MinionSettings mp_settings = null;
    private float m_timer = 0.0f;
    private short m_currentHitPoint = 0;



    /* ========== Public Methodes ========== */

    public void Hit(byte t_damage)
    {
        m_currentHitPoint -= t_damage;
        if (0 >= m_currentHitPoint)
        {
            Die();
        }
        C_CanvasAlwaysShow.instance.SetPlayerBaseHitPointImage((float)m_currentHitPoint / m_maxHitPoint);
    }


    public void Die()
    {
        C_GameManager.instance.gameWin = false;
        SceneManager.LoadScene("Scene_End");
    }



    /* ========== Private Methodes ========== */

    private void Awake()
    {
        // 공격용 적 생성 위치 계산
        m_minionSpawnPoint.x += transform.localPosition.x;
        m_minionSpawnPoint.y += transform.localPosition.z;

        // 채력 초기화
        m_currentHitPoint = m_maxHitPoint;
    }


    private void Start()
    {
        // 설정 가져온다.
        mp_settings = C_PlayManager.instance.GetEnemySettings();
    }


    private void Update()
    {
        m_timer += Time.deltaTime;
        if (m_minionSpawnTimer <= m_timer)
        {
            // 아군 생성
            m_timer -= m_minionSpawnTimer;
            GameObject tp_ally = C_ObjectPool.instance.GetObject(E_ObjectPool.ALLYMINION);
            tp_ally.transform.localPosition = new Vector3(
                m_minionSpawnPoint.x,
                0.0f,
                m_minionSpawnPoint.y
            );
            tp_ally.GetComponent<C_AllyMinion>().MinionInitialize(mp_settings);
            tp_ally.GetComponent<NavMeshAgent>().enabled = true;
            tp_ally.SetActive(true);

            // 기지 방어력 회복
            m_currentHitPoint += m_hitPointRestorePerSpawn;
            if (m_maxHitPoint < m_currentHitPoint)
            {
                m_currentHitPoint = m_maxHitPoint;
                C_CanvasAlwaysShow.instance.SetPlayerBaseHitPointImage((float)m_currentHitPoint / m_maxHitPoint);
            }
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color32(200, 0, 255, 255);
        if (Application.isPlaying)
        {
            Gizmos.DrawSphere(new Vector3(
                m_minionSpawnPoint.x,
                0.0f,
                m_minionSpawnPoint.y
            ), 1.0f);
        }
        else
        {
            Gizmos.DrawSphere(new Vector3(
                transform.localPosition.x + m_minionSpawnPoint.x,
                0.0f,
                transform.localPosition.z + m_minionSpawnPoint.y
            ), 1.0f);
        }
    }
#endif
}
