using UnityEngine;

[CreateAssetMenu(fileName = "EnemySettings", menuName = "Leviathan/EnemySettings")]
public class C_EnemySettings : ScriptableObject
{
    public float m_minPatrolUpdateTime = 5.0f;
    public float m_maxPatrolUpdateTime = 10.0f;
    public float m_sightRange = 20.0f;
    public float m_attackRange = 5.0f;
    public float m_attackTimer = 3.0f;
    public short m_hitPoint = 100;
    public byte m_damage = 10;
}
