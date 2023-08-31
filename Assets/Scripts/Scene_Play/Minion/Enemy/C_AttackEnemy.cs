using UnityEngine;

public class C_AttackEnemy : C_Minion
{
    /* ========== Fields ========== */

    [SerializeField] private E_ObjectPool m_enemyType = E_ObjectPool.ATTACKENEMY_LANDFORCE;



    /* ========== Public Methods ========== */

    public override void Die()
    {
        gameObject.SetActive(false);
        StopAllCoroutines();
        C_ObjectPool.instance.ReturnObject(gameObject, m_enemyType);
    }



    /* ========== Protected Methods ========== */

    protected override E_NodeStatuss BasicAction()
    {
        // �� ���� ó�� �������� Ȯ��
        switch (m_status)
        {
            case C_Constants.ENEMY_BASICACTION:
                if (!mp_agent.hasPath)
                {
                    mp_agent.SetDestination(C_PlayManager.instance.playerBasePosition);
                }
                break;

            default:
                m_status = C_Constants.ENEMY_BASICACTION;
                mp_canvas.SetActive(false);
                mp_agent.ResetPath();
                mp_agent.SetDestination(C_PlayManager.instance.playerBasePosition);
                break;
        }

        // ��ȯ
        return E_NodeStatuss.SUCCESS;
    }
}
