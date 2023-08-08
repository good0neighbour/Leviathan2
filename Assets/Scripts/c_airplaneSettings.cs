using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AirplaneSettings", menuName = "Leviathan/AirplaneSettings")]
public class C_AirplaneSettings : ScriptableObject
{
    public float m_hoverRotateSpeedmult = 10.0f;
    public float m_hoverRotatePower = 10.0f;
    public float m_hoverRotateRestorePower = 0.5f;
    [Space(10)]
    public float m_airResist = 0.01f;
    public float m_liftPower = 0.01f;
}
