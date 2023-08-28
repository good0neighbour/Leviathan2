using UnityEngine;

public class C_AllyMinion : C_Minion
{
    /* ========== Public Methods ========== */

    public void AllyMinionInitialize(C_MinionSettings tp_settings)
    {
        MinionInitialize(tp_settings);
    }


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
                break;

            default:
                m_status = C_Constants.ENEMY_BASICACTION;
                mp_canvas.SetActive(false);
                mp_agent.ResetPath();
                mp_agent.SetDestination(C_PlayManager.instance.landForceBasePosition);
                break;
        }

        // 반환
        return E_NodeStatuss.SUCCESS;
    }


#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        switch (m_status)
        {
            case C_Constants.ENEMY_BASICACTION:
                Gizmos.color = new Color(1.0f, 1.0f, 0.0f, 0.2f);
                Gizmos.DrawLine(transform.localPosition, C_PlayManager.instance.landForceBasePosition);
                return;

            default:
                base.OnDrawGizmos();
                return;
        }
    }
#endif
}
