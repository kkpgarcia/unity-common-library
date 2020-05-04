using System;

namespace Common.AI.Fsm
{
    public interface FsmAction
    {
        FsmState GetOwner();

        void OnEnter();

        void OnUpdate();

        void OnFixedUpdate();

        void OnExit();
    }
}