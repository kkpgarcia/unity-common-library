using System.Linq;
using UnityEngine;

namespace Common.Inputs
{
	public abstract class Slot : MonoBehaviour, ISlot
	{
		protected abstract System.Type[] AcceptedTypes { get; }

		public abstract void OnDragHover(Manipulatable manipulatable);
		public abstract void OnReceive(Manipulatable manipulatable);
		protected virtual bool IsTypeAllowed(Manipulatable manipulatable)
		{
			return AcceptedTypes.Contains(manipulatable.GetType());
		}
	}
}
