using UnityEngine;
using System.Collections;

namespace Common.Animation {
	public class TransformPositionTweener : Vector3Tweener 
	{
		protected override void OnUpdate ()
		{
			base.OnUpdate ();
			transform.position = CurrentTweenValue;
		}
	}	
}