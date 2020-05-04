using System;
using UnityEngine;
using Common.CustomTime;

namespace Common.AI.Fsm.Action
{
	public class MoveAlongDirectionByPolledTime : FsmActionAdapter
	{
		private Transform actor;
		private Vector3 direction;
		private float velocity;

		private TimeReference timeReference;
		private float polledtime;

		private Vector3 startPosition;

		public MoveAlongDirectionByPolledTime (FsmState owner, string timeReferenceName) : base (owner)
		{
			this.timeReference = TimeReferencePool.GetInstance ().Get (timeReferenceName);
		}

		public void Init (Transform actor, Vector3 direction, float velocity)
		{
			this.actor = actor;
			this.direction = direction;
			this.velocity = velocity;
		}

		public Vector3 Startposition {
			get {
				return startPosition;
			}
			set {
				startPosition = value;
			}
		}

		public override void OnEnter ()
		{
			if (Comparison.TolerantEquals (direction.sqrMagnitude, 0)) {
				Debug.LogError ("Direction cannot be zero.");
			}

			if (!Comparison.TolerantEquals (this.direction.sqrMagnitude, 1)) {
				this.direction.Normalize ();
			}

			this.polledtime = 0;
		}

		public override void OnUpdate ()
		{
			this.polledtime += this.timeReference.DeltaTime;
			Vector3 newPosition = this.startPosition + (this.direction * (this.velocity * this.polledtime));
			this.actor.position = newPosition;
		}
	}
}