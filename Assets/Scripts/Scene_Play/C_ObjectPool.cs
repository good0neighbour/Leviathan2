using System.Collections.Generic;
using UnityEngine;

public class C_ObjectPool : MonoBehaviour
{
    /* ========== Fields ========== */

    [SerializeField] private GameObject mp_landForceattackEnemyPrefab = null;
    [SerializeField] private GameObject mp_oceanForceattackEnemyPrefab = null;
    [SerializeField] private GameObject mp_allyPrefab = null;
    [SerializeField] private GameObject mp_actorBulletPrefab = null;
    [SerializeField] private GameObject mp_explosionPrefab = null;
    private Stack<GameObject>[] mp_objectStacks = new Stack<GameObject>[(int)E_ObjectPool.END]
    {
        new Stack<GameObject>(),
        new Stack<GameObject>(),
        new Stack<GameObject>(),
        new Stack<GameObject>(),
        new Stack<GameObject>()
    };
    private byte m_landForceNum = 0;
    private byte m_oceanForceNum = 0;
    private byte m_allyNum = 0;

    public static C_ObjectPool instance
    {
        get;
        private set;
    }



    /* ========== Public Methods ========== */

    public GameObject GetObject(E_ObjectPool t_objectType)
    {
        if (0 == mp_objectStacks[(int)t_objectType].Count)
        {
            switch (t_objectType)
            {
                case E_ObjectPool.ATTACKENEMY_LANDFORCE:
                    if (C_Constants.LANDFORCELIMIT < m_landForceNum)
                    {
                        return null;
                    }
                    ++m_landForceNum;
                    return Instantiate(mp_landForceattackEnemyPrefab);

                case E_ObjectPool.ATTACKENEMY_OCEANFORCE:
                    if (C_Constants.OCEANFORCELIMIT < m_oceanForceNum)
                    {
                        return null;
                    }
                    ++m_oceanForceNum;
                    return Instantiate(mp_oceanForceattackEnemyPrefab);

                case E_ObjectPool.ALLYMINION:
                    if (C_Constants.ALLYLIMIT < m_allyNum)
                    {
                        return null;
                    }
                    ++m_allyNum;
                    return Instantiate(mp_allyPrefab);

                case E_ObjectPool.ACTORBULLET:
                    return Instantiate(mp_actorBulletPrefab);

                case E_ObjectPool.EXPLOSION:
                    return Instantiate(mp_explosionPrefab);

                default:
                    return null;
            }
        }
        else
        {
            return mp_objectStacks[(int)t_objectType].Pop();
        }
    }


    public void ReturnObject(GameObject tp_object, E_ObjectPool t_objectType)
    {
        mp_objectStacks[(int)t_objectType].Push(tp_object);
    }



    /* ========== Private Methods ========== */

    private void Awake()
    {
        // 유니티식 싱글턴패턴
        instance = this;
    }
}
