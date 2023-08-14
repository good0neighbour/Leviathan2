using UnityEngine;

public class C_CameraMove : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private Transform m_target = null;
    [SerializeField] private Vector3 m_cameraPosition = Vector3.zero;
    [SerializeField] float m_lerpWeight = 0.1f;



    /* ========== Private Methods ========== */

    private void FixedUpdate()
    {
        transform.localPosition = Vector3.Lerp(
            m_target.localPosition + m_target.localRotation * m_cameraPosition,
            transform.localPosition,
            m_lerpWeight
        );

        transform.localRotation = Quaternion.Lerp(
            m_target.localRotation,
            transform.localRotation,
            m_lerpWeight
        );
    }
}
