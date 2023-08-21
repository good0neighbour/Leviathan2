using UnityEngine;

[CreateAssetMenu(fileName = "ActorSettings", menuName = "Leviathan/ActorSettings")]
public class C_ActorSettings : ScriptableObject
{
    public float m_maxWalkSpeed = 1.0f;
    public float m_maxRunSpeed = 4.0f;
    public float m_accelerator = 5.0f;
    public float m_cameraRotateSpeed = 20.0f;
}