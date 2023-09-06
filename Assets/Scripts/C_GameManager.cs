using System.IO;
using UnityEngine;

public class C_GameManager
{
    /* ========== Fields ========== */

    public static C_GameManager mp_instance = null;
    private C_ActorInfomation.S_Info[] mp_actorList = new C_ActorInfomation.S_Info[C_Constants.NUM_OF_ACTOR_LIMIT];
    private C_JsonData mp_data = null;
    private byte[] mp_actorIndexs = new byte[C_Constants.NUM_OF_ACTOR_LIMIT];

    public static C_GameManager instance
    {
        get
        {
            switch (mp_instance)
            {
                case null:
                    mp_instance = new C_GameManager();
                    return mp_instance;

                default:
                    return mp_instance;
            }
        }
    }

    public uint faith
    {
        get
        {
            return mp_data.m_faith;
        }
        set
        {
            mp_data.m_faith = value;
        }
    }

    public byte targetFrameRate
    {
        get
        {
            return mp_data.m_targetFrameRate;
        }
        set
        {
            mp_data.m_targetFrameRate = value;
        }
    }

    public E_LanguageType currentLanguage
    {
        get
        {
            return (E_LanguageType)mp_data.m_language;
        }
        set
        {
            mp_data.m_language = (byte)value;
        }
    }

    public bool gameWin
    {
        get;
        set;
    }



    /* ========== Public Methods ========== */

    public void SetActorList(C_ActorInfomation.S_Info[] tp_actorList, byte[] tp_actorIndexs)
    {
        mp_actorList = tp_actorList;
        mp_actorIndexs = tp_actorIndexs;
    }


    public C_ActorInfomation.S_Info[] GetActorList(out byte[] tp_actorIndexs)
    {
        tp_actorIndexs = new byte[3]
        {
            mp_data.mp_actorsLevel[mp_actorIndexs[0]],
            mp_data.mp_actorsLevel[mp_actorIndexs[1]],
            mp_data.mp_actorsLevel[mp_actorIndexs[2]]
        };
        return mp_actorList;
    }


    public byte GetActorLevel(byte t_index)
    {
        return mp_data.mp_actorsLevel[t_index];
    }


    public uint GetFaithRemaining()
    {
        return mp_data.m_faith;
    }


    public void IncreaseActorLevel(byte t_index)
    {
        if (0 < mp_data.m_faith && 255 > mp_data.mp_actorsLevel[t_index])
        {
            ++mp_data.mp_actorsLevel[t_index];
            --mp_data.m_faith;
            SaveGameData();
        }
    }


    public void SaveGameData()
    {
#if PLATFORM_STANDALONE_WIN
        File.WriteAllText($"{Application.dataPath}/GameData.json", JsonUtility.ToJson(mp_data, false));
#endif
#if PLATFORM_ANDROID
        File.WriteAllText($"{Application.persistentDataPath}/GameData.json", JsonUtility.ToJson(mp_data, false));
#endif
    }



    /* ========== Private Methods ========== */

    private C_GameManager()
    {
        try
        {
#if PLATFORM_STANDALONE_WIN
            mp_data = JsonUtility.FromJson<C_JsonData>(File.ReadAllText($"{Application.dataPath}/GameData.json"));
#endif
#if PLATFORM_ANDROID
            mp_data = JsonUtility.FromJson<C_JsonData>(File.ReadAllText($"{Application.persistentDataPath}/GameData.json"));
#endif
        }
        catch
        {
            mp_data = new C_JsonData();
        }
    }



    /* ========== Class ========== */

    public class C_JsonData
    {
        public byte m_language = 0;
        public byte m_targetFrameRate = 60;
        public uint m_faith = 0;
        // ³Ë³ËÇÏ°Ô »ý¼º
        public byte[] mp_actorsLevel = new byte[16];
    }
}
