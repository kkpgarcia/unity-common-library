using System;

using UnityEngine;

using Common.AI.Fsm;
using Common.CustomTime;

namespace Common.AI.Fsm.Action
{
	public class ScaleAction : FsmActionAdapter
	{
		private Transform transform;
		private Vector3 scaleFrom;
		private Vector3 scaleTo;
		private float duration;
		private string timeReference;
		private string finishEvent;

		private CountdownTimer timer;

		public ScaleAction (FsmState owner, string timerReference) : base (owner)
		{
			this.timeReference = timerReference;
			timer = new CountdownTimer (1, timeReference);
		}

		public void Init (Transform transform, Vector3 scaleFrom, Vector3 scaleTo, float duration, string finishEvent)
		{
			this.transform = transform;
			this.scaleFrom = scaleFrom;
			this.scaleTo = scaleTo;
			this.duration = duration;
			this.finishEvent = finishEvent;
		}

		public override void OnEnter ()
		{
			if (Comparison.TolerantEquals (duration, 0)) {
				Finish ();
				return;
			}

			if (VectorUtils.Equals (scaleFrom, scaleTo)) {
				Finish ();
				return;
			}

			transform.localScale = scaleFrom;
			timer.Reset (this.duration);
		}

		public override void OnUpdate ()
		{
			timer.Update ();

			if (timer.HasElapsed ()) {
				Finish ();
				return;
			}

			transform.localScale = Vector3.Lerp (scaleFrom, scaleTo, timer.GetRatio ());
		}

		private void Finish ()
		{
			this.transform.localScale = scaleTo;

			if (!string.IsNullOrEmpty (finishEvent)) {
				GetOwner ().SendEvent (finishEvent);
			}
		}
	}
}