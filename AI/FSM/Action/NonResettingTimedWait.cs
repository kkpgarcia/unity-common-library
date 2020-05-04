using System;

using Common.AI.Fsm;
using Common.CustomTime;

namespace Common.AI.Fsm.Action
{
    public class NonResettingTimedWait : FsmActionAdapter
    {
        private float waitTime;
        private readonly CountdownTimer timer;
        private readonly string timeReference;
        private readonly string finishEvent;
        
        public NonResettingTimedWait(FsmState owner, string timeReferenceName, string finishEvent) : base(owner)
        {
            if(string.IsNullOrEmpty(timeReferenceName))
            {
                this.timer = new CountdownTimer(1);
            }
            else
            {
                this.timer = new CountdownTimer(1, timeReferenceName);
            }

            this.finishEvent = finishEvent;
        }

        public void Init(float waitTime)
        {
            this.waitTime = waitTime;
            this.timer.Reset(this.waitTime);
        }

        public override void OnEnter()
        {
            if(waitTime <= 0)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            timer.Update();

            if(timer.HasElapsed())
            {
                Finish();
            }
        }

        private void Finish()
        {
            GetOwner().SendEvent(finishEvent);
        }

        public float GetRatio()
        {
            return timer.GetRatio();
        }
    }
}