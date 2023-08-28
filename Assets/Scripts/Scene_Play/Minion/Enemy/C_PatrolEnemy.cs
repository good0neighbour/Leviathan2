using UnityEngine;

public class C_PatrolEnemy : C_Minion
{
    /* ========== Fields ========== */

    [Header("자식 클래스")]
    private Vector3 m_basePosition = Vector3.zero;
    private float m_areaRadius = 0.0f;
    private float m_patrolUpdate = 0.0f;



    /* ========== Public Methods ========== */

    /// <summary>
    /// Enemy 초기화
    /// </summary>
    public void PatrolEnemyInitialize(C_MinionSettings tp_settings, Vector3 t_basePosition, float t_areaRadius, float t_patrolUpdate)
    {
        MinionInitialize(tp_settings);
        m_basePosition = t_basePosition;
        m_areaRadius = t_areaRadius;
        m_patrolUpdate = t_patrolUpdate;
    }


    public override void Die()
    {
        Destroy(gameObject);
    }



    /* ========== Protected Methods ========== */

    protected override E_NodeStatuss BasicAction()
    {
        // 이 상태 처음 실행인지 확인
        switch (m_status)
        {
            case C_Constants.ENEMY_BASICACTION:
                break;

            default:
                m_status = C_Constants.ENEMY_BASICACTION;
                m_timer = m_patrolUpdate;
                mp_canvas.SetActive(false);
                break;
        }

        // 카메라 근처에 있을 때만 실행
        if (m_isNearCamera)
        {
            // 주변 순찰
            m_timer += Time.deltaTime;
            if (m_patrolUpdate <= m_timer)
            {
                mp_agent.ResetPath();
                mp_agent.SetDestination(PatrolDestination());
                m_timer -= m_patrolUpdate;
            }
        }

        // 반환
        return E_NodeStatuss.SUCCESS;
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
}
