using System.Collections.Generic;
using UnityEngine;

public delegate E_NodeStatuss D_ActionDelegate();

public class C_BehaviourTree
{
    /* ========== Fields ========== */

    private C_Node mp_root = null;
    private Stack<C_Node> mp_nodes = new Stack<C_Node>();



    /* ========== Public Methods ========== */

    /// <summary>
    /// BehaviourTree ����
    /// </summary>
    public void Execute()
    {
        mp_root.Execute();
    }


    /// <summary>
    /// SelectorNode ����
    /// </summary>
    public C_BehaviourTree Selector()
    {
        C_Node t_node = new C_NodeSelector();
        switch (mp_nodes.Count)
        {
            case 0:
                mp_root = t_node;
                break;

            default:
                mp_nodes.Peek().AddChild(t_node);
                break;
        }
        mp_nodes.Push(t_node);
        return this;
    }


    /// <summary>
    /// SequenceNode ����
    /// </summary>
    public C_BehaviourTree Sequence()
    {
        C_Node t_node = new C_NodeSequence();
        switch (mp_nodes.Count)
        {
            case 0:
                mp_root = t_node;
                break;

            default:
                mp_nodes.Peek().AddChild(t_node);
                break;
        }
        mp_nodes.Push(t_node);
        return this;
    }


    /// <summary>
    /// ActionNode ����
    /// </summary>
    public C_BehaviourTree Action(D_ActionDelegate t_action)
    {
        C_Node t_node = new C_NodeAction(t_action);
        switch (mp_nodes.Count)
        {
            case 0:
                mp_root = t_node;
                break;

            default:
                mp_nodes.Peek().AddChild(t_node);
                break;
        }
        return this;
    }


    public  C_BehaviourTree Escape()
    {
        switch (mp_nodes.Count)
        {
            case 0:
                mp_nodes = null;
                return null;

            default:
                mp_nodes.Pop();
                return this;
        }
    }



    /* ========== Enum ========== */

    



    /* ========== Classes ========== */

    private abstract class C_Node
    {
        public abstract E_NodeStatuss Execute();
        public abstract void AddChild(C_Node t_child);
    }


    /// <summary>
    /// �ڽ� ��尡 ���� ��ȯ�� ������ ����
    /// </summary>
    private class C_NodeSelector : C_Node
    {
        public List<C_Node> mp_nodes = new List<C_Node>();

        public override E_NodeStatuss Execute()
        {
            foreach (C_Node t_node in mp_nodes)
            {
                switch (t_node.Execute())
                {
                    case E_NodeStatuss.SUCCESS:
                        return E_NodeStatuss.SUCCESS;

                    default:
                        continue;
                }
            }

            return E_NodeStatuss.FAILURE;
        }

        public override void AddChild(C_Node t_child)
        {
            mp_nodes.Add(t_child);
        }
    }


    /// <summary>
    /// �ڽ� ��尡 ������ ��ȯ�� ������ ����
    /// </summary>
    private class C_NodeSequence : C_Node
    {
        public List<C_Node> mp_nodes = new List<C_Node>();

        public override E_NodeStatuss Execute()
        {
            foreach (C_Node t_node in mp_nodes)
            {
                switch (t_node.Execute())
                {
                    case E_NodeStatuss.FAILURE:
                        return E_NodeStatuss.FAILURE;

                    default:
                        continue;
                }
            }

            return E_NodeStatuss.SUCCESS;
        }

        public override void AddChild(C_Node t_child)
        {
            mp_nodes.Add(t_child);
        }
    }


    private class C_NodeAction : C_Node
    {
        public D_ActionDelegate mp_action;

        public C_NodeAction(D_ActionDelegate t_action)
        {
            mp_action = t_action;
        }

        public override E_NodeStatuss Execute()
        {
            return mp_action.Invoke();
        }

        public override void AddChild(C_Node t_child)
        {
#if UNITY_EDITOR
            Debug.LogError("Action ��忡 �ڽ� ��� ���� �õ�.");
#endif
        }
    }
}
