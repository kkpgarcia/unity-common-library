using UnityEngine;

namespace Common.UI
{
	public class UIMovementJoint : MonoBehaviour
	{
		[SerializeField] private RectTransform _parent;
		private RectTransform _referencePositionA;
		private RectTransform _referencePositionB;

		public void SetReferencePoints(RectTransform a, RectTransform b)
		{
			this._referencePositionA = a;
			this._referencePositionB = b;
		}

		public RectTransform GetReferencePointA()
		{
			return _referencePositionA;
		}

		public RectTransform GetReferencePointB()
		{
			return _referencePositionB;
		}

		void Update()
		{
			if (_referencePositionA == null || _referencePositionB == null) return;

			Vector3 centerPoint = (_referencePositionA.position + _referencePositionB.position) / 2;
			_parent.position = centerPoint;
		}
	}
}
