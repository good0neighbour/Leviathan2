using UnityEngine;

public abstract class C_GuidedMissleStateBase : I_State<E_GuidedMissleStates>
{
    /* ========== Fields ========== */

    protected Transform mp_transform = null;



    /* ========== Public Methods ========== */

    public C_GuidedMissleStateBase(Transform t_transform)
    {
        mp_transform = t_transform;
    }


    public abstract void ChangeState(E_GuidedMissleStates t_state);


    public abstract void Execute();


    public abstract void StateFixedUpdate();


    public abstract void StateUpdate();
}
