using UnityEngine;
using System.Collections;

namespace Common.Animation {
	public class RectTransformAnchorPositionTweener : Vector3Tweener 
	{
		RectTransform RectTransform;
		
		void Awake ()
		{
			RectTransform = transform as RectTransform;
		}

		protected override void OnUpdate ()
		{
			base.OnUpdate ();
			RectTransform.anchoredPosition = CurrentTweenValue;
		}
	}
}