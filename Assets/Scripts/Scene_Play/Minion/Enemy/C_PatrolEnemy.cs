using UnityEngine;

public class C_PatrolEnemy : C_Minion
{
    /* ========== Fields ========== */

    [Header("�ڽ� Ŭ����")]
    private Vector3 m_basePosition = Vector3.zero;
    private float m_areaRadius = 0.0f;
    private float m_patrolUpdate = 0.0f;



    /* ========== Public Methods ========== */

    /// <summary>
    /// Enemy �ʱ�ȭ
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
        // �� ���� ó�� �������� Ȯ��
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

        // ī�޶� ��ó�� ���� ���� ����
        if (m_isNearCamera)
        {
            // �ֺ� ����
            m_timer += Time.deltaTime;
            if (m_patrolUpdate <= m_timer)
            {
                mp_agent.ResetPath();
                mp_agent.SetDestination(PatrolDestination());
                m_timer -= m_patrolUpdate;
            }
        }

        // ��ȯ
        return E_NodeStatuss.SUCCESS;
    }



    /* ========== Private Methods ========== */

    /// <summary>
    /// ���� �� ���� ����
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
