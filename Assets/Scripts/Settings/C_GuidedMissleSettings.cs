using UnityEngine;

[CreateAssetMenu(fileName = "GuidedMissleSettings", menuName = "Leviathan/GuidedMissleSettings")]
public class C_GuidedMissleSettings : ScriptableObject
{
    [Header("Browsing")]
    public float m_cameraRotateSpeed = 30.0f;
    [Range(0.0f, 1.0f)] public float m_movingCircleLerpWeight = 0.1f;
    [Space(10.0f)]
    [Header("Launching")]
    [Space(10.0f)]
    public float m_missleAccelerator = 100.0f;
    public float m_damageRange = 5.0f;
    public byte m_damage = 200;
    [Header("General")]
    public float m_noiseSpeedmult = 123.456f;
    [Range(0.0f, 1.0f)] public float m_minNoiseAlpha = 0.2f;
    public float m_noiseAlphaSpeed = 2.0f;
    public float m_angleLimit = 20.0f;
    [Space(10.0f)]
    [Header("PostProcess")]
    public Color m_colourFilter = Color.white;
    public float m_saturation = -50.0f;
    public float m_CAIntensity = 0.5f;
}
