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
        public string m_name;
        public Sprite mp_actorImageFull;
        public Sprite mp_actorImageThin;
        public Sprite mp_actorPortrait;
        public GameObject mp_prefab;
    }
}
