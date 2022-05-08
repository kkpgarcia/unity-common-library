using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
	public struct UIPoint
	{
		public Vector2 Position;
		public float Angle;
		public float Thickness;
	}

	public class UILine
	{
		// private UIPoint[] _points;
		private List<UIPoint> _points = new List<UIPoint>();
		private Color _color;

		public UILine(Color color)
		{
			this._points = new List<UIPoint>();
			this._color = color;
		}

		public void AddPoint(UIPoint point)
		{
			this._points.Add(point);
		}

		public void Render(VertexHelper helper)
		{
			foreach (UIPoint point in _points)
			{
				DrawVerticesForPoint(point, helper);
			}

			for (int i = 0; i < _points.Count - 1; i++)
			{
				int index = i * 2;

				helper.AddTriangle(index + 0, index + 1, index + 3);
				helper.AddTriangle(index + 3, index + 2, index + 0);
			}
		}

		public void DrawPoints(UIPoint[] points, int index, VertexHelper helper)
		{
			foreach (UIPoint point in points)
			{
				DrawVerticesForPoint(point, helper);
			}

			for (int i = 0; i < points.Length - 1; i++)
			{
				int idx = (i * 2) + (index * 6);

				helper.AddTriangle(idx + 0, idx + 1, idx + 3);
				helper.AddTriangle(idx + 3, idx + 2, idx + 0);
			}
		}

		public void Clear()
		{
			this._points.Clear();
		}

		private void DrawVerticesForPoint(UIPoint point, VertexHelper helper)
		{
			UIVertex vertex = UIVertex.simpleVert;
			vertex.color = _color;

			vertex.position = Quaternion.Euler(0, 0, point.Angle) * new Vector3(-point.Thickness/2, 0, 0);
			vertex.position += new Vector3 (point.Position.x, point.Position.y);

			helper.AddVert(vertex);

			vertex.position = Quaternion.Euler(0, 0, point.Angle) * new Vector3 (point.Thickness/2, 0,0);
			vertex.position += new Vector3(point.Position.x, point.Position.y);

			helper.AddVert(vertex);
		}
	}
}
