using UnityEngine;

[CreateAssetMenu(fileName = "AirplaneSettings", menuName = "Leviathan/AirplaneSettings")]
public class C_AirplaneSettings : ScriptableObject
{
    [Header("HoverMode")]
    public float m_hoverRotateSpeedmult = 4.0f;
    public float m_hoverRotatePower = 20.0f;
    public float m_hoverRotateRestorePower = 0.5f;
    [Space(10)]
    [Header("FlightMode")]
    public float m_flightRotateSpeedmult = 4.0f;
    public Vector3 m_flightRotatePower = new Vector3(0.5f, 0.1f, 0.2f);
    [Space(10)]
    [Header("GeneralSettings")]
    public Vector3 m_airResist = new Vector3(0.5f, 0.1f, 1.0f);
    public float m_liftPower = 0.01f;
    public float m_maxEnginePower = 20.0f;
    public float m_minEnginePower = 0.0f;
}
