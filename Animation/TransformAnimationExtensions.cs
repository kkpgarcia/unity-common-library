using UnityEngine;
using System;
using System.Collections;

namespace Common.Animation {
	public static class TransformAnimationExtensions
	{
		public static Tweener MoveTo (this Transform t, Vector3 position)
		{
			return MoveTo (t, position, Tweener.DefaultDuration);
		}
		
		public static Tweener MoveTo (this Transform t, Vector3 position, float duration)
		{
			return MoveTo (t, position, duration, Tweener.DefaultEquation);
		}
		
		public static Tweener MoveTo (this Transform t, Vector3 position, float duration, Func<float, float, float, float> equation)
		{
			return MoveTo(t, new TransformAnimationProperty() {
				Vector3Value = position,
				duration = duration,
				equation = equation,
			});
		}

		public static Tweener MoveTo(this Transform t, TransformAnimationProperty property) {
			TransformPositionTweener tweener = t.gameObject.AddComponent<TransformPositionTweener> ();
			tweener.Property = property;
			tweener.StartTweenValue = t.position;
			tweener.EndTweenValue = property.Vector3Value;
			tweener.Play();
			return tweener;
		}

		public static Tweener MoveToUsingCurve(this Transform t, Vector3 position, float duration, AnimationCurve curve) {
			TransformPositionTweener tweener = t.gameObject.AddComponent<TransformPositionTweener>();
			tweener.StartTweenValue = t.position;
			tweener.EndTweenValue = position;
			tweener.Property.duration = duration;
			tweener.Property.curve = curve;
			tweener.Property.useAnimationCurve = true;
			tweener.Play ();
			return tweener;
		}
		
		public static Tweener MoveToLocal (this Transform t, Vector3 position)
		{
			return MoveToLocal (t, position, Tweener.DefaultDuration);
		}
		
		public static Tweener MoveToLocal (this Transform t, Vector3 position, float duration)
		{
			return MoveToLocal (t, position, duration, Tweener.DefaultEquation);
		}
		
		public static Tweener MoveToLocal (this Transform t, Vector3 position, float duration, Func<float, float, float, float> equation)
		{
			return MoveToLocal(t, new TransformAnimationProperty() {
				Vector3Value = position,
				duration = duration,
				equation = equation,
			});
		}

		public static Tweener MoveToLocal(this Transform t, TransformAnimationProperty property) {
			TransformLocalPositionTweener tweener = t.gameObject.AddComponent<TransformLocalPositionTweener> ();
			tweener.Property = property;
			tweener.StartTweenValue = t.localPosition;
			tweener.EndTweenValue = property.Vector3Value;
			tweener.Play ();
			return tweener;
		}

		public static Tweener RotateToLocal (this Transform t, Vector3 euler, float duration, Func<float, float, float, float> equation)
		{
			return RotateToLocal(t, new TransformAnimationProperty() {
				Vector3Value = euler,
				duration = duration,
				equation = equation,
			});
		}

		public static Tweener RotateToLocal(this Transform t, TransformAnimationProperty property) {
			TransformLocalEulerTweener tweener = t.gameObject.AddComponent<TransformLocalEulerTweener> ();
			tweener.Property = property;
			tweener.StartTweenValue = t.localEulerAngles;
			tweener.EndTweenValue = property.Vector3Value;
			tweener.Play ();
			return tweener;
		}
		
		public static Tweener RotateToLocalUsingCurve(this Transform t, Vector3 euler, float duration, AnimationCurve curve) {
			TransformLocalEulerTweener tweener = t.gameObject.AddComponent<TransformLocalEulerTweener>();
			tweener.StartTweenValue = t.localEulerAngles;
			tweener.EndTweenValue = euler;
			tweener.Property.duration = duration;
			tweener.Property.curve = curve;
			tweener.Property.useAnimationCurve = true;
			tweener.Play ();
			return tweener;
		}
		
		public static Tweener ScaleTo (this Transform t, Vector3 scale)
		{
			return ScaleTo (t, scale, Tweener.DefaultDuration);
		}
		
		public static Tweener ScaleTo (this Transform t, Vector3 scale, float duration)
		{
			return ScaleTo (t, scale, duration, Tweener.DefaultEquation);
		}
		
		public static Tweener ScaleTo (this Transform t, Vector3 scale, float duration, Func<float, float, float, float> equation)
		{
			return ScaleTo(t, new TransformAnimationProperty() {
				Vector3Value = scale,
				duration = duration,
				equation = equation,
			});
		}

		public static Tweener ScaleTo(this Transform t, TransformAnimationProperty property) {
			TransformScaleTweener tweener = t.gameObject.AddComponent<TransformScaleTweener> ();
			tweener.Property = property;
			tweener.StartTweenValue = t.localScale;
			tweener.EndTweenValue = property.Vector3Value;
			tweener.Play ();
			return tweener;
		}
		
		public static Tweener ScaleToUsingCurve(this Transform t, Vector3 scale, float duration, AnimationCurve curve) {
			TransformScaleTweener tweener = t.gameObject.AddComponent<TransformScaleTweener>();
			tweener.StartTweenValue = t.localScale;
			tweener.EndTweenValue = scale;
			tweener.Property.duration = duration;
			tweener.Property.curve = curve;
			tweener.Property.useAnimationCurve = true;
			tweener.Play ();
			return tweener;
		}
	}
}