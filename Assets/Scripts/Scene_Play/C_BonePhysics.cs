using System.Collections.Generic;
using UnityEngine;

public class C_BonePhysics : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private float m_boneLength = 0.1f;
    [SerializeField] bool m_includeRoot = false;
    private List<S_BoneInfo> mp_bones = new List<S_BoneInfo>();



    /* ========== Private Methods ========== */

    private void Awake()
    {
        if (m_includeRoot)
        {
            mp_bones.Add(new S_BoneInfo(transform, m_boneLength));
        }

        for (byte t_i = 0;  t_i < transform.childCount; ++t_i)
        {
            Transform t_child = transform.GetChild(t_i);
            mp_bones.Add(new S_BoneInfo(t_child, m_boneLength));
        }
    }


    private void FixedUpdate()
    {
        foreach (S_BoneInfo t_bone in mp_bones)
        {
            Vector3 t_gap = t_bone.m_previousEndPoint
                - t_bone.m_transform.position + t_bone.m_originalRotation * new Vector3(0.0f, m_boneLength, 0.0f);
            Vector3 t_angle = new Vector3(
                -Mathf.Atan(t_gap.z / t_gap.y) * Mathf.Rad2Deg,
                0.0f,
                Mathf.Atan(t_gap.x / t_gap.y) * Mathf.Rad2Deg
            );
            t_bone.m_transform.rotation = t_bone.m_originalRotation * Quaternion.Euler(t_angle);
            Debug.Log($"{t_angle.x}\t{t_angle.z}");
        }
    }



    /* ========== Structure ========== */

    private struct S_BoneInfo
    {
        public Transform m_transform;
        public Vector3 m_previousEndPoint;
        public Quaternion m_originalRotation;

        public S_BoneInfo(Transform t_transform, float t_boneLength)
        {
            m_transform = t_transform;
            m_originalRotation = m_transform.rotation;
            m_previousEndPoint = m_transform.position + m_originalRotation * new Vector3(0.0f, t_boneLength, 0.0f);
        }
    }
}
