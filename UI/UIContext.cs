using UnityEngine;

namespace Common.UI
{
	public class UIContext : MonoBehaviour
	{
		public UIHandle[] Handles;
		private RectTransform _rectTransform;

		public void Start()
		{
			_rectTransform = this.GetComponent<RectTransform>();
		}

		public void CloseContext()
		{
			Destroy(this.gameObject);
		}

		public Vector2 GetPosition()
		{
			return _rectTransform.position;
		}
	}
}
