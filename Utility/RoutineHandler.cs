using UnityEngine;
using System;
using System.Collections;
using strange.extensions.injector.api;
using strange.extensions.context.api;

namespace Common.Utils
{
	[Implements(typeof(IRoutineHandler), InjectionBindingScope.CROSS_CONTEXT)]
	public class RoutineHandler : IRoutineHandler
	{

		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextView { get; set; }

		private RoutineHandlerMB monobehaviour;

		[PostConstruct]
		public void PostConstruct ()
		{
			//if (contextView.GetComponent<RoutineHandlerMB> () == null)
			monobehaviour = contextView.AddComponent<RoutineHandlerMB> ();
		}

		public Coroutine StartCoroutine (IEnumerator routine)
		{
			return monobehaviour.StartCoroutine (routine);
		}

	}

	public class RoutineHandlerMB : MonoBehaviour
	{
	}
}
