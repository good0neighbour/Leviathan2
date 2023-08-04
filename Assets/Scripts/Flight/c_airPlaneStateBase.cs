using UnityEngine;

public abstract class c_airPlaneStateBase : c_stateBase
{
    protected Transform m_transform = null;
    protected Vector3 m_velocity = Vector3.zero;

    public float power
    {
        protected get; set;
    }
}
