using UnityEngine;
using System.Collections;
using Common.Animation;

namespace Common.UI {
	[RequireComponent(typeof(RectTransform))]
	public class LayoutAnchor : MonoBehaviour 
	{
		#region Fields / Properties
		RectTransform MyRectTransform;
		RectTransform ParentRectTransform;
		#endregion

		#region MonoBehaviour
		void Awake ()
		{
			MyRectTransform = transform as RectTransform;
			ParentRectTransform = transform.parent as RectTransform;
			if (ParentRectTransform == null)
				Debug.LogError( "This component requires a RectTransform parent to work.", gameObject );
		}
		#endregion

		#region Public
		public void SnapToAnchorPosition (TextAnchor myAnchor, TextAnchor parentAnchor, Vector2 offset)
		{
			MyRectTransform.anchoredPosition = AnchorPosition(myAnchor, parentAnchor, offset);
		}

		public Tweener MoveToAnchorPosition (TextAnchor myAnchor, TextAnchor parentAnchor, Vector2 offset)
		{
			return MyRectTransform.AnchorTo(AnchorPosition(myAnchor, parentAnchor, offset));
		}

		public Vector2 AnchorPosition (TextAnchor myAnchor, TextAnchor parentAnchor, Vector2 offset)
		{
			Vector2 myOffset = GetPosition(MyRectTransform, myAnchor);
			Vector2 parentOffset = GetPosition(ParentRectTransform, parentAnchor);
			Vector2 anchorCenter = new Vector2( Mathf.Lerp(MyRectTransform.anchorMin.x, MyRectTransform.anchorMax.x, MyRectTransform.pivot.x), Mathf.Lerp(MyRectTransform.anchorMin.y, MyRectTransform.anchorMax.y, MyRectTransform.pivot.y) );
			Vector2 myAnchorOffset = new Vector2(ParentRectTransform.rect.width * anchorCenter.x, ParentRectTransform.rect.height * anchorCenter.y);
			Vector2 myPivotOffset = new Vector2(MyRectTransform.rect.width * MyRectTransform.pivot.x, MyRectTransform.rect.height * MyRectTransform.pivot.y);
			Vector2 pos = parentOffset - myAnchorOffset - myOffset + myPivotOffset + offset;
			pos.x = Mathf.RoundToInt(pos.x);
			pos.y = Mathf.RoundToInt(pos.y);
			return pos;
		}
		#endregion

		#region Private
		Vector2 GetPosition (RectTransform rt, TextAnchor anchor)
		{
			Vector2 retValue = Vector2.zero;
			
			switch (anchor)
			{
			case TextAnchor.LowerCenter: 
			case TextAnchor.MiddleCenter: 
			case TextAnchor.UpperCenter:
				retValue.x += rt.rect.width * 0.5f;
				break;
			case TextAnchor.LowerRight: 
			case TextAnchor.MiddleRight: 
			case TextAnchor.UpperRight:
				retValue.x += rt.rect.width;
				break;
			}
			
			switch (anchor)
			{
			case TextAnchor.MiddleLeft: 
			case TextAnchor.MiddleCenter: 
			case TextAnchor.MiddleRight:
				retValue.y += rt.rect.height * 0.5f;
				break;
			case TextAnchor.UpperLeft: 
			case TextAnchor.UpperCenter: 
			case TextAnchor.UpperRight:
				retValue.y += rt.rect.height;
				break;
			}
			
			return retValue;
		}
		#endregion
	}
}