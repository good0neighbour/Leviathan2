using UnityEngine;

[CreateAssetMenu(fileName = "GuidedMissleSettings", menuName = "Leviathan/GuidedMissleSettings")]
public class C_GuidedMissleSettings : ScriptableObject
{
    [Header("Browsing")]
    public float m_cameraRotateSpeedmult = 30.0f;
    [Range(0.0f, 1.0f)] public float m_movingCircleLerpWeight = 0.1f;
    [Space(10.0f)]
    [Header("PostProcess")]
    public float m_saturation = -50.0f;
    public float m_CAIntensity = 0.5f;
}
