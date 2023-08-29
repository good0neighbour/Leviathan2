using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public abstract class C_Minion : MonoBehaviour, I_Actor
{
    /* ========== Fields ========== */

    [Header("부모 클래스")]
    [SerializeField] protected GameObject mp_canvas = null;
    [SerializeField] protected NavMeshAgent mp_agent = null;
    [SerializeField] private GameObject mp_renderingModel = null;
    [SerializeField] private TextMeshProUGUI mp_surprisedText = null;
    [SerializeField] private LineRenderer mp_lineRenderer = null;
    [SerializeField] private string[] mp_targetTags = new string[2];
    [SerializeField] private Color m_attackColour = Color.white;
    protected float m_timer = 0.0f;
    protected byte m_status = byte.MaxValue;
    protected bool m_isNearCamera = true;
    private C_BehaviourTree mp_behaviourTree = null;
    private Transform mp_canvasTransform = null;
    private Transform mp_cameraTransform = null;
    private Transform mp_target = null;
    private I_Actor mp_actor = null;
    private float m_sightRange = 0.0f;
    private float m_attackRange = 0.0f;
    private float m_attackTimer = 0.0f;
    private float m_targetDistance = 0.0f;
    private short m_hitPoint = 0;
    private byte m_damage = 0;



    /* ========== Public Methods ========== */

    public void Hit(byte t_damage)
    {
        m_hitPoint -= t_damage;
        if (0 >= m_hitPoint)
        {
            Die();
            mp_lineRenderer.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Enemy 초기화
    /// </summary>
    public void MinionInitialize(C_MinionSettings tp_settings)
    {
        m_sightRange = tp_settings.m_sightRange;
        m_attackRange = tp_settings.m_attackRange;
        m_attackTimer = tp_settings.m_attackTimer;
        m_hitPoint = tp_settings.m_hitPoint;
        m_damage = tp_settings.m_damage;
    }


    public abstract void Die();



    /* ========== Protected Methods ========== */


    /// <summary>
    /// 기본 행동
    /// </summary>
    protected abstract E_NodeStatuss BasicAction();



    /* ========== Private Methods ========== */

    /// <summary>
    /// 거리에 따른 비활성화
    /// </summary>
    private void DistanceFade()
    {
        if (m_isNearCamera)
        {
            if (C_Constants.DISTANCE_FADE < Vector3.Distance(transform.localPosition, Camera.main.transform.localPosition))
            {
                m_isNearCamera = false;
                mp_renderingModel.SetActive(false);
            }
        }
        else
        {
            if (C_Constants.DISTANCE_FADE > Vector3.Distance(transform.localPosition, Camera.main.transform.localPosition))
            {
                m_isNearCamera = true;
                mp_renderingModel.SetActive(true);
            }
        }
    }


    /// <summary>
    /// 공격 애니메이션
    /// </summary>
    private IEnumerator AttackAnimation()
    {
        float t_timer = 1.0f;
        switch (mp_actor)
        {
            case null:
                mp_lineRenderer.SetPosition(1, mp_target.localPosition);
                break;

            default:
                mp_lineRenderer.SetPosition(1, mp_target.localPosition + new Vector3(0.0f, 1.0f, 0.0f));
                break;
        }
        mp_lineRenderer.SetPosition(0, transform.localPosition);
        mp_lineRenderer.startColor = m_attackColour;
        mp_lineRenderer.endColor = m_attackColour;
        mp_lineRenderer.gameObject.SetActive(true);
        while (0.0f < t_timer)
        {
            t_timer -= Time.deltaTime;
            Color t_colour = new Color(m_attackColour.r, m_attackColour.g, m_attackColour.b, t_timer);
            mp_lineRenderer.startColor = t_colour;
            mp_lineRenderer.endColor = t_colour;
            yield return null;
        }
        mp_lineRenderer.gameObject.SetActive(false);
    }


    private void Awake()
    {
        // 참조
        mp_canvasTransform = mp_canvas.transform;
        mp_cameraTransform = Camera.main.transform;

        // 행동트리 생성
        #region 행동트리
        mp_behaviourTree = new C_BehaviourTree();
        mp_behaviourTree
            .Selector()
                .Action(() =>
                {
                    #region Detect Enemy
                    mp_target = null;
                    m_targetDistance = m_sightRange;
                    foreach (Collider t_col in Physics.OverlapSphere(transform.localPosition, m_sightRange))
                    {
                        foreach (string t_tag in mp_targetTags)
                        {
                            if (t_col.tag.Equals(t_tag))
                            {
                                Transform t_tran = t_col.transform;
                                float t_dis = Vector3.Distance(t_tran.localPosition, transform.localPosition);
                                if (m_targetDistance > t_dis)
                                {
                                    mp_target = t_tran;
                                    mp_actor = t_tran.GetComponent<I_Actor>();
                                    m_targetDistance = t_dis;
                                }
                                break;
                            }
                        }
                    }
                    return E_NodeStatuss.RUNNING;
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
                                    return E_NodeStatuss.SUCCESS;
                                }
                                break;
                        }
                        return E_NodeStatuss.FAILURE;
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
                                mp_agent.ResetPath();
                                m_timer = 0.0f;
                                mp_surprisedText.color = Color.red;
                                mp_canvas.SetActive(true);
                                break;
                        }
                        m_timer += Time.deltaTime;
                        if (m_attackTimer < m_timer)
                        {
                            if (m_isNearCamera)
                            {
                                StartCoroutine(AttackAnimation());
                            }
                            mp_actor?.Hit(m_damage);
                            m_timer -= m_attackTimer;
                        }
                        mp_canvasTransform.rotation = mp_cameraTransform.localRotation;
                        return E_NodeStatuss.SUCCESS;
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
                                return E_NodeStatuss.FAILURE;

                            default:
                                if (!mp_agent.hasPath)
                                {
                                    mp_agent.SetDestination(mp_target.localPosition);
                                }
                                return E_NodeStatuss.SUCCESS;
                        }
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
                                mp_agent.ResetPath();
                                mp_agent.SetDestination(mp_target.localPosition);
                                mp_surprisedText.color = Color.white;
                                mp_canvas.SetActive(true);
                                break;
                        }
                        mp_canvasTransform.rotation = mp_cameraTransform.localRotation;
                        return E_NodeStatuss.SUCCESS;
                        #endregion
                    })
                .Escape()
                .Action(
                    BasicAction
                )
            .Escape();
        #endregion
    }


    private void Update()
    {
        // 행동트리 실행
        mp_behaviourTree.Execute();

        // 거리에 따른 활성화 여부
        DistanceFade();
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (m_isNearCamera)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.localPosition, mp_agent.destination);
        }
    }
#endif
}
