using UnityEngine;

[CreateAssetMenu(fileName = "AirplaneSettings", menuName = "Leviathan/AirplaneSettings")]
public class C_AirplaneSettings : ScriptableObject
{
    [Header("HoverMode")]
#if PLATFORM_STANDALONE_WIN
    public float m_hoverRotateSpeedmult = 4.0f;
#endif
    public float m_hoverRotatePower = 20.0f;
    public float m_hoverRotateRestorePower = 0.5f;
    [Space(10)]
    [Header("FlightMode")]
    public Vector3 m_flightRotatePower = new Vector3(0.5f, 0.1f, 0.2f);
    public float m_flightPowerMultiply = 5.0f;
#if PLATFORM_STANDALONE_WIN
    public float m_flightRotateSpeed = 4.0f;
#endif
    public float m_flightFalldownForce = 10.0f;
    public float m_liftPower = 0.005f;
    [Space(10)]
    [Header("GeneralSettings")]
    public Vector3 m_airResist = new Vector3(0.5f, 0.1f, 1.0f);
    public float m_maxEnginePower = 20.0f;
    public float m_minEnginePower = 0.0f;
    public float m_powerMovement = 10.0f;
    public float m_altitudeLimit = 1000.0f;
    public float m_minEngineSoundSpeed = 0.7f;
    public float m_engineSoundSpeedMult = 0.05f;
    public byte m_maxHitPoint = 255;
    [Space(10)]
    [Header("HUDSettings")]
    public Color32 m_HUDColour = new Color32(0, 255, 255, 255);
    [Range(0.0f, 1.0f)] public float m_HUDColourDarkMultiply = 0.5f;
    public float m_HUDLineWidth = 0.0004f;
    public float m_HUDTextSize = 0.005f;
    public float m_HUDHorizonWidthMultiply = 1.5f;
    public float m_powerImageLength = 0.8f;
}
