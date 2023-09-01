using UnityEngine;

[CreateAssetMenu(fileName = "ActorSettings", menuName = "Leviathan/ActorSettings")]
public class C_ActorSettings : ScriptableObject
{
#if PLATFORM_STANDALONE_WIN
    public float m_accelerator = 5.0f;
#endif
    public float m_cameraRotateSpeed = 20.0f;
}
