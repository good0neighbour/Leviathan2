using UnityEngine;
using UnityEngine.AI;

public class C_Enemy : MonoBehaviour
{
    /* ========== Fields ========== */

    private C_BehaviourTree mp_behaviourTree = null;
    private NavMeshAgent mp_agent = null;



    /* ========== Private Methods ========== */

    private void Awake()
    {
        // 행동트리
        mp_behaviourTree = new C_BehaviourTree();
        mp_behaviourTree
            .Selector()
                .Sequence()
                    .Action(() =>
                    {
                        Collider[] t_colliders = Physics.OverlapSphere(transform.localPosition, 5.0f);
                        foreach (Collider t_col in t_colliders)
                        {
                            if (t_col.tag.Equals("tag_actor"))
                            {
                                mp_agent.destination = transform.localPosition;
                                return C_BehaviourTree.E_NodeStatus.SUCCESS;
                            }
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
                        Collider[] t_colliders = Physics.OverlapSphere(transform.localPosition, 10.0f);
                        foreach (Collider t_col in t_colliders)
                        {
                            if (t_col.tag.Equals("tag_actor"))
                            {
                                mp_agent.destination = t_col.transform.localPosition;
                                return C_BehaviourTree.E_NodeStatus.SUCCESS;
                            }
                        }
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

        // 참조
        mp_agent = GetComponent<NavMeshAgent>();
    }


    private void Update()
    {
        mp_behaviourTree.Execute();
    }
}
