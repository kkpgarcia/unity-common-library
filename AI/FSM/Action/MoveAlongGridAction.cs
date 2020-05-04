using UnityEngine;
using System.Collections.Generic;

using System.Linq;

using Common.AI.Fsm;

namespace Common.AI.Fsm.Action
{
	public class MoveAlongGridAction : FsmActionAdapter
	{
		private Vector3 position;
		private Transform transform;
		private float speed;
		private List<Vector3> positionQueue;
		private Vector3 lastPosition;
		private float duration;
		private string finishEvent;
		private Space space;
		private Vector3 obstaclePosition;
		private CountdownTimer timer;
		private Vector3 CurrentPosition;

		public MoveAlongGridAction (FsmState owner) : base (owner)
		{
			this.timer = new CountdownTimer (1);
		}

		public MoveAlongGridAction (FsmState owner, string timeReferenceName) : base (owner)
		{
			this.timer = new CountdownTimer (1, timeReferenceName);
		}

		public void Init (Transform transform, int speed, List<Vector3> positionQueue, float duration, string finishEvent, Space space = Space.World)
		{
			this.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
			this.transform = transform;
			this.speed = speed;
			this.positionQueue = positionQueue;

			if (positionQueue != null)
				this.lastPosition = positionQueue [positionQueue.Count - 1];
			
			this.duration = duration;
			this.finishEvent = finishEvent;
			this.space = space;
		}

		public override void OnEnter ()
		{
			if (positionQueue.Count <= 0) {
				Finish ();
			}

			timer.Reset (this.duration);
		}

		public void AddObstacle (Vector2 position)
		{
			this.obstaclePosition = position;

			Vector3 p = this.positionQueue.Find (x => x == this.obstaclePosition);

			if (p != null) {
				int index = this.positionQueue.FindIndex (x => x == p);
				Vector3 p2 = this.positionQueue [index - 1];

				if (this.positionQueue [index - 1] != null)
					this.lastPosition = this.positionQueue [index - 1];

				positionQueue.Clear ();
				positionQueue.Add (p2);
			}

		}

		public override void OnUpdate ()
		{
			timer.Update ();

			if (positionQueue.Count > 0) {

				CurrentPosition = positionQueue [0];

				if (Vector3.Distance (this.transform.position, this.positionQueue [0]) > 0.025f) {
					switch (this.space) {
					case Space.World:
						this.position += (this.positionQueue [0] - this.position).normalized * this.speed * Time.deltaTime;
						this.transform.position = position;
						break;
					case Space.Self:
						//this.transform.localPosition = Vector3.MoveTowards (this.position2d, this.positionQueue [0].tilePosition, timer.GetRatio ());
						break;
					}

					if (Vector3.Distance (this.positionQueue [0], transform.position) < 0.025f) {
						switch (this.space) {
						case Space.World:
							this.position = positionQueue [0];
							this.transform.position = this.position;
							break;
						case Space.Self:
							//this.transform.localPosition = positionQueue [0].tilePosition;
							break;
						}

						positionQueue.RemoveAt (0);
					}
				}
			} else {
				Finish ();
			}
		}

		public Vector3 GetCurrentTile ()
		{
			if (positionQueue != null)
				return CurrentPosition;
			else
				return Vector3.zero;
		}

		private void Finish ()
		{
			switch (this.space) {
			case Space.World:
				this.transform.position = this.lastPosition;
				break;
			case Space.Self:
				this.transform.localPosition = this.lastPosition; 
				break;
			}

			if (!string.IsNullOrEmpty (finishEvent)) {
				GetOwner ().SendEvent (finishEvent);
			}

			obstaclePosition = new Vector2 (0, 0);
		}
	}
}