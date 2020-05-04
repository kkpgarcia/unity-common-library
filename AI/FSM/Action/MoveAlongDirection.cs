using UnityEngine;
using System.Collections;

using Common.AI.Fsm;
using Common.CustomTime;

namespace Common.AI.Fsm.Action
{
	public class MoveAlongDirection : FsmActionAdapter
	{
		private Transform actor;
		private Vector3 direction;
		private float velocity;

		private TimeReference timeReference;

		public MoveAlongDirection (FsmState owner, string timeReferenceName) : base (owner)
		{
			this.timeReference = TimeReferencePool.GetInstance ().Get (timeReferenceName);
		}

		public void Init (Transform actor, Vector3 direction, float velocity)
		{
			this.actor = actor;
			this.direction = direction;
			this.velocity = velocity;
		}

		public override void OnEnter ()
		{
			if (Comparison.TolerantEquals (direction.sqrMagnitude, 0)) {
				Debug.LogError ("Direction cannot be zero.");
				return;
			}

			if (!Comparison.TolerantEquals (this.direction.sqrMagnitude, 1)) {
				this.direction.Normalize ();
			}
		}

		public override void OnUpdate ()
		{
			Vector3 displacement = this.direction * (this.velocity * this.timeReference.DeltaTime);
			this.actor.position = this.actor.position + displacement;
		}
	}
}