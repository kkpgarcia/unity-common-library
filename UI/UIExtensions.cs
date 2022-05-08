using UnityEngine;

namespace Common.UI
{
	public static class UIExtensions
	{
		public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
		{
			Vector2 size = rectTransform.rect.size;
			Vector2 deltaPivot = rectTransform.pivot - pivot;
			Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
			rectTransform.pivot = pivot;
			rectTransform.localPosition -= deltaPosition;
		}
	}
}
