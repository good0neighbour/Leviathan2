using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ActorInfomation", menuName = "Leviathan/ActorInfomation")]
public class C_ActorInfomation : ScriptableObject
{
    public S_Info[] mp_actorInformation = null;



    [Serializable]
    public class S_Info
    {
        [Header("¿œπ›")]
        public string m_name = null;
        public Sprite mp_actorImageFull = null;
        public Sprite mp_actorImageThin = null;
        public Sprite mp_actorPortrait = null;
        public GameObject mp_prefab = null;
        public float m_interactRange = 1.2f;
        public float m_conquestSpeed = 0.2f;
        public float m_maxSpeed = 4.0f;
        public short m_hitPoint = 5;
    }
}
