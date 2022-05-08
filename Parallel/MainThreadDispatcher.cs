using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Common.Events;


namespace Common.Parallel
{
	public class MainThreadDispatcher : MonoBehaviour
	{
		public static readonly Queue<Action> _executionQueue = new Queue<Action>();

		private static MainThreadDispatcher _instance = null;

		public static MainThreadDispatcher Instance() {

			return _instance;
		}

		public void Update() {
			lock(_executionQueue) {
				while (_executionQueue.Count > 0) {
					_executionQueue.Dequeue().Invoke();
				}
			}
		}

		public void Enqueue(IEnumerator action) {
			lock (_executionQueue) {
				_executionQueue.Enqueue (() => {
					StartCoroutine (action);
				});
			}
		}

		public void Enqueue(Action action)
		{
			Enqueue(ActionWrapper(action));
		}

		public Task EnqueueAsync(Action action)
		{
			var tcs = new TaskCompletionSource<bool>();

			void WrappedAction() {
				try
				{
					action();
					tcs.TrySetResult(true);
				} catch (Exception ex)
				{
					tcs.TrySetException(ex);
				}
			}

			Enqueue(ActionWrapper(WrappedAction));
			return tcs.Task;
		}

		IEnumerator ActionWrapper(Action a)
		{
			a();
			yield return null;
		}


		public static bool Exists() {
			return _instance != null;
		}

		void Awake() {
			if (_instance == null) {
				_instance = this;
				DontDestroyOnLoad(this.gameObject);
			}
		}

		void OnDestroy() {
			_instance = null;
		}
	}
}
