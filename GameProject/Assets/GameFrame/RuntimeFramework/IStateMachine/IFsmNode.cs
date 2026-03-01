using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duskvern
{
    /// <summary>
    /// 一个状态机节点必须要实现的功能
    /// </summary>
    public interface IFsmNode
    {
        IFsmBlackboard Blackboard { get; }

        E_FsmState State { get; }

        IFsmNode Init(FsmElement fsm, IFsmBlackboard blackboard, E_FsmState state);

        void OnEnter();

        void OnExecute();

        void OnLastExecute();

        void OnExit();
    }

    public interface IFsmBlackboard
    {

    }

    public enum E_FsmState
    {
        None = 0,
    }
}