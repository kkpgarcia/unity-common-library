using UnityEngine;
using System.Collections;

namespace Common.StateMachines {
	public abstract class State : MonoBehaviour 
	{
		public virtual void Enter ()
		{
			AddListeners();
		}
		
		public virtual void Exit ()
		{
			RemoveListeners();
		}

		public virtual void OnUpdate() {}

		protected virtual void OnDestroy ()
		{
			RemoveListeners();
		}

		protected virtual void AddListeners ()
		{

		}
		
		protected virtual void RemoveListeners ()
		{

		}
	}
}