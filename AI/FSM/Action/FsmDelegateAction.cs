using System;

namespace Common.AI.Fsm.Action
{
    public class FsmDelegateAction : FsmActionAdapter
    {
        public delegate void FsmActionRoutine(FsmState owner);

        private FsmActionRoutine onEnterRoutine;
        private FsmActionRoutine onUpdateRoutine;
        private FsmActionRoutine onExitRoutine;

        public FsmDelegateAction(FsmState owner, FsmActionRoutine onEnterRoutine) : this(owner, onEnterRoutine, null, null)
        {
        }

        public FsmDelegateAction(FsmState owner, FsmActionRoutine onEnterRoutine, FsmActionRoutine onUpdateRoutine, FsmActionRoutine onExitRoutine = null) : base(owner)
        {
            this.onEnterRoutine = onEnterRoutine;
            this.onUpdateRoutine = onUpdateRoutine;
            this.onExitRoutine = onExitRoutine;
        }

        public override void OnEnter()
        {
            if(onEnterRoutine != null)
            {
                onEnterRoutine(GetOwner());
            }
        }

        public override void OnUpdate()
        {
            if(onUpdateRoutine != null)
            {
                onUpdateRoutine(GetOwner());
            }
        }

        public override void OnExit()
        {
            if(onExitRoutine != null)
            {
                onExitRoutine(GetOwner());
            }
        }
    }
}