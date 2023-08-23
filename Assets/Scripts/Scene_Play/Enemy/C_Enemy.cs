using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class C_Enemy : MonoBehaviour, I_Actor
{
    /* ========== Fields ========== */

    [SerializeField] private GameObject mp_canvas = null;
    [SerializeField] private NavMeshAgent mp_agent = null;
    [SerializeField] private TextMeshProUGUI mp_surprisedText = null;
    [SerializeField] private LineRenderer mp_lineRenderer = null;
    private C_BehaviourTree mp_behaviourTree = null;
    private Transform mp_canvasTransform = null;
    private Transform mp_cameraTransform = null;
    private Transform mp_target = null;
    private I_Actor mp_actor = null;
    private Vector3 m_basePosition = Vector3.zero;
    private float m_areaRadius = 0.0f;
    private float m_patrolUpdate = 0.0f;
    private float m_sightRange = 0.0f;
    private float m_attackRange = 0.0f;
    private float m_attackTimer = 0.0f;
    private float m_targetDistance = 0.0f;
    private float m_timer = 0.0f;
    private short m_hitPoint = 0;
    private byte m_damage = 0;
    private byte m_status = 0;



    /* ========== Public Methods ========== */

    /// <summary>
    /// Enemy 초기화
    /// </summary>
    public void EnemyInitialize(Vector3 t_basePosition, float t_areaRadius, float t_patrolUpdate, float t_sightRange, float t_attackRange, float t_attackTimer, short t_hitPoint, byte t_damage)
    {
        m_basePosition = t_basePosition;
        m_areaRadius = t_areaRadius;
        m_patrolUpdate = t_patrolUpdate;
        m_sightRange = t_sightRange;
        m_attackRange = t_attackRange;
        m_attackTimer = t_attackTimer;
        m_hitPoint = t_hitPoint;
        m_damage = t_damage;
    }


    public void Hit(byte t_damage)
    {
        m_hitPoint -= t_damage;
        if (0 >= m_hitPoint)
        {
            Die();
        }
    }


    public void Die()
    {
        Destroy(gameObject);
    }



    /* ========== Private Methods ========== */

    /// <summary>
    /// 구역 내 순찰 지점
    /// </summary>
    private Vector3 PatrolDestination()
    {
        float t_ranAngle = Random.Range(0.0f, C_Constants.DOUBLE_PI);
        float t_ranRad = Random.Range(0.0f, m_areaRadius);

        return m_basePosition + new Vector3(
            Mathf.Cos(t_ranAngle) * t_ranRad,
            0.0f,
            Mathf.Sin(t_ranAngle) * t_ranRad
        );
    }


    /// <summary>
    /// 공격 애니메이션
    /// </summary>
    private IEnumerator AttackAnimation()
    {
        float t_timer = 1.0f;
        mp_lineRenderer.SetPosition(0, transform.localPosition);
        mp_lineRenderer.SetPosition(1, mp_target.localPosition + new Vector3(0.0f, 1.0f, 0.0f));
        mp_lineRenderer.startColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        mp_lineRenderer.endColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        mp_lineRenderer.gameObject.SetActive(true);
        mp_actor.Hit(m_damage);
        while (0.0f < t_timer)
        {
            t_timer -= Time.deltaTime;
            mp_lineRenderer.startColor = new Color(1.0f, 0.0f, 0.0f, t_timer);
            mp_lineRenderer.endColor = new Color(1.0f, 0.0f, 0.0f, t_timer);
            yield return null;
        }
        mp_lineRenderer.gameObject.SetActive(false);
    }


    private void Awake()
    {
        // 참조
        mp_canvasTransform = mp_canvas.transform;
        mp_cameraTransform = Camera.main.transform;

        #region 행동트리
        mp_behaviourTree = new C_BehaviourTree();
        mp_behaviourTree
            .Selector()
                .Action(() =>
                {
                    #region Detect Enemy
                    mp_target = null;
                    foreach (Collider t_col in Physics.OverlapSphere(transform.localPosition, m_sightRange))
                    {
                        if (t_col.tag.Equals("tag_actor"))
                        {
                            mp_target = t_col.transform;
                            mp_actor = t_col.GetComponent<C_Actor>();
                            m_targetDistance = Vector3.Distance(mp_target.localPosition, transform.localPosition);
                            return C_BehaviourTree.E_NodeStatus.RUNNING;
                        }
                    }
                    return C_BehaviourTree.E_NodeStatus.RUNNING;
                    #endregion
                })
                .Sequence()
                    .Action(() =>
                    {
                        #region Enemy In Range
                        switch (mp_target)
                        {
                            case null:
                                break;

                            default:
                                if (m_attackRange > m_targetDistance)
                                {
                                    mp_agent.destination = transform.localPosition;
                                    return C_BehaviourTree.E_NodeStatus.SUCCESS;
                                }
                                break;
                        }
                        return C_BehaviourTree.E_NodeStatus.FAILURE;
                        #endregion
                    })
                    .Action(() =>
                    {
                        #region Attack
                        switch (m_status)
                        {
                            case C_Constants.ENEMY_ATTACK:
                                break;

                            default:
                                m_status = C_Constants.ENEMY_ATTACK;
                                m_timer = 0.0f;
                                mp_surprisedText.color = Color.red;
                                mp_canvas.SetActive(true);
                                break;
                        }
                        m_timer += Time.deltaTime;
                        if (m_attackTimer < m_timer)
                        {
                            StartCoroutine(AttackAnimation());
                            m_timer -= m_attackTimer;
                        }
                        mp_canvasTransform.rotation = mp_cameraTransform.localRotation;
                        return C_BehaviourTree.E_NodeStatus.SUCCESS;
                        #endregion
                    })
                .Escape()
                .Sequence()
                    .Action(() =>
                    {
                        #region Enemy In Sight
                        switch (mp_target)
                        {
                            case null:
                                break;

                            default:
                                if (m_sightRange > m_targetDistance)
                                {
                                    mp_agent.destination = mp_target.localPosition;
                                    return C_BehaviourTree.E_NodeStatus.SUCCESS;
                                }
                                break;
                        }
                        return C_BehaviourTree.E_NodeStatus.FAILURE;
                        #endregion
                    })
                    .Action(() =>
                    {
                        #region Head To Enemy
                        switch (m_status)
                        {
                            case C_Constants.ENEMY_HEAD_TO_ENEMY:
                                break;

                            default:
                                m_status = C_Constants.ENEMY_HEAD_TO_ENEMY;
                                mp_surprisedText.color = Color.white;
                                mp_canvas.SetActive(true);
                                break;
                        }
                        mp_canvasTransform.rotation = mp_cameraTransform.localRotation;
                        return C_BehaviourTree.E_NodeStatus.SUCCESS;
                        #endregion
                    })
                .Escape()
                .Action(() =>
                {
                    #region Patrol
                    switch (m_status)
                    {
                        case C_Constants.ENEMY_PATROL:
                            break;

                        default:
                            m_status = C_Constants.ENEMY_PATROL;
                            m_timer = m_patrolUpdate;
                            mp_canvas.SetActive(false);
                            break;
                    }
                    m_timer += Time.deltaTime;
                    if (m_patrolUpdate <= m_timer)
                    {
                        mp_agent.destination = PatrolDestination();
                        m_timer -= m_patrolUpdate;
                    }
                    return C_BehaviourTree.E_NodeStatus.SUCCESS;
                    #endregion
                })
            .Escape();
        #endregion
    }


    private void Update()
    {
        mp_behaviourTree.Execute();
    }
}
