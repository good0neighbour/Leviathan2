using UnityEngine;

[CreateAssetMenu(fileName = "AirplaneSettings", menuName = "Leviathan/AirplaneSettings")]
public class C_AirplaneSettings : ScriptableObject
{
    public float m_hoverRotateSpeedmult = 10.0f;
    public float m_hoverRotatePower = 10.0f;
    public float m_hoverRotateRestorePower = 0.5f;
    [Space(10)]
    public Vector3 m_airResist = new Vector3(0.005f, 0.01f, 0.001f);
    public float m_liftPower = 0.01f;
    public float m_maxEnginePower = 20.0f;
    public float m_minEnginePower = 0.0f;
}
