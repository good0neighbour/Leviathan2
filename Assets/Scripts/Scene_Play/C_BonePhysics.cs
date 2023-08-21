using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class C_BonePhysics : MonoBehaviour
{
    private struct S_BoneInfo
    {
        public Transform m_transform;
        public Vector3 m_previousEndPoint;

        public S_BoneInfo(Transform t_transform, Vector3 t_endPoint)
        {
            m_transform = t_transform;
            m_previousEndPoint = t_endPoint;
        }
    }

    /* ========== Fields ========== */

    [SerializeField] private float m_boneLength = 0.1f;
    [SerializeField] bool m_includeRoot = false;
    private List<S_BoneInfo> mp_bones = new List<S_BoneInfo>();



    /* ========== Private Methods ========== */

    private void Awake()
    {
        if (m_includeRoot)
        {
            mp_bones.Add(new S_BoneInfo(
                transform,
                transform.position + transform.rotation * new Vector3(0.0f, m_boneLength, 0.0f)
            ));
        }

        for (byte t_i = 0;  t_i < transform.childCount; ++t_i)
        {
            Transform t_child = transform.GetChild(t_i);
            mp_bones.Add(new S_BoneInfo(
                t_child,
                t_child.rotation * new Vector3(0.0f, m_boneLength, 0.0f)
            ));
        }
    }


    private void FixedUpdate()
    {
        foreach (S_BoneInfo t_bone in mp_bones)
        {
            
        }
    }
}
