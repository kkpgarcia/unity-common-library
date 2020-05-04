using System;

namespace Common.AI.Fsm
{
    public abstract class FsmActionAdapter : FsmAction
    {
        private readonly FsmState owner;

        public FsmActionAdapter(FsmState owner)
        {
            this.owner = owner;
        }

        public FsmState GetOwner()
        {
            return owner;
        }

        public virtual void OnEnter()
        {

        }

        public virtual void OnUpdate()
        {

        }

        public virtual void OnFixedUpdate()
        {

        }

        public virtual void OnExit()
        {

        }

    }
}