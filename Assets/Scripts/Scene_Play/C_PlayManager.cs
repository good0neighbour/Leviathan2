using UnityEngine;

public class C_PlayManager : MonoBehaviour
{
    [SerializeField] private I_StateBase m_state = null;

    private void Awake()
    {
        m_state = FindAnyObjectByType<C_AirPlane>();
    }

    private void Update()
    {
        m_state.StateUpdate();
    }

    private void FixedUpdate()
    {
        m_state.StateFixedUpdate();
    }
}
