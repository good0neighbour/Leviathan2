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
    public Vector3 m_flightRotatePower = new Vector3(0.5f, 0.1f, 0.2f);
    public float m_flightPowerMultiply = 5.0f;
    public float m_flightRotateSpeedmult = 4.0f;
    public float m_flightFalldownForce = 10.0f;
    public float m_liftPower = 0.005f;
    [Space(10)]
    [Header("GeneralSettings")]
    public Vector3 m_airResist = new Vector3(0.5f, 0.1f, 1.0f);
    public float m_maxEnginePower = 20.0f;
    public float m_minEnginePower = 0.0f;
    public float m_powerMovement = 10.0f;
    [Space(10)]
    [Header("HUDSettings")]
    public Color32 m_HUDColour = new Color32(0, 255, 255, 255);
    [Range(0.0f, 1.0f)] public float m_HUDColourDarkMultiply = 0.5f;
    public float m_HUDLineWidth = 0.0004f;
    public float m_HUDHorizonWidthMultiply = 1.5f;
    public float m_powerImageLength = 0.8f;
}
