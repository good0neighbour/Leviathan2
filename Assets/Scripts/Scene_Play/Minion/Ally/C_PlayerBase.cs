using UnityEngine;
using UnityEngine.AI;

public class C_PlayerBase : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private float m_minionSpawnTimer = 10.0f;
    [SerializeField] private Vector2 m_minionSpawnPoint = Vector3.zero;
    private C_MinionSettings mp_settings = null;
    private float m_timer = 0.0f;



    /* ========== Private Methodes ========== */

    private void Awake()
    {
        // 공격용 적 생성 위치 계산
        m_minionSpawnPoint.x += transform.localPosition.x;
        m_minionSpawnPoint.y += transform.localPosition.z;
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
            tp_ally.GetComponent<C_AllyMinion>().AllyMinionInitialize(mp_settings);
            tp_ally.GetComponent<NavMeshAgent>().enabled = true;
            tp_ally.SetActive(true);
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
