using UnityEngine;
using System.Collections;

using Common.AI.Fsm;

namespace Common.AI.Fsm.Action
{
    public class MoveAction : FsmActionAdapter
    {
        private Transform transform;
        private Vector3 positionFrom;
        private Vector3 positionTo;
        private float duration;
        private string timeReference;
        private string finishEvent;
        private Space space;

        private CountdownTimer timer;

        public MoveAction(FsmState owner) : base(owner)
        {
            this.timer = new CountdownTimer(1);
        }

        public MoveAction(FsmState owner, string timeReferenceName) : base(owner)
        {
            this.timer = new CountdownTimer(1, timeReferenceName);
        }

        public void Init(Transform transform, Vector3 positionFrom, Vector3 positionTo, float duration, string finishEvent, Space space = Space.World)
        {
            this.transform = transform;
            this.positionFrom = positionFrom;
            this.positionTo = positionTo;
            this.duration = duration;
            this.finishEvent = finishEvent;
            this.space = space;
        }

        public override void OnEnter()
        {
            if(Comparison.TolerantEquals(duration, 0))
            {
                Finish();
                return;
            }

            if(VectorUtils.Equals(this.positionFrom, this.positionTo))
            {
                Finish();
                return;
            }

            SetPosition(this.positionFrom);
            timer.Reset(this.duration);
        }

        public override void OnUpdate()
        {
            timer.Update();

            if(timer.HasElapsed())
            {
                Finish();
                return;
            }

            SetPosition(Vector3.Lerp(this.positionFrom, this.positionTo, timer.GetRatio()));
        }

        private void Finish()
        {
            SetPosition(this.positionTo);

            if(!string.IsNullOrEmpty(finishEvent))
            {
                GetOwner().SendEvent(finishEvent);
            }
        }

        private void SetPosition(Vector3 position)
        {
            switch(this.space)
            {
                case Space.World:
                    this.transform.position = position;
                    break;
                case Space.Self:
                    this.transform.localPosition = position;
                    break;
            }
        }
    }
}