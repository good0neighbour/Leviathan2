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

    public static C_ObjectPool instance
    {
        get;
        private set;
    }



    /* ========== Public Methods ========== */

    public GameObject GetObject(E_ObjectPool t_objectType)
    {
        switch (mp_objectStacks[(int)t_objectType].Count)
        {
            case 0:
                switch (t_objectType)
                {
                    case E_ObjectPool.ATTACKENEMY_LANDFORCE:
                        return Instantiate(mp_landForceattackEnemyPrefab);
                        
                    case E_ObjectPool.ATTACKENEMY_OCEANFORCE:
                        return Instantiate(mp_oceanForceattackEnemyPrefab);

                    case E_ObjectPool.ALLYMINION:
                        return Instantiate(mp_allyPrefab);

                    case E_ObjectPool.ACTORBULLET:
                        return Instantiate(mp_actorBulletPrefab);

                    case E_ObjectPool.EXPLOSION:
                        return Instantiate(mp_explosionPrefab);
                }
                return null;

            default:
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
