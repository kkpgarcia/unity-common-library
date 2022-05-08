using System;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
	public class UILineConnector : MaskableGraphic
	{
		[SerializeField] private RectTransform _A;
		[SerializeField] private RectTransform _B;
		[SerializeField] private float _lineThickness = 2;

		public void SetConnections(RectTransform a, RectTransform b)
		{
			this._A = a;
			this._B = b;

			this.enabled = true;
		}

		public void RemoveConnections()
		{
			this._A = null;
			this._B = null;

			this.enabled = false;
		}

		public void SetThickness(float value)
		{
			this._lineThickness = _lineThickness;
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (_A == null || _B == null) return;

			vh.Clear();

			Vector2 start = GetRelativePosition(_A.position);
			Vector2 cornerPoint = GetRelativePosition(new Vector2(_B.position.x, _A.position.y));
			Vector2 end = GetRelativePosition(_B.position);

			if (start.x < end.x)
			{
				DrawVerticesForPoint(start, vh, start.y > end.y ? 90 : -90, _lineThickness);
				DrawVerticesForPoint(cornerPoint, vh, start.y > end.y ? 45 : -45, _lineThickness + (_lineThickness / 2));
			}
			else
			{
				DrawVerticesForPoint(start, vh, start.y > end.y ? -90 : 90, _lineThickness);
				DrawVerticesForPoint(cornerPoint, vh, start.y > end.y ? -45 : 45, _lineThickness + (_lineThickness / 2));
			}

			DrawVerticesForPoint(end, vh, 0, _lineThickness);

			for (int i = 0; i < 3 - 1; i++)
			{
				int index = i * 2;

				vh.AddTriangle(index + 0, index + 1, index + 3);
				vh.AddTriangle(index + 3, index + 2, index + 0);
			}
		}

		public void Update()
		{
			if (_A == null || _B == null) return;

			this.SetAllDirty();
		}

		void DrawVerticesForPoint(Vector2 point, VertexHelper vh, float angle, float thickness)
		{
			UIVertex vertex = UIVertex.simpleVert;
			vertex.color = color;

			vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0, 0);
			vertex.position += new Vector3(point.x, point.y);

			vh.AddVert(vertex);

			vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0, 0);
			vertex.position += new Vector3(point.x, point.y);

			vh.AddVert(vertex);
		}

		public Vector2 GetRelativePosition(Vector2 position)
		{
			return this.transform.parent.InverseTransformPoint(position);
		}
	}
}
