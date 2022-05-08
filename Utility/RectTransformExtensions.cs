using UnityEngine;

namespace Common.Utility
{
    public static class RectTransformExtensions
    {
        public static Rect GetGlobalPosition(this RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
        }

        public static bool IsMouseOver(this RectTransform rectTransform)
        {
            Rect position = rectTransform.GetGlobalPosition();
            return position.Contains(Input.mousePosition);
        }

    }
}