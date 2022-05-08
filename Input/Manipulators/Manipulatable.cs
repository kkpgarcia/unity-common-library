using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common.Inputs
{
	public abstract class Manipulatable : MonoBehaviour, IManipulatable
	{
		private List<IManipulator> _manipulators;
		public abstract RectTransform Transform { get; }

		protected virtual void Awake()
		{
			_manipulators = new List<IManipulator>();
		}

		protected virtual void Start()
		{
			this.CreateManipulators();
		}

		protected abstract void CreateManipulators();

		protected void AddManipulator(IManipulator manipulator)
		{
			this._manipulators.Add(manipulator);
		}

		protected void RemoveManipulator(IManipulator manipulator)
		{
			this._manipulators.Remove(manipulator);
		}

		public IManipulator[] GetManipulators()
		{
			return _manipulators.ToArray();
		}

		public static Manipulatable GetImmediate(GameObject[] selection)
		{
			IEnumerable<Manipulatable> manipulatables =
				selection.Select(x => x.GetComponent<Manipulatable>());
			return manipulatables.FirstOrDefault(x => x != null);
		}
	}
}
