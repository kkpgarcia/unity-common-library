using UnityEngine;
using System;
using System.Collections;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using Common.Utils;

namespace Common.Main.Service
{
	public abstract class CoroutineEnabledService
	{
		[Inject]
		public ICoroutineHost CoroutineHost { get; set; }

		protected Coroutine StartCoroutine (IEnumerator routine)
		{
			return CoroutineHost.StartCoroutine (routine);
		}

		protected void PerformAfterSeconds (Action action, float time)
		{
			StartCoroutine (PerformAfterSecondsCoroutine (action, time));
		}

		protected IEnumerator PerformAfterSecondsCoroutine (Action action, float time)
		{
			yield return new WaitForSeconds (time);
			action ();
		}

		protected void PerformAfterFrames (Action action, int framesToWait)
		{
			StartCoroutine (PerformAfterFramesCoroutine (action, framesToWait));
		}

		protected IEnumerator PerformAfterFramesCoroutine (Action action, int framesToWait)
		{
			for (int x = 0; x < framesToWait; x++)
				yield return null;
			action ();
		}
	}
}