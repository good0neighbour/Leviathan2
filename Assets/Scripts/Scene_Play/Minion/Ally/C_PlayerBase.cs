using UnityEngine;
using UnityEngine.AI;

public class C_PlayerBase : MonoBehaviour, I_Hitable
{
    /* ========== Fields ========== */

    [SerializeField] private float m_minionSpawnTimer = 10.0f;
    [SerializeField] private Vector2 m_minionSpawnPoint = Vector3.zero;
    [SerializeField] private byte m_maxHitPoint = 100;
    [SerializeField] private byte m_hitPointRestorePerSpawn = 10;
    [Header("�̴ϸ�")]
    [SerializeField] private Transform mp_canvasTransform = null;
    private Transform mp_minimapCameraTransform = null;
    private C_MinionSettings mp_settings = null;
    private float m_timer = 0.0f;
    private short m_currentHitPoint = 0;
    private byte m_messageState = byte.MaxValue;



    /* ========== Public Methodes ========== */

    public void Hit(byte t_damage)
    {
        m_currentHitPoint -= t_damage;
        if (0 >= m_currentHitPoint)
        {
            Die();
        }
        C_CanvasAlwaysShow.instance.SetPlayerBaseHitPointImage((float)m_currentHitPoint / m_maxHitPoint);

        // �޼��� ǥ��
        if (0 < (m_messageState & C_Constants.PLAYER_HIT))
        {
            C_CanvasAlwaysShow.instance.DisplayMessage("������ ���ݹް� �ֽ��ϴ�.");
            m_messageState ^= C_Constants.PLAYER_HIT;
        }
        if (m_maxHitPoint > m_currentHitPoint * 2
            && 0 < (m_messageState & C_Constants.PLAYER_HALFHITPOINT))
        {
            C_CanvasAlwaysShow.instance.DisplayMessage("������ ������ ���� �����Դϴ�.");
            m_messageState ^= C_Constants.PLAYER_HALFHITPOINT;
        }
        if (m_maxHitPoint > m_currentHitPoint * 10
            && 0 < (m_messageState & C_Constants.PLAYER_LOWHITPOINT))
        {
            C_CanvasAlwaysShow.instance.DisplayMessage("������ �ı��Ǳ� �����Դϴ�.");
            m_messageState ^= C_Constants.PLAYER_LOWHITPOINT;
        }
    }


    public void Die()
    {
        C_PlayManager.instance.GameEnd(false);
    }



    /* ========== Private Methodes ========== */

    /// <summary>
    /// �Ʊ� ����
    /// </summary>
    private void SummonAllyMinion()
    {
        // �Ʊ� ����
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

        // ���� ���� ȸ��
        m_currentHitPoint += m_hitPointRestorePerSpawn;
        if (m_maxHitPoint < m_currentHitPoint)
        {
            m_currentHitPoint = m_maxHitPoint;
            C_CanvasAlwaysShow.instance.SetPlayerBaseHitPointImage((float)m_currentHitPoint / m_maxHitPoint);
        }

        // ���ݹ��� �޼��� ��� ����
        m_messageState ^= C_Constants.PLAYER_HIT;
    }


    private void Awake()
    {
        // ���ݿ� �� ���� ��ġ ���
        m_minionSpawnPoint.x += transform.localPosition.x;
        m_minionSpawnPoint.y += transform.localPosition.z;

        // ä�� �ʱ�ȭ
        m_currentHitPoint = m_maxHitPoint;
    }


    private void Start()
    {
        // ���� �����´�.
        mp_settings = C_PlayManager.instance.GetEnemySettings();

        // ����
        mp_minimapCameraTransform = C_MinimapCameraMove.instance.transform;
    }


    private void Update()
    {
        m_timer += Time.deltaTime;
        if (m_minionSpawnTimer <= m_timer)
        {
            // �Ʊ� ����
            SummonAllyMinion();
        }

        // �̴ϸ� ������ ǥ��
        mp_canvasTransform.rotation = Quaternion.Euler(
            90.0f,
            mp_minimapCameraTransform.localEulerAngles.y,
            0.0f
        );
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
