﻿using UnityEngine;
using System;
using System.Collections;

namespace Common.Animation {
	public static class AudioSourceAnimationExtensions 
	{
		public static Tweener VolumeTo (this AudioSource s, float volume)
		{
			return VolumeTo(s, volume, Tweener.DefaultDuration);
		}

		public static Tweener VolumeTo (this AudioSource s, float volume, float duration)
		{
			return VolumeTo(s, volume, duration, Tweener.DefaultEquation);
		}

		public static Tweener VolumeTo (this AudioSource s, float volume, float duration, Func<float, float, float, float> equation)
		{
			AudioSourceVolumeTweener tweener = s.gameObject.AddComponent<AudioSourceVolumeTweener>();
			tweener.Source = s;
			tweener.startValue = s.volume;
			tweener.endValue = volume;
			tweener.Property.duration = duration;
			tweener.Property.equation = equation;
			tweener.Play ();
			return tweener;
		}
	}
}