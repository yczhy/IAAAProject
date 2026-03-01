using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duskvern
{
    public abstract class FsmElement
    {
        private Dictionary<E_FsmState, IFsmNode> m_fsmElementDic = new ();

        private IFsmNode m_currentNode;
        private E_FsmState m_currentState
        {
            get
            {
                return m_currentNode == null ? E_FsmState.None : m_currentNode.State;
            }
        }
        private E_FsmState m_preState;

        private IFsmBlackboard m_fsmBlackboard;
        public IFsmBlackboard FsmBlackboard => m_fsmBlackboard;

        private bool m_Warrning;

        private string Name; // 携带状态机的物体的名字

        public FsmElement Init(string name, IFsmNode firstNode, Dictionary<E_FsmState, IFsmNode> fsmElementDic, bool warrning = false)
        {
            Name = name;
            if (firstNode == null || fsmElementDic == null || !fsmElementDic.ContainsKey(firstNode.State) || fsmElementDic[firstNode.State] != firstNode)
            {
                DebugLogger.LogError($"{Name} 初始化状态机失败");
                return this;
            }
            m_fsmElementDic = fsmElementDic;
            m_Warrning = warrning;
            m_currentNode = firstNode;
            m_fsmBlackboard = null;

            foreach (var node in m_fsmElementDic.Values)
            {
                if (node == null)
                {
                    DebugLogger.LogError($"{node.State} 节点为空");
                    return this;
                }
                if (m_fsmBlackboard != null && !FsmBlackboard.Equals(node.Blackboard))
                {
                    DebugLogger.LogError($"{Name} 状态机节点黑板不一致");
                    return this;
                }
                m_fsmBlackboard = node.Blackboard;
            }

            return this;
        } 

        public void AddFsmNode(E_FsmState state, IFsmNode node)
        {
            if (node == null || m_fsmElementDic.TryGetValue(state, out _) || node.Blackboard != m_fsmBlackboard)
            {
               DebugLogger.LogWarning($"{Name} 添加 {state} 的状态机节点失败");
               return;
            }
            m_fsmElementDic[state] = node;
        }

        public void RemoveFsmNode(E_FsmState state)
        {
            if (m_fsmElementDic.TryGetValue(state, out var node))
            {
                if (m_currentNode != null && state == m_currentState)
                {
                    DebugLogger.LogWarning($"移除的状态是当前状态机节点， 先退出当前状态机节点");
                    m_currentNode.OnExit();
                    m_currentNode = null;
                }

                m_fsmElementDic.Remove(state);
                return;
            }
            DebugLogger.LogWarning($"{Name} 没有找到 {state} 的状态机节点");
        }

        public void BackFsmNode()
        {
            if (m_preState == E_FsmState.None)
            {
                DebugLogger.LogWarning($"{Name} 上一个状态机节点为空， 回退失败");
                return;
            }
            else if (m_preState != m_currentState)
            {
                SwitchFsmNode(m_preState);
            }
        }

        public void SwitchFsmNode(E_FsmState state)
        {
            if (m_currentNode == null)
            {
                DebugLogger.LogWarning($"{Name} 当前状态机节点为空");
            }
            if (m_fsmElementDic.TryGetValue(state, out var targetNode))
            {
                m_currentNode?.OnExit();
                m_preState = m_currentState;
                m_currentNode = targetNode;
                m_currentNode.OnEnter();
                if (m_Warrning) DebugLogger.LogInfo($"{Name} 切换到 {state} 状态");
                return;
            }
            DebugLogger.LogError($"{Name} 切换状态机失败：没有找到 {state} 的状态机节点");
        }

        public void OnExecute()
        {
            if (m_currentNode == null)
            {
                DebugLogger.LogError($"{Name} 当前状态机节点为空");
                return;
            }
            m_currentNode.OnExecute();
            m_currentNode.OnLastExecute();
        }
    }
}

