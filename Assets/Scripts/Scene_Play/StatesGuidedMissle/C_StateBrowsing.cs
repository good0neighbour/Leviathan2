using UnityEngine;

public class C_StateBrowsing : C_GuidedMissleStateBase
{
    /* ========== Fields ========== */

    private Transform mp_attachedTarget = null;



    /* ========== Public Methods ========== */

    public C_StateBrowsing(Transform t_transform, Transform tp_attachedTarget) : base(t_transform)
    {
        mp_attachedTarget = tp_attachedTarget;
    }


    public override void ChangeState(E_GuidedMissleStates t_state)
    {
        
    }


    public override void Execute()
    {
        
    }


    public override void StateFixedUpdate()
    {
        
    }


    public override void StateUpdate()
    {
        
    }
}
