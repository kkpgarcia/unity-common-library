using System;
using UnityEngine;

using Common.AI.Fsm;
using Common.CustomTime;

namespace Common.AI.Fsm.Action
{
    public class IdleAction : FsmActionAdapter
    {
        public delegate void OnCallBack();
        private OnCallBack callBack;

        private CountdownTimer timer;
        private float duration;
        private string finishEvent;
        
        public IdleAction(FsmState owner, string timeReferenceName) : base(owner)
        {
            this.timer = new CountdownTimer(1, timeReferenceName);
        }

        public void Init(float duration, string finishEvent, OnCallBack callBack)
        {
            this.duration = duration;
            this.finishEvent = finishEvent;
            this.callBack = callBack;
        }

        public override void OnEnter()
        {
            this.timer.Reset(this.duration);
        }

        public override void OnUpdate()
        {
            this.timer.Update();

            if(this.timer.HasElapsed())
            {
                Finish();
            }
        }

        private void Finish()
        {
            if(!string.IsNullOrEmpty(this.finishEvent))
            {
                GetOwner().SendEvent(this.finishEvent);
            }

            if(callBack != null)
                callBack();
        }
    }
}
