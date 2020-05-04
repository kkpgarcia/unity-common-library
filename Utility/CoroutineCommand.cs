using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.context.impl;

using Common.Utils;

namespace Common.Main.Controller
{
	public abstract class CoroutineCommand : Command
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
			float currTime = 0;
			bool performAction = true;

			while (currTime < time) {
				if (!retain) {
					performAction = false;
					yield break;
				}
				currTime += Time.deltaTime;
				yield return null;
			}

			if (performAction) {
				action ();
			}
		}

		protected void PerformActionsAfterSeconds (Dictionary<Action, float> actions)
		{
			foreach (KeyValuePair<Action, float> entry in actions) {
				PerformAfterSeconds (entry.Key, entry.Value);
			}
		}

		protected void PerformAfterFrames (Action action, int framesToWait)
		{
			StartCoroutine (PerformAfterFramesCoroutine (action, framesToWait));
		}

		protected IEnumerator PerformAfterFramesCoroutine (Action action, int framesToWait)
		{
			bool performAction = true;

			for (int x = 0; x < framesToWait; x++) {
				if (retain) {
					performAction = false;
					yield break;
				}

				yield return null;
			}

			if (performAction) {
				action ();
			}
		}

		protected void PerfromAfterFramesCoroutine (Dictionary<Action, int> actions)
		{
			foreach (KeyValuePair<Action, int> entry in actions) {
				PerformAfterFrames (entry.Key, entry.Value);
			}
		}
	}
}
