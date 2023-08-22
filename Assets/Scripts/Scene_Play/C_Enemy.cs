using UnityEngine;

public class C_Enemy : MonoBehaviour
{
    /* ========== Fields ========== */

    C_BehaviourTree mp_behaviourTree = null;



    /* ========== Private Methods ========== */

    private void Awake()
    {
        mp_behaviourTree = new C_BehaviourTree();
        mp_behaviourTree
            .Selector()
                .Sequence()
                    .Action(() =>
                    {
                        Collider[] t_colliders = Physics.OverlapSphere(transform.localPosition, 5.0f);
                        foreach (Collider t_col in t_colliders)
                        {
                            Debug.Log($"{t_col.gameObject.layer}");
                        }
                        return C_BehaviourTree.E_NodeStatus.FAILURE;
                    })
                    .Action(() =>
                    {
                        Debug.Log("Atack");
                        return C_BehaviourTree.E_NodeStatus.SUCCESS;
                    })
                .Escape()
                .Sequence()
                    .Action(() =>
                    {
                        return C_BehaviourTree.E_NodeStatus.FAILURE;
                    })
                    .Action(() =>
                    {
                        return C_BehaviourTree.E_NodeStatus.SUCCESS;
                    })
                .Escape()
                .Action(() =>
                {
                    return C_BehaviourTree.E_NodeStatus.SUCCESS;
                })
            .Escape();
    }


    private void Update()
    {
        mp_behaviourTree.Execute();
    }
}
