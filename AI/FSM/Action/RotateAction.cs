using System;

using UnityEngine;

using Common.AI.Fsm;
using Common.CustomTime;

namespace Common.AI.Fsm.Action
{
    public class RotateAction : FsmActionAdapter
    {
        private Transform transform;
        private Quaternion quatFrom;
        private Quaternion quatTo;
        private float duration;
        private string finishEvent;

        private CountdownTimer timer;

        public RotateAction(FsmState owner) : base(owner)
        {
            this.timer = new CountdownTimer(1);
        }

        public RotateAction(FsmState owner, string timerReferenceName) : base(owner)
        {
            this.timer = new CountdownTimer(1, timerReferenceName);
        }

        public void Init(Transform transform, Quaternion quatFrom, Quaternion quatTo, float duration, string finishEvent)
        {
            this.transform = transform;
            this.quatFrom = quatFrom;
            this.quatTo = quatTo;
            this.duration = duration;
            this.finishEvent = finishEvent;
        }

        public override void OnEnter()
        {
            if(Comparison.TolerantEquals(this.duration, 0))
            {
                Finish();
                return;
            }

            if(Quaternion.Equals(this.quatFrom, this.quatTo))
            {
                Finish();
                return;
            }

            this.transform.rotation = this.quatFrom;
            timer.Reset(this.duration);
        }

        public override void OnUpdate()
        {
            this.timer.Update();

            if(this.timer.HasElapsed())
            {
                Finish();
                return;
            }

            this.transform.rotation = Quaternion.Lerp(this.quatFrom, this.quatTo, this.timer.GetRatio());
        }

        private void Finish()
        {
            this.transform.rotation = this.quatTo;

            if(!string.IsNullOrEmpty(finishEvent))
            {
                GetOwner().SendEvent(finishEvent);
            }
        }
    }
}