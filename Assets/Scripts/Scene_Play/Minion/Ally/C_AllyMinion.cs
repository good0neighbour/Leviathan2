using UnityEngine;

public class C_AllyMinion : C_Minion
{
    /* ========== Public Methods ========== */

    public override void Die()
    {
        gameObject.SetActive(false);
        C_ObjectPool.instance.ReturnObject(gameObject, E_ObjectPool.ALLYMINION);
    }



    /* ========== Protected Methods ========== */

    protected override E_NodeStatuss BasicAction()
    {
        // 이 상태 처음 실행인지 확인
        switch (m_status)
        {
            case C_Constants.ENEMY_BASICACTION:
                if (!mp_agent.hasPath)
                {
                    mp_agent.SetDestination(C_PlayManager.instance.RandomAllyDestination());
                }
                break;

            default:
                m_status = C_Constants.ENEMY_BASICACTION;
                mp_canvas.SetActive(false);
                mp_agent.ResetPath();
                mp_agent.SetDestination(C_PlayManager.instance.RandomAllyDestination());
                break;
        }

        // 반환
        return E_NodeStatuss.SUCCESS;
    }
}
