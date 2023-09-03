using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ActorInfomation", menuName = "Leviathan/ActorInfomation")]
public class C_ActorInfomation : ScriptableObject
{
    public S_Info[] mp_actorInformation = null;



    [Serializable]
    public class S_Info
    {
        [Header("일반")]
        public string mp_name = null;
        public Sprite mp_actorImageFull = null;
        public Sprite mp_actorImageThin = null;
        public Sprite mp_actorPortrait = null;
        public GameObject mp_prefab = null;
        public float m_interactRange = 1.2f;
        public float m_conquestSpeed = 0.2f;
        public float m_maxSpeed = 4.0f;
        public float m_hitPoint = 5.0f;
        [Header("증가율")]
        public float m_interactRangeUp = 0.1f;
        public float m_conquestSpeedUp = 0.05f;
        public float m_maxSpeedUp = 0.1f;
        public float m_hitPointUp = 0.5f;
    }
}
