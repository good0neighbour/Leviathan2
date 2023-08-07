using UnityEngine;

[CreateAssetMenu(fileName = "AirplaneSettings", menuName = "Leviathan/AirplaneSettings")]
public class c_airplaneSettings : ScriptableObject
{
    public float m_hoverRotateSpeedmult = 10.0f;
    public float m_hoverRotateRestoreSpeedmult = 1.0f;
    public float m_hiverMaxRotation = 30.0f;
}
